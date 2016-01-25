using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordToWordConverter.Data
{
    public class DbDictionaryMapper: IDictionaryMapper
    {
        private readonly SQLiteConnection _connection;

        public string ConnectionString { get; set; }

        public DbDictionaryMapper()
        {
            string connStr = ConfigurationManager.ConnectionStrings[ConnectionString].ConnectionString;
            if (!string.IsNullOrEmpty(connStr))
                _connection = new SQLiteConnection(connStr);
        }

        public IEnumerable<WordItem> GetAll()
        {
            throw new NotImplementedException();   
        }

        public WordItem Get(int key)
        {
            throw new NotImplementedException();
        }

        public int? GetKey(string value)
        {
            throw new NotImplementedException();
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

            if (_connection.State == ConnectionState.Closed)
                _connection.Open();

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

                    List<WordItem> items = new List<WordItem>(); 
                    using (IDbCommand cmd = new SQLiteCommand("SELECT w.id, w.word FROM worditems w WHERE w.word LIKE @0", _connection))
                    {
                        cmd.Parameters.Add(new SQLiteParameter(DbType.String, wordBeginning));
                        using (IDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                items.Add(new WordItem(reader.GetInt32(0), reader.GetString(1)));
                            }
                        }
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
            return Task.Run(() => _connection.Open());
        }
    }
}
