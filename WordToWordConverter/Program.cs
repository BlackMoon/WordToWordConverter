using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using StructureMap;
using WordToWordConverter.Configuration;
using WordToWordConverter.Converter;
using WordToWordConverter.Data;

namespace WordToWordConverter
{
    class Program
    {
        private const string Yes = "Y";
        private static void Main(string[] args)
        {
            IContainer container = ConfigureDependencies();

            AlgorithmSettingsSection config = (AlgorithmSettingsSection)ConfigurationManager.GetSection("algorithmsection");
            
            string dictionaryFile = ConfigurationManager.AppSettings["dictionaryFile"];
            if (!string.IsNullOrEmpty(dictionaryFile))
                dictionaryFile = Path.GetFullPath(dictionaryFile);

            FileDictionaryMapper dictionaryMapper = new FileDictionaryMapper(dictionaryFile);
            Task task = dictionaryMapper.Load();

            string answer = Yes;
            while (answer != null && answer.ToUpper() == Yes)
            {
                Console.Clear();

                WriteWelcome();
                ReadWords();

                task.Wait();
                // TODO operations
                IConverter converter = container.GetInstance<IConverter>();

                Console.WriteLine();
                Console.WriteLine("Operation");
                Console.WriteLine();

                Console.WriteLine("Продолжить? [Y/N]");
                answer = Console.ReadLine();
            } 
        }

        private static IContainer ConfigureDependencies()
        {
            return new Container(x =>
            {
                x.For<IConverter>().Use<WordConverter>();
            });
        }

        private static void ReadWords()
        {
            Console.WriteLine("Введите начальное слово:");
            string word1 = Console.ReadLine();

            Console.WriteLine("Введите конечное слово:");
            string word2 = Console.ReadLine();
        }

        private static void WriteWelcome()
        {
            Console.WriteLine("WordToWordConverter");
            Console.WriteLine("-------------------");
            Console.WriteLine();
        }
    }
}
