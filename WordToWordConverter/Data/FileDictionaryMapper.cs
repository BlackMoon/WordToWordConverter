using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WordToWordConverter.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class FileDictionaryMapper : IDictionaryMapper<int, WordItem>
    {
        public string FileName { get; set; }

        private readonly List<WordItem> _items = new List<WordItem>();

        public IEnumerable<WordItem> GetAll()
        {
            throw new NotImplementedException();
        }

        public WordItem Get(int key)
        {
            return _items.Find(w => w.Id == key);
        }

        public Task Load()
        {
            _items.Clear();

            return Task.Run(() =>
            {
                if (!string.IsNullOrEmpty(FileName))
                {
                    string[] lines = System.IO.File.ReadAllLines(FileName);

                    int len = lines.Length;

                    _items.Capacity = len;
                    for (int i = 0; i < len; i++)
                    {
                        _items.Add(new WordItem(i, lines[i]));
                    }
                }
            });
        }
    }
}
