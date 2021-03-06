﻿using System.Collections.Generic;

namespace WordToWordConverter.Converter
{
    /// <summary>
    /// Цепочка преобразований
    /// </summary>
    public class Chain
    {
        /// <summary>
        /// Ключи новых наденных слов
        /// </summary>
        public List<int> Keys { get; set; }

        /// <summary>
        /// Позиции измененных букв
        /// </summary>
        public List<int> Positions { get; set; }

        /// <summary>
        /// Веса преобразований (для поиска оптимальных цепочек)
        /// </summary>
        public int Score { get; set; }

        public Chain()
        {
            Keys = new List<int>();
            Positions = new List<int>();
        }

        public Chain(int capacity)
        {
            Keys = new List<int>(capacity);
            Positions = new List<int>(capacity);
        }
    }
}
