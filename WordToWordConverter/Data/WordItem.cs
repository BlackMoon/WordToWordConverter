﻿namespace WordToWordConverter.Data
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

        /// <summary>
        /// Значение
        /// </summary>
        public string Word { get; set; }
    }
}