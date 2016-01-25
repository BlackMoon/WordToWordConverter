using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WordToWordConverter.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class FileDictionaryMapper : IDictionaryMapper
    {
        public bool NeedSort { get; set; }

        public string FileName { get; set; }

        private readonly List<WordItem> _items = new List<WordItem>();

        public IEnumerable<WordItem> GetAll()
        {
            return _items;
        }

        public WordItem Get(int key)
        {
            WordItem item = null;

            if (key > -1)
                item = _items[key];

            return item;
        }

        public int? GetKey(string value)
        {
            int? key = null;

            WordItem item = new WordItem() { Word = value };

            int id = _items.BinarySearch(item, new ItemComparer());
            if (id > -1)
                key = id;

            return key;
        }

        /// <summary>
        /// Получение списка пар {id слова, позиция мутации символа} для возможных вариантов мутаций.
        /// Для поиска используется рабочий словарь плюс общие вспомогательные словари
        /// </summary>
        /// <param name="wordFrom">Начальное слово</param>
        /// <param name="wordTo">Конечное слово</param>
        /// <param name="wordLen">Длина обоих слов</param>
        /// <param name="disabledMutationPos">индекс в слове буквы, которую не нужно менять (была изменена на предыдущем шаге)</param>
        /// <param name="excludedWordIds">Уже использованные слова</param>
        /// <returns>Список вариантов преобразований [id слова из словаря, № буквы]</returns>
        public IDictionary<int, int> FindMutationVariants(string wordFrom, string wordTo, int wordLen, int disabledMutationPos, IList<int> excludedWordIds)
        {
            IDictionary<int, int> variants = new Dictionary<int, int>();
            List<WordItem> items = (List<WordItem>)GetAll(); 

            for (int pos = 0; pos < wordLen; pos++)
            {
                // Пропускаем исключённую букву (нет смысла менять ту же, что на пред. шаге)
			    if (pos == disabledMutationPos) 
                    continue;
                
                // Получаем обгрызенное слово без pos-й буквы
    			string wordBeginning = wordFrom.Substring(0, pos),
	    		       wordEnding = wordFrom.Substring(pos + 1);

                // Ищем такие псевдослова
                if (pos < -2)
                {

                }
                else
                {
                    int ix = 0;
                    // Вспомогательного словаря нет - берём основной, ищем начало слова и перебираем всё подходящее
				    if (pos == 0) {
					    // Когда совсем нет вспомогательных словарей, и рассматривается мутация 
					    // первой буквы слова, это совсем не круто - нужно использовать полный перебор
					    // (здесь тоже можно пойти на оптимизацию группировки слов по длине)
					    ix = 0;
				    }
				    else {
					    // Определяем с какой позиции в словаре начинать перебор
				        
                        WordItem item = items.FirstOrDefault(i => i.Word.StartsWith(wordBeginning));
				        if (item != null)
				            ix = item.Id;

				        if (ix < 0)
				            ix = -ix;
				    }

                    // Перебираем
				    for (; ix < items.Count; ix++) 
                    {
					    WordItem item = items[ix];
                        string word = item.Word;

                        // Пропускаем по критерию длины слова (простор для дальнейшей оптимизации)
                        if (word.Length != wordLen)
                            continue;

					    // Можно выходить, если слово уже начинается не так
					    if (word.Substring(0, pos) != wordBeginning)
						    break;

					    // Наконец, проверяем соответствие конца слова
					    if (word.Substring(pos + 1) != wordEnding) 
						    continue;
					
					    // Не повторяемся - пропускаем ранее использованные слова
					    if (excludedWordIds.Contains(ix)) 
                            continue;
					
					    // Слово подходит, добавляем как вариант
					    variants[item.Id] = pos;
				    }

                }
            }
            return variants;
        }

        public Task Load()
        {
            _items.Clear();

            return Task.Run(() =>
            {
                if (!string.IsNullOrEmpty(FileName))
                {
                    string[] lines = File.ReadAllLines(FileName);

                    int len = lines.Length;

                    _items.Capacity = len;
                    for (int i = 0; i < len; i++)
                    {
                        if (!string.IsNullOrEmpty(lines[i]))
                            _items.Add(new WordItem(i, lines[i].ToLower()));
                    }

                    // сортировка по возрастанию
                    if (NeedSort)
                        _items.Sort((x, y) => String.CompareOrdinal(x.Word, y.Word));
#if DEBUG
                    File.WriteAllLines("dicTest.txt", _items.Select(i => i.Word));
#endif
                }
            });
        }
    }

    class ItemComparer : IComparer<WordItem>
    {
        public int Compare(WordItem x, WordItem y)
        {
            // ReSharper disable once StringCompareToIsCultureSpecific
            return x.Word.CompareTo(y.Word);
        }
    }
}
