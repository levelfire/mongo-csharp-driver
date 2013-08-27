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
using MongoDB.Bson.Serialization;
using NUnit.Framework;

namespace MongoDB.BsonUnitTests
{
    [TestFixture]
    public class BsonDocumentWrapperTests
    {
        private class C
        {
            public int X { get; set; }
        }

        private C _c = new C { X = 1 };
        private IBsonSerializer<C> _serializer = BsonSerializer.LookupSerializer<C>();

        [Test]
        public void TestConstructorWithObject()
        {
            var wrapper = new BsonDocumentWrapper(_c);
            var expected = "{ \"X\" : 1 }";
            Assert.AreEqual(expected, wrapper.ToJson());
        }

        [Test]
        public void TestConstructorWithNullObject()
        {
            var wrapper = new BsonDocumentWrapper(null);
            var expected = "null";
            Assert.AreEqual(expected, wrapper.ToJson());
        }

        [Test]
        public void TestConstructorWithSerializerAndObject()
        {
            var wrapper = new BsonDocumentWrapper(_c, _serializer);
            var expected = "{ \"X\" : 1 }";
            Assert.AreEqual(expected, wrapper.ToJson());
        }

        [Test]
        public void TestConstructorWithSerializerAndNullObject()
        {
            var wrapper = new BsonDocumentWrapper(null, _serializer);
            var expected = "null";
            Assert.AreEqual(expected, wrapper.ToJson());
        }

        [Test]
        public void TestConstructorWithNullSerializerAndObject()
        {
            Assert.Throws<ArgumentNullException>(() => { var wrapper = new BsonDocumentWrapper(_c, null); });
        }


        [Test]
        public void TestConstructorWithSerializerAndObjectAndIsUpdateDocument()
        {
            var wrapper = new BsonDocumentWrapper(_c, _serializer, false);
            var expected = "{ \"X\" : 1 }";
            Assert.AreEqual(expected, wrapper.ToJson());
        }

        [Test]
        public void TestConstructorWithSerializerAndNullObjectAndIsUpdateDocument()
        {
            var wrapper = new BsonDocumentWrapper(null, _serializer, false);
            var expected = "null";
            Assert.AreEqual(expected, wrapper.ToJson());
        }

        [Test]
        public void TestConstructorWithNullSerializerAndObjectAndIsUpdateDocument()
        {
            Assert.Throws<ArgumentNullException>(() => { var wrapper = new BsonDocumentWrapper(_c, null, false); });
        }

        [Test]
        public void TestCreateWithValue()
        {
            var wrapper = BsonDocumentWrapper.Create(_c);
            var expected = "{ \"X\" : 1 }";
            Assert.AreEqual(expected, wrapper.ToJson());
        }

        [Test]
        public void TestCreateWithNullValue()
        {
            var wrapper = BsonDocumentWrapper.Create((C)null);
            var expected = "null";
            Assert.AreEqual(expected, wrapper.ToJson());
        }

        [Test]
        public void TestCreateGenericWithValueAndIsUpdateDocument()
        {
            var wrapper = BsonDocumentWrapper.Create<C>(_c, false);
            var expected = "{ \"X\" : 1 }";
            Assert.AreEqual(expected, wrapper.ToJson());
        }

        [Test]
        public void TestCreateGenericWithNullValueAndIsUpdateDocument()
        {
            var wrapper = BsonDocumentWrapper.Create<C>(null, false);
            var expected = "null";
            Assert.AreEqual(expected, wrapper.ToJson());
        }

        [Test]
        public void TestCreateWithNominalTypeAndValue()
        {
            var wrapper = BsonDocumentWrapper.Create(typeof(C), _c);
            var expected = "{ \"X\" : 1 }";
            Assert.AreEqual(expected, wrapper.ToJson());
        }

        [Test]
        public void TestCreateWithNominalTypeAndNullValue()
        {
            var wrapper = BsonDocumentWrapper.Create(typeof(C), null);
            var expected = "null";
            Assert.AreEqual(expected, wrapper.ToJson());
        }

        [Test]
        public void TestCreateWithNullNominalTypeAndValue()
        {
            Assert.Throws<ArgumentNullException>(() => { var wrapper = BsonDocumentWrapper.Create(null, _c); });
        }

        [Test]
        public void TestCreateWithNominalTypeAndValueAndIsUpdateDocument()
        {
            var wrapper = BsonDocumentWrapper.Create(typeof(C), _c, false);
            var expected = "{ \"X\" : 1 }";
            Assert.AreEqual(expected, wrapper.ToJson());
        }

        [Test]
        public void TestCreateWithNominalTypeAndNullValueAndIsUpdateDocument()
        {
            var wrapper = BsonDocumentWrapper.Create(typeof(C), null, false);
            var expected = "null";
            Assert.AreEqual(expected, wrapper.ToJson());
        }

        [Test]
        public void TestCreateWithNullNominalTypeAndValueAndIsUpdateDocument()
        {
            Assert.Throws<ArgumentNullException>(() => { var wrapper = BsonDocumentWrapper.Create(null, _c, false); });
        }

        [Test]
        public void TestCreateMultipleGenericWithValues()
        {
            var wrappers = BsonDocumentWrapper.CreateMultiple<C>(new C[] { _c, null });
            var expected = "[{ \"X\" : 1 }, null]";
            Assert.AreEqual(expected, wrappers.ToJson());
        }

        [Test]
        public void TestCreateMultipleGenericWithNullValues()
        {
            Assert.Throws<ArgumentNullException>(() => { var wrappers = BsonDocumentWrapper.CreateMultiple<C>(null); });
        }

        [Test]
        public void TestCreateMultipleWithNominalTypeAndValues()
        {
            var wrappers = BsonDocumentWrapper.CreateMultiple(typeof(C), new C[] { _c, null });
            var expected = "[{ \"X\" : 1 }, null]";
            Assert.AreEqual(expected, wrappers.ToJson());
        }

        [Test]
        public void TestCreateMultipleWithNullNominalTypeAndValues()
        {
            Assert.Throws<ArgumentNullException>(() => { var wrappers = BsonDocumentWrapper.CreateMultiple(null, new C[] { _c, null }); });
        }

        [Test]
        public void TestCreateMultipleWithNominalTypeAndNullValues()
        {
            Assert.Throws<ArgumentNullException>(() => { var wrappers = BsonDocumentWrapper.CreateMultiple(typeof(C), null); });
        }
    }
}
