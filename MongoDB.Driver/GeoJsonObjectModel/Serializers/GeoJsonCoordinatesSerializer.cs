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
    /// Represents a serializer for a GeoJsonCoordinates value.
    /// </summary>
    public class GeoJsonCoordinatesSerializer : BsonBaseSerializer<GeoJsonCoordinates>
    {
        // public methods
        /// <summary>
        /// Deserializes an object from a BsonReader.
        /// </summary>
        /// <param name="bsonReader">The BsonReader.</param>
        /// <returns>
        /// An object.
        /// </returns>
        /// <exception cref="System.FormatException">Actual type of GeoJsonCoordinates must be provided explicitly.</exception>
        public override GeoJsonCoordinates Deserialize(DeserializationContext context)
        {
            throw new InvalidOperationException("Only concrete subclasses of GeoJsonCoordinates can be serialized.");
        }

        /// <summary>
        /// Serializes an object to a BsonWriter.
        /// </summary>
        /// <param name="bsonWriter">The BsonWriter.</param>
        /// <param name="value">The object.</param>
        /// <exception cref="System.InvalidOperationException">Only concrete subclasses of GeoJsonCoordinates can be serialized.</exception>
        public override void Serialize(SerializationContext context, GeoJsonCoordinates value)
        {
            throw new InvalidOperationException("Only concrete subclasses of GeoJsonCoordinates can be serialized.");
        }
    }
}
