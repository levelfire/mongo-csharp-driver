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
using System.IO;
using System.Xml;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace MongoDB.Bson.Serialization.Serializers
{
    /// <summary>
    /// Represents a serializer for UInt32s.
    /// </summary>
    public class UInt32Serializer : BsonBaseSerializer<uint>, IBsonSerializerWithRepresentation<UInt32Serializer>, IBsonSerializerWithRepresentationConverter<UInt32Serializer>
    {
        // private fields
        private readonly BsonType _representation;
        private readonly RepresentationConverter _converter;

        // constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="UInt32Serializer"/> class.
        /// </summary>
        public UInt32Serializer()
            : this(BsonType.Int32)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UInt32Serializer"/> class.
        /// </summary>
        /// <param name="representation">The representation.</param>
        public UInt32Serializer(BsonType representation)
            : this(representation, new RepresentationConverter(false, false))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UInt32Serializer"/> class.
        /// </summary>
        /// <param name="representation">The representation.</param>
        /// <param name="converter">The converter.</param>
        public UInt32Serializer(BsonType representation, RepresentationConverter converter)
        {
            _representation = representation;
            _converter = converter;
        }

        // public properties
        public RepresentationConverter Converter
        {
            get { return _converter; }
        }

        public BsonType Representation
        {
            get { return _representation; }
        }

        // public methods
        /// <summary>
        /// Deserializes an object from a BsonReader.
        /// </summary>
        /// <param name="bsonReader">The BsonReader.</param>
        /// <returns>An object.</returns>
        public override uint Deserialize(DeserializationContext context)
        {
            var bsonReader = context.Reader;

            var bsonType = bsonReader.GetCurrentBsonType();
            switch (bsonType)
            {
                case BsonType.Double:
                    return _converter.ToUInt32(bsonReader.ReadDouble());

                case BsonType.Int32:
                    return _converter.ToUInt32(bsonReader.ReadInt32());

                case BsonType.Int64:
                    return _converter.ToUInt32(bsonReader.ReadInt64());

                case BsonType.String:
                    return XmlConvert.ToUInt32(bsonReader.ReadString());

                default:
                    var message = string.Format("Cannot deserialize UInt32 from BsonType {0}.", bsonType);
                    throw new FileFormatException(message);
            }
        }

        /// <summary>
        /// Serializes an object to a BsonWriter.
        /// </summary>
        /// <param name="bsonWriter">The BsonWriter.</param>
        /// <param name="value">The object.</param>
        public override void Serialize(SerializationContext context, uint value)
        {
            var bsonWriter = context.Writer;

            switch (_representation)
            {
                case BsonType.Double:
                    bsonWriter.WriteDouble(_converter.ToDouble(value));
                    break;

                case BsonType.Int32:
                    bsonWriter.WriteInt32(_converter.ToInt32(value));
                    break;

                case BsonType.Int64:
                    bsonWriter.WriteInt64(_converter.ToInt64(value));
                    break;

                case BsonType.String:
                    bsonWriter.WriteString(XmlConvert.ToString(value));
                    break;

                default:
                    var message = string.Format("'{0}' is not a valid UInt32 representation.", _representation);
                    throw new BsonSerializationException(message);
            }
        }

        public UInt32Serializer WithConverter(RepresentationConverter converter)
        {
            if (converter == _converter)
            {
                return this;
            }
            else
            {
                return new UInt32Serializer(_representation, converter);
            }
        }

        public UInt32Serializer WithRepresentation(BsonType representation)
        {
            if (representation == _representation)
            {
                return this;
            }
            else
            {
                return new UInt32Serializer(representation, _converter);
            }
        }

        // explicit interface implementations
        IBsonSerializer IBsonSerializerWithRepresentationConverter.WithConverter(RepresentationConverter converter)
        {
            return WithConverter(converter);
        }

        IBsonSerializer IBsonSerializerWithRepresentation.WithRepresentation(BsonType representation)
        {
            return WithRepresentation(representation);
        }
    }
}
