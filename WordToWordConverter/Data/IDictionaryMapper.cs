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
        IDictionary<int, int> FindMutationVariants(string wordFrom, string wordTo, int wordLen, int disabledMutationPos, IList<int> excludedWordIds);

        Task Load();
    }
}