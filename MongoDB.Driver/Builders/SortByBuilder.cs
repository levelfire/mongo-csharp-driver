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
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver.Linq.Utils;

namespace MongoDB.Driver.Builders
{
    /// <summary>
    /// A builder for specifying a sort order.
    /// </summary>
    public static class SortBy
    {
        // public static properties
        /// <summary>
        /// Gets a null value with a type of IMongoSortBy.
        /// </summary>
        public static IMongoSortBy Null
        {
            get { return null; }
        }

        // public static methods
        /// <summary>
        /// Adds keys to be sorted by in ascending order.
        /// </summary>
        /// <param name="keys">One or more key names.</param>
        /// <returns>The builder (so method calls can be chained).</returns>
        public static SortByBuilder Ascending(params string[] keys)
        {
            return new SortByBuilder().Ascending(keys);
        }

        /// <summary>
        /// Adds keys to be sorted by in descending order.
        /// </summary>
        /// <param name="keys">One or more key names.</param>
        /// <returns>The builder (so method calls can be chained).</returns>
        public static SortByBuilder Descending(params string[] keys)
        {
            return new SortByBuilder().Descending(keys);
        }
    }

    /// <summary>
    /// A builder for specifying a sort order.
    /// </summary>
    [Serializable]
    [BsonSerializer(typeof(SortByBuilder.Serializer))]
    public class SortByBuilder : BuilderBase, IMongoSortBy
    {
        // private fields
        private BsonDocument _document;

        // constructors
        /// <summary>
        /// Initializes a new instance of the SortByBuider class.
        /// </summary>
        public SortByBuilder()
        {
            _document = new BsonDocument();
        }

        // public methods
        /// <summary>
        /// Adds keys to be sorted by in ascending order.
        /// </summary>
        /// <param name="keys">One or more key names.</param>
        /// <returns>The builder (so method calls can be chained).</returns>
        public SortByBuilder Ascending(params string[] keys)
        {
            foreach (var key in keys)
            {
                _document.Add(key, 1);
            }
            return this;
        }

        /// <summary>
        /// Adds keys to be sorted by in descending order.
        /// </summary>
        /// <param name="keys">One or more key names.</param>
        /// <returns>The builder (so method calls can be chained).</returns>
        public SortByBuilder Descending(params string[] keys)
        {
            foreach (var key in keys)
            {
                _document.Add(key, -1);
            }
            return this;
        }

        /// <summary>
        /// Returns the result of the builder as a BsonDocument.
        /// </summary>
        /// <returns>A BsonDocument.</returns>
        public override BsonDocument ToBsonDocument()
        {
            return _document;
        }

        // nested classes
        internal class Serializer : BsonBaseSerializer<SortByBuilder>
        {
            public override void Serialize(SerializationContext context, SortByBuilder value)
            {
                context.SerializeWithChildContext(BsonDocumentSerializer.Instance, value._document);
            }
        }
    }

    /// <summary>
    /// A builder for specifying a sort order.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    public static class SortBy<TDocument>
    {
        /// <summary>
        /// Adds keys to be sorted by in ascending order.
        /// </summary>
        /// <param name="memberExpressions">The member expressions.</param>
        /// <returns>
        /// The builder (so method calls can be chained).
        /// </returns>
        public static SortByBuilder<TDocument> Ascending(params Expression<Func<TDocument, object>>[] memberExpressions)
        {
            return new SortByBuilder<TDocument>().Ascending(memberExpressions);
        }

        /// <summary>
        /// Adds keys to be sorted by in descending order.
        /// </summary>
        /// <param name="memberExpressions">The member expressions.</param>
        /// <returns>
        /// The builder (so method calls can be chained).
        /// </returns>
        public static SortByBuilder<TDocument> Descending(params Expression<Func<TDocument, object>>[] memberExpressions)
        {
            return new SortByBuilder<TDocument>().Descending(memberExpressions);
        }
    }

    /// <summary>
    /// A builder for specifying a sort order.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    [Serializable]
    [BsonSerializer(typeof(SortByBuilder<>.Serializer))]
    public class SortByBuilder<TDocument> : BuilderBase, IMongoSortBy
    {
        // private fields
        private readonly BsonSerializationInfoHelper _serializationInfoHelper;
        private SortByBuilder _sortByBuilder;

        // constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SortByBuilder&lt;TDocument&gt;"/> class.
        /// </summary>
        public SortByBuilder()
            : this(new BsonSerializationInfoHelper())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SortByBuilder{TDocument}" /> class.
        /// </summary>
        /// <param name="serializationInfoHelper">The serialization info helper.</param>
        internal SortByBuilder(BsonSerializationInfoHelper serializationInfoHelper)
        {
            _serializationInfoHelper = serializationInfoHelper;
            _sortByBuilder = new SortByBuilder();
        }

        // public methods
        /// <summary>
        /// Adds keys to be sorted by in ascending order.
        /// </summary>
        /// <param name="memberExpressions">The member expressions indicating which elements to sort by.</param>
        /// <returns>
        /// The builder (so method calls can be chained).
        /// </returns>
        public SortByBuilder<TDocument> Ascending(params Expression<Func<TDocument, object>>[] memberExpressions)
        {
            var elementNames = GetElementNames(memberExpressions);
            _sortByBuilder = _sortByBuilder.Ascending(elementNames.ToArray());
            return this;
        }

        /// <summary>
        /// Adds keys to be sorted by in descending order.
        /// </summary>
        /// <param name="memberExpressions">The member expressions indicating which elements to sort by.</param>
        /// <returns>
        /// The builder (so method calls can be chained).
        /// </returns>
        public SortByBuilder<TDocument> Descending(params Expression<Func<TDocument, object>>[] memberExpressions)
        {
            var elementNames = GetElementNames(memberExpressions);
            _sortByBuilder = _sortByBuilder.Descending(elementNames.ToArray());
            return this;
        }

        /// <summary>
        /// Converts this object to a BsonDocument.
        /// </summary>
        /// <returns>
        /// A BsonDocument.
        /// </returns>
        public override BsonDocument ToBsonDocument()
        {
            return _sortByBuilder.ToBsonDocument();
        }

        // private methods
        private IEnumerable<string> GetElementNames(IEnumerable<Expression<Func<TDocument, object>>> memberExpressions)
        {
            return memberExpressions
                .Select(x => _serializationInfoHelper.GetSerializationInfo(x))
                .Select(x => x.ElementName);
        }

        // nested classes
        internal class Serializer : BsonBaseSerializer<SortByBuilder<TDocument>>
        {
            public override void Serialize(SerializationContext context, SortByBuilder<TDocument> value)
            {
                context.SerializeWithChildContext(BsonDocumentSerializer.Instance, value._sortByBuilder.ToBsonDocument());
            }
        }
    }
}
