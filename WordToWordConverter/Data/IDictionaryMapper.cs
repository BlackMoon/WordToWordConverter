using System.Collections.Generic;
using System.Threading.Tasks;

namespace WordToWordConverter.Data
{
    /// <summary>
    /// Интрефейс словаря
    /// </summary>
    public interface IDictionaryMapper : IRepository<int, WordItem> 
    {
        int? GetKey(string value);

        IDictionary<int, int> FindMutationVariants(string wordFrom, string wordTo, int wordLen, int disabledMutationPos, IList<int> excludedWordKeys);

        Task Load();
    }
}