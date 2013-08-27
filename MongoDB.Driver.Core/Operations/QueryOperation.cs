﻿/* Copyright 2010-2013 10gen Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver.Core.Diagnostics;
using MongoDB.Driver.Core.Protocol;
using MongoDB.Driver.Core.Protocol.Messages;
using MongoDB.Driver.Core.Sessions;
using MongoDB.Driver.Core.Support;

namespace MongoDB.Driver.Core.Operations
{
    /// <summary>
    /// Represents a Query operation.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    public sealed class QueryOperation<TDocument> : QueryOperationBase<ICursor<TDocument>>, IEnumerable<TDocument>
    {
        // private fields
        private int _batchSize;
        private CollectionNamespace _collection;
        private object _fields;
        private QueryFlags _flags;
        private int _limit;
        private BsonDocument _options;
        private object _query;
        private ReadPreference _readPreference;
        private IBsonSerializer<TDocument> _serializer;
        private int _skip;

        // constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryOperation{TDocument}" /> class.
        /// </summary>
        public QueryOperation()
        {
            _readPreference = ReadPreference.Primary;
        }

        // public properties
        /// <summary>
        /// Gets or sets the size of the batch.
        /// </summary>
        public int BatchSize
        {
            get { return _batchSize; }
            set { _batchSize = value; }
        }

        /// <summary>
        /// Gets or sets the collection.
        /// </summary>
        public CollectionNamespace Collection
        {
            get { return _collection; }
            set { _collection = value; }
        }

        /// <summary>
        /// Gets or sets the fields.
        /// </summary>
        public object Fields
        {
            get { return _fields; }
            set { _fields = value; }
        }

        /// <summary>
        /// Gets or sets the flags.
        /// </summary>
        public QueryFlags Flags
        {
            get { return _flags; }
            set { _flags = value; }
        }

        /// <summary>
        /// Gets or sets the limit.
        /// </summary>
        public int Limit
        {
            get { return _limit; }
            set { _limit = value; }
        }

        /// <summary>
        /// Gets or sets the query options.
        /// </summary>
        public BsonDocument Options
        {
            get { return _options; }
            set { _options = value; }
        }

        /// <summary>
        /// Gets or sets the query object.
        /// </summary>
        public object Query
        {
            get { return _query; }
            set { _query = value; }
        }

        /// <summary>
        /// Gets or sets the read preference.
        /// </summary>
        public ReadPreference ReadPreference
        {
            get { return _readPreference; }
            set { _readPreference = value; }
        }

        /// <summary>
        /// Gets or sets the serializer.
        /// </summary>
        public IBsonSerializer<TDocument> Serializer
        {
            get { return _serializer; }
            set { _serializer = value; }
        }

        /// <summary>
        /// Gets or sets the skip.
        /// </summary>
        public int Skip
        {
            get { return _skip; }
            set { _skip = value; }
        }

        // public methods
        /// <summary>
        /// Executes the Query operation.
        /// </summary>
        /// <returns>A cursor to enumerate over the results.</returns>
        public override ICursor<TDocument> Execute()
        {
            EnsureRequiredProperties();

            // NOTE: not disposing of channel provider here because it will get disposed
            // by the cursor
            var activity = __trace.TraceActivity("QueryOperation");
            var channelProvider = CreateServerChannelProvider(new ReadPreferenceServerSelector(_readPreference), true);
            try
            {
                if (__trace.Switch.ShouldTrace(TraceEventType.Verbose))
                {
                    __trace.TraceVerbose("running query {0}, fields {1}, skip {2}, limit {3}, and batch size {4} on collection {5} at {6}.", _query.ToJson(), _fields.ToJson(), _skip, _limit, _batchSize, _collection.FullName, channelProvider.Server.DnsEndPoint);
                }

                var flags = _flags;
                if (_readPreference.ReadPreferenceMode != ReadPreferenceMode.Primary)
                {
                    flags |= QueryFlags.SlaveOk;
                }

                var readerSettings = GetServerAdjustedReaderSettings(channelProvider.Server);
                var protocol = new QueryProtocol<TDocument>(
                    collection: _collection,
                    fields: _fields,
                    flags: flags,
                    numberToReturn: CalculateNumberToReturnForFirstBatch(),
                    query: WrapQuery(channelProvider.Server, _query, _options, _readPreference),
                    readerSettings: readerSettings,
                    serializer: _serializer,
                    skip: _skip,
                    writerSettings: GetServerAdjustedWriterSettings(channelProvider.Server));

                using (var channel = channelProvider.GetChannel(Timeout, CancellationToken))
                {
                    var result = protocol.Execute(channel);
                    return new Cursor<TDocument>(
                        activity,
                        channelProvider: channelProvider,
                        cursorId: result.CursorId,
                        collection: _collection,
                        limit: Math.Abs(_limit),
                        numberToReturn: _batchSize,
                        firstBatch: result.Documents,
                        serializer: Serializer,
                        timeout: Timeout,
                        cancellationToken: CancellationToken,
                        readerSettings: readerSettings);
                }
            }
            catch
            {
                channelProvider.Dispose();
                activity.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>An enumerator.</returns>
        public IEnumerator<TDocument> GetEnumerator()
        {
            return Execute();
        }

        // explicit interface implementations
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // protected methods
        /// <summary>
        /// Ensures that required properties have been set or provides intelligent defaults.
        /// </summary>
        protected override void EnsureRequiredProperties()
        {
            base.EnsureRequiredProperties();
            Ensure.IsGreaterThanOrEqualTo("BatchSize", _batchSize, 0);
            Ensure.IsNotNull("Collection", _collection);
            Ensure.IsNotNull("Query", _query);
            Ensure.IsNotNull("ReadPreference", _readPreference);
            if (_serializer == null)
            {
                _serializer = BsonSerializer.LookupSerializer<TDocument>();
            }
        }

        // private methods
        private int CalculateNumberToReturnForFirstBatch()
        {
            if (_limit < 0)
            {
                return _limit;
            }
            else if (_limit == 0)
            {
                return _batchSize;
            }
            else if (_batchSize == 0)
            {
                return _limit;
            }
            else if (_limit < _batchSize)
            {
                return _limit;
            }

            return _batchSize;
        }
    }
}
