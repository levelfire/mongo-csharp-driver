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
using System.Globalization;
using System.IO;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace MongoDB.Bson.Serialization.Serializers
{
    /// <summary>
    /// Represents a serializer for Bytes.
    /// </summary>
    public class ByteSerializer : BsonBaseSerializer<byte>, IBsonSerializerWithRepresentation<ByteSerializer>
    {
        // private fields
        private readonly BsonType _representation;

        // constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ByteSerializer"/> class.
        /// </summary>
        public ByteSerializer()
            : this(BsonType.Int32)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByteSerializer"/> class.
        /// </summary>
        /// <param name="representation">The representation.</param>
        public ByteSerializer(BsonType representation)
        {
            _representation = representation;
        }

        // public properties
        public BsonType Representation
        {
            get { return _representation; }
        }

        // public methods
        /// <summary>
        /// Deserializes an object from a BsonReader.
        /// </summary>
        /// <param name="bsonReader">The BsonReader.</param>
        /// <param name="actualType">The actual type of the object.</param>
        /// <returns>An object.</returns>
        public override byte Deserialize(DeserializationContext context)
        {
            var bsonReader = context.Reader;
            byte value;
            var lostData = false;

            var bsonType = bsonReader.GetCurrentBsonType();
            switch (bsonType)
            {
                case BsonType.Binary:
                    var bytes = bsonReader.ReadBytes();
                    if (bytes.Length != 1)
                    {
                        throw new FileFormatException("Binary data for Byte must be exactly one byte long.");
                    }
                    value = bytes[0];
                    break;

                case BsonType.Int32:
                    var int32Value = bsonReader.ReadInt32();
                    value = (byte)int32Value;
                    lostData = (int)value != int32Value;
                    break;

                case BsonType.Int64:
                    var int64Value = bsonReader.ReadInt64();
                    value = (byte)int64Value;
                    lostData = (int)value != int64Value;
                    break;

                case BsonType.String:
                    var s = bsonReader.ReadString();
                    if (s.Length == 1)
                    {
                        s = "0" + s;
                    }
                    value = byte.Parse(s, NumberStyles.HexNumber);
                    break;

                default:
                    var message = string.Format("Cannot deserialize Byte from BsonType {0}.", bsonType);
                    throw new FileFormatException(message);
            }

            if (lostData)
            {
                var message = string.Format("Data loss occurred when trying to convert from {0} to Byte.", bsonType);
                throw new FileFormatException(message);
            }

            return value;
        }

        /// <summary>
        /// Serializes an object to a BsonWriter.
        /// </summary>
        /// <param name="bsonWriter">The BsonWriter.</param>
        /// <param name="value">The object.</param>
        public override void Serialize(SerializationContext context, byte value)
        {
            var bsonWriter = context.Writer;

            switch (_representation)
            {
                case BsonType.Binary:
                    bsonWriter.WriteBytes(new byte[] { value });
                    break;

                case BsonType.Int32:
                    bsonWriter.WriteInt32(value);
                    break;

                case BsonType.Int64:
                    bsonWriter.WriteInt64(value);
                    break;

                case BsonType.String:
                    bsonWriter.WriteString(string.Format("{0:x2}", value));
                    break;

                default:
                    var message = string.Format("'{0}' is not a valid Byte representation.", _representation);
                    throw new BsonSerializationException(message);
            }
        }

        public ByteSerializer WithRepresentation(BsonType representation)
        {
            if (representation == _representation)
            {
                return this;
            }
            else
            {
                return new ByteSerializer(representation);
            }
        }

        // explicit interface implementations
        IBsonSerializer IBsonSerializerWithRepresentation.WithRepresentation(BsonType representation)
        {
            return WithRepresentation(representation);
        }
    }
}
