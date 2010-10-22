﻿/* Copyright 2010 10gen Inc.
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
using MongoDB.Bson.DefaultSerializer;
using MongoDB.Driver;

namespace MongoDB.DriverOnlineTests.Jira.CSharp77 {
    public class Foo {
        // [BsonId] // no longer required since we have a slightly better Id detection algorithm
        public ObjectId _id { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
    }

    [TestFixture]
    public class CSharp77Tests {
        [Test]
        public void TestSave() {
            var server = MongoServer.Create();
            var database = server["onlinetests"];
            var collection = database.GetCollection<Foo>("csharp77");

            collection.RemoveAll();
            for (int i = 0; i < 10; i++) {
                var foo = new Foo {
                    _id = ObjectId.Empty,
                    Name = string.Format("Foo-{0}", i),
                    Summary = string.Format("Summary for Foo-{0}", i)
                };
                collection.Save(foo, SafeMode.True);
                var count = collection.Count();
                Assert.AreEqual(i + 1, count);
            }
        }
    }
}