using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WordToWordConverter.Data;

namespace WordToWordConverter.Tests
{
    [TestClass]
    public class DictionaryMapperTest
    {
        private IDictionaryMapper _dictionaryMapper;
        private Task _task;
        [TestInitialize]
        public void DictionaryMapperTestInit()
        {
            _dictionaryMapper = new FileDictionaryMapper()
            {
                FileName = Path.GetFullPath(@"..\..\..\WordTowordConverter\dictionary.txt"),
                NeedSort = false
            };

            _task = _dictionaryMapper.Load();
        }

        [TestMethod]
        public void TestGet()
        {
            _task.Wait();

            WordItem item = _dictionaryMapper.Get(10);
            Assert.IsNotNull(item);

            Assert.AreEqual(item.Word, "абаша");
        }

        [TestMethod]
        public void TestGetKey()
        {
            _task.Wait();

            int? key = _dictionaryMapper.GetKey("абзац");
            Assert.IsNotNull(key);

            Assert.AreEqual(key, 21);
        }

        [TestMethod]
        public void TestFindMutationVariants()
        {
            IDictionary<int, int> expected = new Dictionary<int, int>()
            {
               { 5113, 1 },
               { 5427, 1 },
               { 5499, 3 },
               { 5448, 2 },
               { 5454, 2 },
               { 5469, 2 },
               { 5479, 2 },
               { 5485, 2 },
               { 5493, 2 },
               { 5502, 3 }
            };

            _task.Wait();

            IDictionary<int, int> variants = _dictionaryMapper.FindMutationVariants("муха", "слон", 4, 0, new List<int>());

            CollectionAssert.AreEqual((ICollection) variants.Keys, (ICollection) expected.Keys);

            CollectionAssert.AreEqual((ICollection)variants.Values, (ICollection)expected.Values);
        }
    }
}
