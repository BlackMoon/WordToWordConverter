namespace WordToWordConverter.Data
{
    /// <summary>
    /// Единица словаря - слово
    /// </summary>
    public class WordItem
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }

        private string _word;

        /// <summary>
        /// Значение
        /// </summary>
        public string Word
        {
            get { return _word; }
            set
            {
                _word = (value != null) ? value.ToLower() : null;
            }
        }

        public WordItem(int id, string word)
        {
            Id = id;
            Word = word;
        }
    }

}
