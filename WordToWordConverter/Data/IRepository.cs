using System.Collections.Generic;

namespace WordToWordConverter.Data
{
    public interface IRepository<in TK, out T> where T : class
    {
        IEnumerable<T> GetAll();

        T Get(TK key);
    }

}