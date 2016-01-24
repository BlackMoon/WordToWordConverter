using System;
using System.Collections.Generic;

namespace WordToWordConverter.Data
{
    internal class FileDictionary : IRepository<int, WordItem>
    {
        public IEnumerable<WordItem> GetAll()
        {
            throw new NotImplementedException();
        }

        public WordItem Get(int key)
        {
            throw new NotImplementedException();
        }
    }
}
