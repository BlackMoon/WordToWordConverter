
using System.Collections.Generic;
using System.Linq;
using WordToWordConverter.Data;

namespace WordToWordConverter.Converter
{
    public class WordConverter : IConverter
    {
        /// <summary>
        /// частота употребления букв (взято с Wiki)
        /// </summary>
        private IDictionary<char, float> _letterFrequences = new Dictionary<char, float>(33)
        {
            { 'о', 0.10983f },
			{ 'е', 0.08483f },
			{ 'а', 0.07998f },
			{ 'и', 0.07367f },
			{ 'н', 0.06700f },
			{ 'т', 0.06318f },
			{ 'с', 0.05473f },
			{ 'р', 0.04746f },
			{ 'в', 0.04533f },
			{ 'л', 0.04343f },
			{ 'к', 0.03486f },
			{ 'м', 0.03203f },
			{ 'д', 0.02977f },
			{ 'п', 0.02804f },
			{ 'у', 0.02615f },
			{ 'я', 0.02001f },
			{ 'ы', 0.01898f },
			{ 'ь', 0.01735f },
			{ 'г', 0.01687f },
			{ 'з', 0.01641f },
			{ 'б', 0.01592f },
			{ 'ч', 0.01450f },
			{ 'й', 0.01208f },
			{ 'х', 0.00966f },
			{ 'ж', 0.00940f },
			{ 'ш', 0.00718f },
			{ 'ю', 0.00639f },
			{ 'ц', 0.00486f },
			{ 'щ', 0.00361f },
			{ 'э', 0.00331f },
			{ 'ф', 0.00267f },
			{ 'ъ' , 0.00037f },
			{ 'ё' , 0.00013f }
        };

        private readonly IDictionary<char, float> _letterFrequencesConsonant = new Dictionary<char, float>(23);
        private readonly IDictionary<char, float> _letterFrequencesVovels = new Dictionary<char, float>(10);

        public IDictionaryMapper<int, WordItem> DictionaryMapper { get; set; }

        public WordConverter()
        {
            foreach (KeyValuePair<char, float> kvp in _letterFrequences)
            {
                if (IsVovel(kvp.Key))
                    _letterFrequencesVovels[kvp.Key] = kvp.Value;
                else
                    _letterFrequencesConsonant[kvp.Key] = kvp.Value;
            }
            
            // Нормируем.
            // Массивы частот упорядочены, потому поиск не требуется
            float maxFrequency = _letterFrequencesVovels.First().Value;
            _letterFrequencesVovels = _letterFrequencesVovels.ToDictionary(kvp => kvp.Key, kvp => kvp.Value/maxFrequency);
            
            maxFrequency = _letterFrequencesConsonant.First().Value;
            _letterFrequencesConsonant = _letterFrequencesConsonant.ToDictionary(kvp => kvp.Key, kvp => kvp.Value / maxFrequency);
        }

        /// <summary>
        /// Найти цепочку преобразований
        /// </summary>
        /// <param name="wordFrom">Начальное слово</param>
        /// <param name="wordTo">Конечное слово</param>
        /// <param name="maxSteps">Макс. кол-во шагов</param>
        /// <param name="maxPopulation">Макс. размер популяции</param>
        /// <returns></returns>
        public IEnumerable<string> FindMutationChain(string wordFrom, string wordTo, int maxSteps = 100, int maxPopulation = 50)
        {
            return new string[] {"sss", "dd"};
        }

        public bool IsVovel(char letter)
        {
            return "aeiouAEIOUаеёиоуыэюяАЕЁИОУЫЭЮЯ".IndexOf(letter) >= 0;
        }
    }
}
