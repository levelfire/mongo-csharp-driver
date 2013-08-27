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
    /// Represents a serializer for a GeoJson3DGeographicCoordinates value.
    /// </summary>
    public class GeoJson3DGeographicCoordinatesSerializer : BsonBaseSerializer<GeoJson3DGeographicCoordinates>
    {
        // public methods
        /// <summary>
        /// Deserializes an object from a BsonReader.
        /// </summary>
        /// <param name="bsonReader">The BsonReader.</param>
        /// <returns>
        /// An object.
        /// </returns>
        public override GeoJson3DGeographicCoordinates Deserialize(DeserializationContext context)
        {
            var bsonReader = context.Reader;

            if (bsonReader.GetCurrentBsonType() == BsonType.Null)
            {
                bsonReader.ReadNull();
                return null;
            }
            else
            {
                bsonReader.ReadStartArray();
                var longitude = bsonReader.ReadDouble();
                var latitude = bsonReader.ReadDouble();
                var altitude = bsonReader.ReadDouble();
                bsonReader.ReadEndArray();

                return new GeoJson3DGeographicCoordinates(longitude, latitude, altitude);
            }
        }

        /// <summary>
        /// Serializes an object to a BsonWriter.
        /// </summary>
        /// <param name="bsonWriter">The BsonWriter.</param>
        /// <param name="value">The object.</param>
        public override void Serialize(SerializationContext context, GeoJson3DGeographicCoordinates value)
        {
            var bsonWriter = context.Writer;

            if (value == null)
            {
                bsonWriter.WriteNull();
            }
            else
            {
                bsonWriter.WriteStartArray();
                bsonWriter.WriteDouble(value.Longitude);
                bsonWriter.WriteDouble(value.Latitude);
                bsonWriter.WriteDouble(value.Altitude);
                bsonWriter.WriteEndArray();
            }
        }
    }
}
