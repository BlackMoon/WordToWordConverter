using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WordToWordConverter.Converter;
using WordToWordConverter.Data;

namespace WordToWordConverter.Tests
{
    [TestClass]
    public class WordConverterTest
    {
        private IDictionaryMapper _dictionaryMapper;
        private Task _task;

        [TestInitialize]
        public void WordConverterTestInit()
        {
            _dictionaryMapper = new FileDictionaryMapper()
            {
                FileName = Path.GetFullPath(@"..\..\..\WordTowordConverter\dictionary.txt"),
                NeedSort = false
            };

            _task = _dictionaryMapper.Load();
        }

        [TestMethod]
        public void TestFindMutationChain()
        {
            IEnumerable<string> expected = new List<string>()
            {
                "муха",
                "муна",
                "мана",
                "манн",
                "ланн",
                "линн",
                "лион",
                "сион",
                "слон"
            };

            _task.Wait();

            IConverter converter = new WordConverter() { DictionaryMapper = _dictionaryMapper };
            IEnumerable<string> chain = converter.FindMutationChain("муха", "слон", 100, 100);
        }
    }
}
