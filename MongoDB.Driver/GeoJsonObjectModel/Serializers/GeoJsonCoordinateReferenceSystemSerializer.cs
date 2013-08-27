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
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace MongoDB.Driver.GeoJsonObjectModel.Serializers
{
    /// <summary>
    /// Represents a serializer for a GeoJsonCoordinateReferenceSystem value.
    /// </summary>
    public class GeoJsonCoordinateReferenceSystemSerializer : BsonBaseSerializer<GeoJsonCoordinateReferenceSystem>
    {
        // public methods
        /// <summary>
        /// Deserializes an object from a BsonReader.
        /// </summary>
        /// <param name="bsonReader">The BsonReader.</param>
        /// <returns>
        /// An object.
        /// </returns>
        public override GeoJsonCoordinateReferenceSystem Deserialize(DeserializationContext context)
        {
            var bsonReader = context.Reader;

            if (bsonReader.GetCurrentBsonType() == BsonType.Null)
            {
                bsonReader.ReadNull();
                return null;
            }
            else
            {
                var actualType = GetActualType(bsonReader);
                var serializer = BsonSerializer.LookupSerializer(actualType);
                return (GeoJsonCoordinateReferenceSystem)serializer.Deserialize(context);
            }
        }

        /// <summary>
        /// Serializes an object to a BsonWriter.
        /// </summary>
        /// <param name="bsonWriter">The BsonWriter.</param>
        /// <param name="value">The object.</param>
        public override void Serialize(SerializationContext context, GeoJsonCoordinateReferenceSystem value)
        {
            var bsonWriter = context.Writer;

            if (value == null)
            {
                bsonWriter.WriteNull();
            }
            else
            {
                var actualType = value.GetType();
                var serializer = BsonSerializer.LookupSerializer(actualType);
                serializer.Serialize(context, value);
            }
        }

        // private methods
        private Type GetActualType(BsonReader bsonReader)
        {
            var bookmark = bsonReader.GetBookmark();
            bsonReader.ReadStartDocument();
            if (bsonReader.FindElement("type"))
            {
                var type = bsonReader.ReadString();
                bsonReader.ReturnToBookmark(bookmark);

                switch (type)
                {
                    case "link": return typeof(GeoJsonLinkedCoordinateReferenceSystem);
                    case "name": return typeof(GeoJsonNamedCoordinateReferenceSystem);
                    default:
                        var message = string.Format("The type field of the GeoJsonCoordinateReferenceSystem is not valid: '{0}'.", type);
                        throw new FormatException(message);
                }
            }
            else
            {
                throw new FormatException("GeoJsonCoordinateReferenceSystem object is missing the type field.");
            }
        }
    }
}
