
using System;
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
            string lWordFrom = wordFrom.ToLower(),
                   lWordTo = wordTo.ToLower();

            // Для идентичных слов ответ очевиден и не требует поиска
		    if (lWordFrom == lWordTo) 
			    return new [] { lWordTo };

            int fromLength = wordFrom.Length,
                toLength = wordTo.Length;

            if (fromLength != toLength)
                throw new Exception("Слова должны быть одинаковой длины.");

            // Существование первого слова в словаре для алгоритма не обязательно
            int? wordFromId = DictionaryMapper.GetKey(lWordFrom);

            // Но для второго слова, для простоты, будем это требовать
            int? wordToId = DictionaryMapper.GetKey(lWordTo);
            if (!wordToId.HasValue) 
                throw new NullReferenceException("Конечное слово " + wordTo + " не обнаружено в словаре.");
            
            Chain startChain  = new Chain();
            startChain.Keys.Add(wordFromId ?? -1);
            startChain.Positions.Add(-1);
            startChain.Scores.Add(0);

            IList<Chain> mutatedChains = new List<Chain>() { startChain };
            IList<int> resultKeysChain = new List<int>();

            // Главный цикл генетического алгоритма поиска
            int step = 0;
            
    		for (; step < maxSteps; step++)
    		{
    		    bool found = false;
                // Не дошли ли до искомого слова?
    			foreach (Chain chain in mutatedChains) 
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
    		        foreach (Chain chain in mutatedChains)
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
					        Chain newMutationChain = chain;
					        newMutationChain.Keys.Add(kvp.Key);
					        newMutationChain.Positions.Add(kvp.Value);
					        newMutationChain.Scores.Add(score);
					
					        newMutationChains.Add(newMutationChain);

    		            }


    		        }
    		    }
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
            string cachedComparationWord = string.Empty;
		    int i, wordLen = 0;
		    IList<Tuple<char, bool>> cwLetters = new List<Tuple<char, bool>>();
		
            if (cachedComparationWord != comparationWord) {
			    cachedComparationWord = comparationWord;
			    wordLen = comparationWord.Length;
			    cwLetters.Clear();

			    for (i = 0; i < wordLen; i++) {
				    
                    char letter = comparationWord[i];
                    cwLetters.Add(new Tuple<char, bool>(letter, IsVovel(letter)));
			    }
		    }

            double score = 0;
		    for (i = 0; i < wordLen; i++) 
            {
			    char letter = word[i];
			    bool isVovel = IsVovel(letter);
			
			    // Полностью совпадающим символам максимальная оценка = 3
			    if (letter == cwLetters[i].Item1) 
                {
				    score += 1;
				
				    if (isVovel) 
					    score += 2 + 1 * _letterFrequencesVovels[letter];
				  
				    else 
					    score += 0 + 3 * _letterFrequencesConsonants[letter];
				    
				    continue;
			    }
			
			    if (isVovel) {
				    
                    if (cwLetters[i].Item2) 
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
