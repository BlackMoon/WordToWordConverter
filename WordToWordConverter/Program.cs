using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StructureMap;
using WordToWordConverter.Configuration;
using WordToWordConverter.Converter;
using WordToWordConverter.Data;
using WordToWordConverter.Validation;
using StringValidator = WordToWordConverter.Validation.StringValidator;

namespace WordToWordConverter
{
    class Program
    {
        private const string Yes = "Y";
        private static void Main(string[] args)
        {
            IContainer container = ConfigureDependencies();
            IDictionaryMapper dictionaryMapper = null;

            HtmlToTextConverter.Combine(@"d:\3\_vel_v3\data\psrc\10\", "*.txt", "dic1-5.txt");

            // чтение настроек
            AlgorithmSettingsSection algConfig = (AlgorithmSettingsSection)ConfigurationManager.GetSection("algorithmsection");
            DictionarySettingsSection dicConfig = (DictionarySettingsSection)ConfigurationManager.GetSection("dictionarysection");

            string dictionaryFile = dicConfig.FileName;
            if (!string.IsNullOrEmpty(dictionaryFile))
                dictionaryFile = Path.GetFullPath(dictionaryFile);
           
            dictionaryMapper = new FileDictionaryMapper
            {
                FileName = dictionaryFile,
                NeedSort = dicConfig.NeedSort
            };

            Console.WriteLine("Загрузка словаря..");
            Task taskDictionary = dictionaryMapper.Load();

            IConverter converter = container.GetInstance<IConverter>();
            converter.DictionaryMapper = dictionaryMapper;

            IStringValidator validator = container.GetInstance<IStringValidator>();

            string answer = Yes;
            while (answer != null && answer.ToUpper() == Yes)
            {
                Console.Clear();
                WriteWelcome(algConfig);

                Stopwatch watch = new Stopwatch();
                try
                {
                    taskDictionary.Wait();

                    Console.WriteLine("Введите начальное слово:");
                    string wordFrom = Console.ReadLine();
                    
                    string msg;

                    validator.Value = wordFrom;
                    if (!validator.Validate(out msg))
                        throw new Exception(msg);

                    Console.WriteLine();
                    Console.WriteLine("Введите конечное слово:");
                    string wordTo = Console.ReadLine();
                   
                    validator.Value = wordTo;
                    if (!validator.Validate(out msg))
                        throw new Exception(msg);

                    // Преобразование
                    Console.WriteLine();
                    Console.WriteLine("Идет преобразование..");

                    watch.Start();

                    Task<IEnumerable<string>> taskConverter =
                        Task.Factory.StartNew(
                            () => converter.FindMutationChain(wordFrom, wordTo, algConfig.MaxSteps, algConfig.MaxPopulation));

                    Task taskConverterContinue = taskConverter.ContinueWith(t =>
                    {
                        Console.WriteLine();
                        Console.WriteLine("Цепочка преобразований");

                        int i = 0;
                        foreach (string word in t.Result)
                        {
                            Console.WriteLine("\t" + ++i + "\t" + word);
                        }
                        Console.WriteLine();
                    });

                    // интервал - 5 сек
                    while (!Task.WaitAll(new[] {taskConverter, taskConverterContinue}, 5000))
                    {
                        Console.Write("-");
                    }

                }
                catch (AggregateException aex)
                {
                    Console.WriteLine();

                    Console.WriteLine(string.Join("\n", aex.InnerExceptions
                        .Where(ex => ex.Source == Assembly.GetExecutingAssembly().GetName().Name)
                        .Select(ex => ex.Message)));
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine(ex.Message);
                }

                watch.Stop();

                Console.WriteLine("Время: " + watch.Elapsed);

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
                x.For<IStringValidator>().Use<StringValidator>();
            });
        }
        
        private static void WriteWelcome(AlgorithmSettingsSection config)
        {
            Console.WriteLine("WordToWordConverter");
            Console.WriteLine("-------------------");
            Console.WriteLine("Настройки (.config): maxSteps - " + config.MaxSteps + ", maxPopulation - " + config.MaxPopulation);
            Console.WriteLine();
        }
    }
}
