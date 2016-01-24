using System.Collections.Generic;

namespace WordToWordConverter.Data
{
    /// <summary>
    /// Интерфейс - репозиторий
    /// </summary>
    /// <typeparam name="TK">тип ключа</typeparam>
    /// <typeparam name="T">тип элемента</typeparam>
    public interface IRepository<in TK, out T> where TK : struct
    {
        IEnumerable<T> GetAll();

        T Get(TK key);
    }

}