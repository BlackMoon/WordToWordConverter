using System;
using System.Net.Mime;

namespace WordToWordConverter
{
    class Program
    {
        private const string Yes = "Y";
        static void Main(string[] args)
        {
            string answer = Yes;
            while (answer != null && answer.ToUpper() == Yes)
            {
                Console.Clear();

                WriteWelcome();
                ReadWords();

                // TODO operations
                Console.WriteLine();
                Console.WriteLine("Operation");
                Console.WriteLine();

                Console.WriteLine("Продолжить? [Y/N]");
                answer = Console.ReadLine();
            } 
        }

        static void ReadWords()
        {
            Console.WriteLine("Введите начальное слово:");
            string word1 = Console.ReadLine();

            Console.WriteLine("Введите конечное слово:");
            string word2 = Console.ReadLine();
        }

        static void WriteWelcome()
        {
            Console.WriteLine("WordToWordConverter");
            Console.WriteLine("-------------------");
            Console.WriteLine();
        }
    }
}
