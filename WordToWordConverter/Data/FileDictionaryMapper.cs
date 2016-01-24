using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WordToWordConverter.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class FileDictionaryMapper : IDictionaryMapper
    {
        public string FileName { get; set; }

        private readonly List<WordItem> _items = new List<WordItem>();

        public IEnumerable<WordItem> GetAll()
        {
            throw new NotImplementedException();
        }

        public WordItem Get(int key)
        {
            return _items.Find(w => w.Id == key);
        }

        public int? GetKey(string value)
        {
            int? id = null;

            WordItem item = _items.Find(w => w.Word == value);
            if (item != null)
                id = item.Id;

            return id;
        }

        /// <summary>
        /// Получение списка пар {id слова, позиция мутации символа} для возможных вариантов мутаций.
        /// Для поиска используется рабочий словарь плюс общие вспомогательные словари
        /// </summary>
        /// <param name="wordFrom">Начальное слово</param>
        /// <param name="wordTo">Конечное слово</param>
        /// <param name="wordLen">Длина обоих слов</param>
        /// <param name="disabledMutationPos">ндекс в слове буквы, которую не нужно менять (была изменена на предыдущем шаге)</param>
        /// <param name="excludedWordKeys">Уже использованные слова</param>
        /// <returns></returns>
        public IDictionary<int, int> FindMutationVariants(string wordFrom, string wordTo, int wordLen, int disabledMutationPos, IList<int> excludedWordKeys)
        {
            IDictionary<int, int> variants = new Dictionary<int, int>();

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
                    int ix;
                    // Вспомогательного словаря нет - берём основной, ищем начало слова и перебираем всё подходящее
				    if (pos == 0) {
					    // Когда совсем нет вспомогательных словарей, и рассматривается мутация 
					    // первой буквы слова, это совсем не круто - нужно использовать полный перебор
					    // (здесь тоже можно пойти на оптимизацию группировки слов по длине)
					    ix = 0;
				    }
				    else {
					    // Определяем с какой позиции в словаре начинать перебор
					    ix = _items.FindIndex(i => i.Word.StartsWith(wordBeginning));
				        if (ix < 0)
				            ix = -ix;
				    }

                    // Перебираем
				    for (; ix < _items.Count; ix++) 
                    {
					    WordItem item = _items[ix];
                        string word = item.Word;

					    // Можно выходить, если слово уже начинается не так
					    if (word.Substring(0, pos) != wordBeginning) {
						    break;
					    }
					    
                        // Пропускаем по критерию длины слова (простор для дальнейшей оптимизации)
					    if (word.Length != wordLen) {
						    continue;
					    }

					    // Наконец, проверяем соответствие конца слова
					    if (word.Substring(pos + 1) != wordEnding) {
						    continue;
					    }
					
					    // Не повторяемся - пропускаем ранее использованные слова
					    if (excludedWordKeys.Contains(ix)) 
                            continue;
					
					    // Слово подходит, добавляем как вариант
					    variants.Add(ix, pos);
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
                    string[] lines = System.IO.File.ReadAllLines(FileName);

                    int len = lines.Length;

                    _items.Capacity = len;
                    for (int i = 0; i < len; i++)
                    {
                        _items.Add(new WordItem(i, lines[i]));
                    }
                }
            });
        }
    }
}
