
using System;
using System.Collections.Generic;
using System.Linq;
using WordToWordConverter.Data;

namespace WordToWordConverter.Converter
{
    public class WordConverter : IConverter
    {
        #region Для поиска внутри [GetWordScope] - с одним и тем же эталонным словом, - используем кэширование
        private int _wordLen;
        private string _cachedComparationWord;
        private readonly IList<Tuple<char, bool>> _cwLetters = new List<Tuple<char, bool>>();
        #endregion

        /// <summary>
        /// частота употребления букв (взято с Wiki)
        /// </summary>
        private readonly IDictionary<char, float> _letterFrequences = new Dictionary<char, float>(33)
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

        private readonly char[] _vovels = { 'а', 'о', 'у', 'ы', 'э', 'я', 'ё', 'ю', 'и', 'е' };

        private readonly IDictionary<char, float> _letterFrequencesConsonants = new Dictionary<char, float>(23);
        private readonly IDictionary<char, float> _letterFrequencesVovels = new Dictionary<char, float>(10);

        public IDictionaryMapper DictionaryMapper { get; set; }

        public WordConverter()
        {

            foreach (KeyValuePair<char, float> kvp in _letterFrequences)
            {
                if (IsVovel(kvp.Key))
                    _letterFrequencesVovels[kvp.Key] = kvp.Value;
                else
                    _letterFrequencesConsonants[kvp.Key] = kvp.Value;
            }
            
            // Нормируем.
            // Массивы частот упорядочены, потому поиск не требуется
            float maxFrequency = _letterFrequencesVovels.First().Value;
            _letterFrequencesVovels = _letterFrequencesVovels.ToDictionary(kvp => kvp.Key, kvp => kvp.Value/maxFrequency);
            
            maxFrequency = _letterFrequencesConsonants.First().Value;
            _letterFrequencesConsonants = _letterFrequencesConsonants.ToDictionary(kvp => kvp.Key, kvp => kvp.Value / maxFrequency);
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
            if (string.IsNullOrEmpty(wordFrom))
                throw new ArgumentException(wordFrom);

            if (string.IsNullOrEmpty(wordTo))
                throw new ArgumentException(wordTo);

            // Принудительно приводим к нижнему регистру входные слова
            wordFrom = wordFrom.ToLower();
            wordTo = wordTo.ToLower();

            // Для идентичных слов ответ очевиден и не требует поиска
		    if (wordFrom == wordTo) 
			    return new [] { wordTo };

            int fromLength = wordFrom.Length,
                toLength = wordTo.Length;

            if (fromLength != toLength)
                throw new Exception("Слова должны быть одинаковой длины.");

            // Существование первого слова в словаре для алгоритма не обязательно
            int? wordFromId = DictionaryMapper.GetKey(wordFrom);

            // Но для второго слова, для простоты, будем это требовать
            int? wordToId = DictionaryMapper.GetKey(wordTo);
            if (!wordToId.HasValue) 
                throw new NullReferenceException("Конечное слово " + wordTo + " не обнаружено в словаре.");
            
            Chain startChain  = new Chain();
            startChain.Keys.Add(wordFromId ?? -1);
            startChain.Positions.Add(-1);
            startChain.Score = 0;

            List<Chain> mutationChains = new List<Chain>() { startChain };
            IList<int> resultKeysChain = new List<int>();

            // Главный цикл генетического алгоритма поиска
            int step = 0;
            
    		for (; step < maxSteps; step++)
    		{
    		    bool found = false;
                // Не дошли ли до искомого слова?
    			foreach (Chain chain in mutationChains) 
                {
    				if (chain.Keys.Last() == wordToId) {
	    				// Найдена одна из кратчайших (для этого забега) цепочек
		    			resultKeysChain = chain.Keys;
    				    found = true;
			    		break;
				    }
			    }
                
                // Выращиваем следующее поколение
    		    if (!found)
    		    {
    		        IList<Chain> newMutationChains = new List<Chain>();
    		        foreach (Chain chain in mutationChains)
    		        {
    		            int lastKey = chain.Keys.Last();
    		            int lastMutatedPos = chain.Positions.Last();

    		            string lastWord = wordFrom;

    		            WordItem lastItem = DictionaryMapper.Get(lastKey);

    		            if (lastItem != null)
    		                lastWord = lastItem.Word;

    		            IDictionary<int, int> nextMutations = DictionaryMapper.FindMutationVariants(lastWord, wordTo, fromLength, lastMutatedPos, chain.Keys);

    		            foreach (KeyValuePair<int, int> kvp in nextMutations)
    		            {
    		                WordItem nextItem = DictionaryMapper.Get(kvp.Key);
    		                string nextWord = nextItem.Word;

    		                int score = GetWordScore(nextWord, wordTo);

    		                // Новый потомок
    		                Chain newMutationChain = new Chain
    		                {
    		                    Keys = chain.Keys, 
                                Positions = chain.Positions, 
                                Score = score
    		                };
                            
                            newMutationChain.Keys.Add(kvp.Key);
                            newMutationChain.Positions.Add(kvp.Value);

    		                newMutationChains.Add(newMutationChain);
    		            }
    		        }

    		        // Предыдущее поколение убираем не полностью
    		        // Выкашиваем только часть самых слабых, оставляя сильных для конкуренции новому поколению
    		        int index = maxPopulation / 2;

    		        if (index < mutationChains.Count)
    		            mutationChains.RemoveRange(index, mutationChains.Count - index);

    		        mutationChains.AddRange(newMutationChains);

    		        // А если нового не появилось..
    		        if (!mutationChains.Any())
    		            throw new Exception("На шаге" + step + " (из максимально " + maxSteps + ") закончились варианты. Поиск не увенчался успехом.");

    		        // Сортируем новое поколение по "степени приспособленности" (похожести последнего слова цепочки на искомое)
    		        mutationChains.Sort((a, b) =>
    		        {
    		            int diff = b.Score - a.Score;
    		            if (diff == 0)
    		            {
    		                Random rnd = new Random();
    		                diff = rnd.Next(-1, 1);
    		            }
    		            return diff;
    		        });

    		        // Естественный отбор - оставляем самых лучших
    		        if (maxPopulation < mutationChains.Count)
    		            mutationChains.RemoveRange(maxPopulation, mutationChains.Count - maxPopulation);
    		    }
    		    else
    		        break;
    		}

            // слишком глубокий поиск?
		    if (step == maxSteps) 
			    throw new Exception("Пройдено максимально разрешённое число шагов (" + maxSteps + "), но поиск не увенчался успехом.");
		
            // Формируем итоговую цепочку из слов
            IEnumerable<WordItem> resultChain = resultKeysChain.Select(k => DictionaryMapper.Get(k));
            return resultChain.Where(i => i != null).Select(i => i.Word);
        }

          
        /// <summary>
        /// Функция оценки похожести слова
        /// </summary>
        /// <param name="word">Оцениваемое слово</param>
        /// <param name="comparationWord">Эталонное слово</param>
        /// <returns></returns>
        public int GetWordScore(string word, string comparationWord)
        {
            if (string.IsNullOrEmpty(word))
                throw new ArgumentException(word);

            if (string.IsNullOrEmpty(comparationWord))
                throw new ArgumentException(comparationWord);

            word = word.ToLower();
            comparationWord = comparationWord.ToLower();

		    int i;
            // Частый случай поиска - с одним и тем же эталонным словом, - используем кэширование

            if (_cachedComparationWord != comparationWord) {
			    _cachedComparationWord = comparationWord;

			    _wordLen = comparationWord.Length;
			    _cwLetters.Clear();

			    for (i = 0; i < _wordLen; i++) {
				    
                    char letter = comparationWord[i];
                    _cwLetters.Add(new Tuple<char, bool>(letter, IsVovel(letter)));
			    }
		    }

            double score = 0;

		    for (i = 0; i < _wordLen; i++) 
            {
			    char letter = word[i];
			    bool isVovel = IsVovel(letter);
			
			    // Полностью совпадающим символам максимальная оценка = 3
			    if (letter == _cwLetters[i].Item1) 
                {
				    score += 1;
				
				    if (isVovel) 
					    score += 2 + 1 * _letterFrequencesVovels[letter];
				  
				    else 
					    score += 0 + 3 * _letterFrequencesConsonants[letter];
				    
				    continue;
			    }
			
			    if (isVovel) {
				    
                    if (_cwLetters[i].Item2) 
					    // Совпадение позиции гласной буквы = 2
					    score += 2 + 2 * _letterFrequencesVovels[letter];
				    
				    else 
					    // Наличие гласной буквы = 1
					    score += 2 * _letterFrequencesVovels[letter];
				    
			    }
			    else 
                {
				    if (_letterFrequencesConsonants.ContainsKey(letter)) 
					    score += 3 * _letterFrequencesConsonants[letter];
				    
			    }
		    }
		
		    return (int)score;
        }

        public bool IsVovel(char letter)
        {
            return Array.IndexOf(_vovels, letter) >= 0;
        }

    }
}
