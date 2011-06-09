﻿/* Copyright 2010-2011 10gen Inc.
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
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace MongoDB.DriverOnlineTests.Jira.CSharp199
{
    [TestFixture]
    public class CSharp199Tests
    {

        [Test]
        public void TestSingleRename()
        {
            var server = MongoServer.Create("mongodb://localhost/?safe=true");
            var database = server["onlinetests"];
            var collection = database.GetCollection("CSharp199");
            collection.RemoveAll();

            var testDoc = new BsonDocument
            {
                { "a", 1 }
            };  

            var result = collection.Insert(testDoc);
            var renameDoc = Update.Rename("a", "b");
            var expectedDoc = new BsonDocument {
                { "$rename" , new BsonDocument { { "a", "b"} }}
            };
            Assert.AreEqual(expectedDoc.ToJson(), renameDoc.ToJson());

            var query = Query.EQ("a", 1);
            collection.Update(query, renameDoc);
        }

        [Test]
        public void TestMultipleRenames()
        {
            var server = MongoServer.Create("mongodb://localhost/?safe=true");
            var database = server["onlinetests"];
            var collection = database.GetCollection("CSharp199");
            collection.RemoveAll();

            var testDoc = new BsonDocument
            {
                { "a", 1 },
                { "b", 2}
            };

            var result = collection.Insert(testDoc);
            var renameDoc = Update.Rename("a", "x").Rename("b", "y");
            var expectedDoc = new BsonDocument {
                { "$rename" , new BsonDocument { 
                    { "a", "x" },
                    { "b", "y" }}}
            };
            Assert.AreEqual(expectedDoc.ToJson(), renameDoc.ToJson());

            var query = Query.EQ("a", 1);
            collection.Update(query, renameDoc);
        }

        [Test]
        public void TestRenameWithSet()
        {
            var server = MongoServer.Create("mongodb://localhost/?safe=true");
            var database = server["onlinetests"];
            var collection = database.GetCollection("CSharp199");
            collection.RemoveAll();

            var testDoc = new BsonDocument
            {
                { "a", 1 },
                { "x", 1 }
            };

            var result = collection.Insert(testDoc);
            var renameDoc = Update.Rename("a", "b").Set("x", 2);
            var expectedDoc = new BsonDocument {
                { "$rename" , new BsonDocument { {"a", "b"} }},
                { "$set", new BsonDocument { {"x", 2} }}
            };
            Assert.AreEqual(expectedDoc.ToJson(), renameDoc.ToJson());

            var query = Query.EQ("a", 1);
            collection.Update(query, renameDoc);
        }

    }
}