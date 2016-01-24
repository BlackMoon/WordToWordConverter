using System.Collections.Generic;
using WordToWordConverter.Data;

namespace WordToWordConverter.Converter
{
    public interface IConverter
    {
        IDictionaryMapper DictionaryMapper { get; set; }

        IEnumerable<string> FindMutationChain(string wordFrom, string wordTo, int maxSteps, int maxPopulation);
    }
}