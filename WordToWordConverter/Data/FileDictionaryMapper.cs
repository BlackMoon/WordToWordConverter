using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WordToWordConverter.Data
{
    /// <summary>
    /// 
    /// </summary>
    internal class FileDictionaryMapper : IDictionaryMapper<int, WordItem>
    {
        private readonly string _fileName;

        public List<WordItem> Items = new List<WordItem>(); 

        public IEnumerable<WordItem> GetAll()
        {
            throw new NotImplementedException();
        }

        public WordItem Get(int key)
        {
            throw new NotImplementedException();
        }

        public FileDictionaryMapper(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("fileName");

            _fileName = fileName;
        }

        public Task Load()
        {
            return Task.Run(() =>
            {
                string[] lines = System.IO.File.ReadAllLines(_fileName);

                int len = lines.Length;

                Items = new List<WordItem>(len);
                for (int i = 0; i < len; i++)
                {
                    Items.Add(new WordItem(i, lines[i]));
                }                    
            });
        }
    }
}
