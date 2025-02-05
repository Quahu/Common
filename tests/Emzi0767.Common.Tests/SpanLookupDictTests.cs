﻿// This file is part of Emzi0767.Common project
//
// Copyright 2019 Emzi0767
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Emzi0767.Common.Tests
{
    [TestClass]
    public sealed class SpanLookupDictTests
    {
        private IEnumerable<(string Key, int Value)> TestValues { get; } = new[] { ("42", 42), ("69", 69) };
        private IEnumerable<(string Key, int Value)> NonTestValues { get; } = new[] { ("144", 144), ("0", 0), ("-3", -3) };

        [TestMethod]
        public void TestInsertionRetrievalViaString()
        {
            var dict = new CharSpanLookupDictionary<int>();

            Assert.AreEqual(0, dict.Count);
            foreach (var (k, v) in this.TestValues)
                Assert.IsFalse(dict.TryGetValue(k, out _));

            foreach (var (k, v) in this.TestValues)
                dict.Add(k, v);

            Assert.AreEqual(this.TestValues.Count(), dict.Count);
            foreach (var (k, v) in this.TestValues)
            {
                Assert.IsTrue(dict.TryGetValue(k, out var rv));
                Assert.AreEqual(v, rv);
            }

            foreach (var (k, v) in this.NonTestValues)
                Assert.IsFalse(dict.TryGetValue(k, out _));

            foreach (var (k, v) in this.TestValues)
                Assert.ThrowsException<ArgumentException>(() => dict.Add(k, v));
            Assert.AreEqual(this.TestValues.Count(), dict.Count);

            foreach (var (k, v) in this.TestValues)
                Assert.IsFalse(dict.TryAdd(k, v));
            Assert.AreEqual(this.TestValues.Count(), dict.Count);

            foreach (var (k, v) in this.TestValues)
                dict[k] = v;
            Assert.AreEqual(this.TestValues.Count(), dict.Count);

            foreach (var (k, v) in this.TestValues)
                Assert.IsTrue(dict.ContainsKey(k));
        }

        [TestMethod]
        public void TestInsertionRetrievalViaSpan()
        {
            var dict = new CharSpanLookupDictionary<int>();

            Assert.AreEqual(0, dict.Count);
            foreach (var (k, v) in this.TestValues)
                Assert.IsFalse(dict.TryGetValue(k.AsSpan(), out _));

            foreach (var (k, v) in this.TestValues)
                dict.Add(k.AsSpan(), v);

            Assert.AreEqual(this.TestValues.Count(), dict.Count);
            foreach (var (k, v) in this.TestValues)
            {
                Assert.IsTrue(dict.TryGetValue(k.AsSpan(), out var rv));
                Assert.AreEqual(v, rv);
            }

            foreach (var (k, v) in this.NonTestValues)
                Assert.IsFalse(dict.TryGetValue(k.AsSpan(), out _));

            foreach (var (k, v) in this.TestValues)
                Assert.ThrowsException<ArgumentException>(() => dict.Add(k.AsSpan(), v));
            Assert.AreEqual(this.TestValues.Count(), dict.Count);

            foreach (var (k, v) in this.TestValues)
                Assert.IsFalse(dict.TryAdd(k.AsSpan(), v));
            Assert.AreEqual(this.TestValues.Count(), dict.Count);

            foreach (var (k, v) in this.TestValues)
                dict[k.AsSpan()] = v;
            Assert.AreEqual(this.TestValues.Count(), dict.Count);

            foreach (var (k, v) in this.TestValues)
                Assert.IsTrue(dict.ContainsKey(k.AsSpan()));
        }

        [TestMethod]
        public void TestCreationFromAnother()
        {
            var baseDict = new Dictionary<string, int>();
            foreach (var (k, v) in this.TestValues)
                baseDict[k] = v;
            Assert.AreEqual(this.TestValues.Count(), baseDict.Count);
            foreach (var (k, v) in this.TestValues)
                Assert.IsTrue(baseDict.ContainsKey(k));
            foreach (var (k, v) in this.NonTestValues)
                Assert.IsFalse(baseDict.ContainsKey(k));

            var dict = new CharSpanLookupDictionary<int>(baseDict as IDictionary<string, int>);
            Assert.AreEqual(this.TestValues.Count(), dict.Count);

            foreach (var (k, v) in this.TestValues)
            {
                Assert.IsTrue(dict.TryGetValue(k, out var rv));
                Assert.AreEqual(v, rv);

                Assert.IsTrue(dict.TryGetValue(k.AsSpan(), out rv));
                Assert.AreEqual(v, rv);
            }

            foreach (var (k, v) in this.NonTestValues)
            {
                Assert.IsFalse(dict.TryGetValue(k, out var rv));
                Assert.IsFalse(dict.TryGetValue(k.AsSpan(), out rv));
            }

            dict = new CharSpanLookupDictionary<int>(baseDict as IReadOnlyDictionary<string, int>);
            Assert.AreEqual(this.TestValues.Count(), dict.Count);

            foreach (var (k, v) in this.TestValues)
            {
                Assert.IsTrue(dict.TryGetValue(k, out var rv));
                Assert.AreEqual(v, rv);

                Assert.IsTrue(dict.TryGetValue(k.AsSpan(), out rv));
                Assert.AreEqual(v, rv);
            }

            foreach (var (k, v) in this.NonTestValues)
            {
                Assert.IsFalse(dict.TryGetValue(k, out var rv));
                Assert.IsFalse(dict.TryGetValue(k.AsSpan(), out rv));
            }

            dict = new CharSpanLookupDictionary<int>(baseDict as IEnumerable<KeyValuePair<string, int>>);
            Assert.AreEqual(this.TestValues.Count(), dict.Count);

            foreach (var (k, v) in this.TestValues)
            {
                Assert.IsTrue(dict.TryGetValue(k, out var rv));
                Assert.AreEqual(v, rv);

                Assert.IsTrue(dict.TryGetValue(k.AsSpan(), out rv));
                Assert.AreEqual(v, rv);
            }

            foreach (var (k, v) in this.NonTestValues)
            {
                Assert.IsFalse(dict.TryGetValue(k, out var rv));
                Assert.IsFalse(dict.TryGetValue(k.AsSpan(), out rv));
            }

            dict = new CharSpanLookupDictionary<int>(this.TestValues.Select(x => new KeyValuePair<string, int>(x.Key, x.Value)));
            Assert.AreEqual(this.TestValues.Count(), dict.Count);

            foreach (var (k, v) in this.TestValues)
            {
                Assert.IsTrue(dict.TryGetValue(k, out var rv));
                Assert.AreEqual(v, rv);

                Assert.IsTrue(dict.TryGetValue(k.AsSpan(), out rv));
                Assert.AreEqual(v, rv);
            }

            foreach (var (k, v) in this.NonTestValues)
            {
                Assert.IsFalse(dict.TryGetValue(k, out var rv));
                Assert.IsFalse(dict.TryGetValue(k.AsSpan(), out rv));
            }
        }

        [TestMethod]
        public void TestRemovalViaString()
        {
            var dict = new CharSpanLookupDictionary<int>(this.TestValues.Select(x => new KeyValuePair<string, int>(x.Key, x.Value)));
            Assert.AreEqual(this.TestValues.Count(), dict.Count);

            var i = 0;
            foreach (var (k, v) in this.TestValues)
            {
                Assert.IsTrue(dict.TryRemove(k, out var rv));
                Assert.AreEqual(v, rv);
                Assert.AreEqual(this.TestValues.Count() - ++i, dict.Count);
                Assert.IsFalse(dict.ContainsKey(k));
            }

            Assert.AreEqual(0, dict.Count);
        }

        [TestMethod]
        public void TestRemovalViaSpan()
        {
            var dict = new CharSpanLookupDictionary<int>(this.TestValues.Select(x => new KeyValuePair<string, int>(x.Key, x.Value)));
            Assert.AreEqual(this.TestValues.Count(), dict.Count);

            var i = 0;
            foreach (var (k, v) in this.TestValues)
            {
                Assert.IsTrue(dict.TryRemove(k.AsSpan(), out var rv));
                Assert.AreEqual(v, rv);
                Assert.AreEqual(this.TestValues.Count() - ++i, dict.Count);
                Assert.IsFalse(dict.ContainsKey(k.AsSpan()));
            }

            Assert.AreEqual(0, dict.Count);
        }

        [TestMethod]
        public void TestClearing()
        {
            var dict = new CharSpanLookupDictionary<int>(this.TestValues.Select(x => new KeyValuePair<string, int>(x.Key, x.Value)));
            Assert.AreEqual(this.TestValues.Count(), dict.Count);

            dict.Clear();
            Assert.AreEqual(0, dict.Count);
        }

        [TestMethod]
        public void TestEnumeration()
        {
            var dict = new CharSpanLookupDictionary<int>(this.TestValues.Select(x => new KeyValuePair<string, int>(x.Key, x.Value)));
            Assert.AreEqual(this.TestValues.Count(), dict.Count);

            var i = 0;
            foreach (var (k, v) in dict)
            {
                Assert.IsTrue(this.TestValues.Any(x => x.Key == k && x.Value == v));
                i++;
            }
            Assert.AreEqual(this.TestValues.Count(), i);

            dict.Clear();
            Assert.AreEqual(0, dict.Count);

            i = 0;
            foreach (var (k, v) in dict)
            {
                Assert.Fail();
                i++;
            }
            Assert.AreEqual(0, i);
        }

        [TestMethod]
        public void TestKeysValues()
        {
            var dict = new CharSpanLookupDictionary<int>(this.TestValues.Select(x => new KeyValuePair<string, int>(x.Key, x.Value)));
            Assert.AreEqual(this.TestValues.Count(), dict.Count);

            var keys = this.TestValues.Select(x => x.Key).ToImmutableArray();
            var vals = this.TestValues.Select(x => x.Value).ToImmutableArray();

            var i = 0;
            var dkeys = dict.Keys;
            Assert.IsNotNull(dkeys);
            Assert.AreEqual(dict.Count, dkeys.Count());
            foreach (var k in dkeys)
            {
                Assert.IsTrue(keys.Contains(k));
                i++;
            }
            Assert.AreEqual(dict.Count, i);

            i = 0;
            var dvals = dict.Values;
            Assert.IsNotNull(dvals);
            Assert.AreEqual(dict.Count, dvals.Count());
            foreach (var v in dvals)
            {
                Assert.IsTrue(vals.Contains(v));
                i++;
            }
            Assert.AreEqual(dict.Count, i);
        }
    }
}
