﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
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

            AlgorithmSettingsSection config = (AlgorithmSettingsSection)ConfigurationManager.GetSection("algorithmsection");
            
            string dictionaryFile = ConfigurationManager.AppSettings["dictionaryFile"];
            if (!string.IsNullOrEmpty(dictionaryFile))
                dictionaryFile = Path.GetFullPath(dictionaryFile);

            FileDictionaryMapper dictionaryMapper = new FileDictionaryMapper { FileName = dictionaryFile };
            Task taskDictionary = dictionaryMapper.Load();

            IConverter converter = container.GetInstance<IConverter>();
            converter.DictionaryMapper = dictionaryMapper;

            IStringValidator validator = container.GetInstance<IStringValidator>();

            string answer = Yes;
            while (answer != null && answer.ToUpper() == Yes)
            {
                Console.Clear();
                WriteWelcome(config);

                try
                {
                    Console.WriteLine("Введите начальное слово:");
                    string wordFrom = Console.ReadLine();
                    wordFrom = "НОгА";
                    string msg;

                    validator.Value = wordFrom;
                    if (!validator.Validate(out msg))
                        throw new Exception(msg);

                    Console.WriteLine();
                    Console.WriteLine("Введите конечное слово:");
                    string wordTo = Console.ReadLine();
                    wordTo = "РАБА";
                    validator.Value = wordTo;
                    if (!validator.Validate(out msg))
                        throw new Exception(msg);

                    taskDictionary.Wait();

                    // Преобразование
                    Console.WriteLine();
                    Console.WriteLine("Идет преобразование..");

                    Stopwatch watch = Stopwatch.StartNew();

                    Task<IEnumerable<string>> taskConverter =
                        Task.Factory.StartNew(
                            () => converter.FindMutationChain(wordFrom, wordTo, config.MaxSteps, config.MaxPopulation));

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
                    watch.Stop();

                    Console.WriteLine("Время: " + watch.Elapsed);

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
