using System.Threading.Tasks;

namespace WordToWordConverter.Data
{
    public interface IDictionaryMapper<in TK, out T> : IRepository<TK, T> where TK : struct
    {
        Task Load();
    }
}