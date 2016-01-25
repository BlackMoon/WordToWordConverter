using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace WordToWordConverter
{
    /// <summary>
    /// Формирование словарей
    /// </summary>
    /// IEnumerable<string> files = Directory.GetFiles(@"d:\3\_vel_v3\data\psrc\", "*.html");
    /// string f = @"d:\3\_vel_v3\data\psrc\7-ми буквенные слова на букву Д.html";
    /// HtmlToTextConverter.Convert(f, Path.ChangeExtension(f, ".txt"));
    /// Parallel.ForEach(files, f => HtmlToTextConverter.Convert(f, Path.ChangeExtension(f, ".txt")));
    /// HtmlToTextConverter.Combine(@"d:\3\_vel_v3\data\psrc\", "*.txt", "dic.txt");
    public static class HtmlToTextConverter
    {
        public static void Combine(string path, string searchPattern, string outputFilePath)
        {
            string[] files = Directory.GetFiles(path, searchPattern);
            System.Diagnostics.Debug.WriteLine("Number of files: {0}.", files.Length);
            
            using (var outputStream = File.Create(outputFilePath))
            {
                foreach (string f in files)
                {
                    using (var inputStream = File.OpenRead(f))
                    {
                        // Buffer size can be passed as the second argument.
                        inputStream.CopyTo(outputStream);
                    }
                    Console.WriteLine("The file {0} has been processed.", f);
                }
            }
        }

        /// <summary>
        /// Конвертация html в txt
        /// </summary>
        /// <param name="fileName">html-файл</param>
        /// <param name="outName">txt-файл</param>
        public static void Convert(string fileName, string outName)
        {
            string text = File.ReadAllText(fileName);

            Regex rgx = new Regex(@"blue['|""]>([\s\w\W]+?)</[span|SPAN]|blue; mso-bidi-font-size: 1[0|2].0pt['|""]>([\s\w\W]+?)</[span|SPAN]|blue; FONT-SIZE: 16pt; mso-bidi-font-size: 1[0|2]\.0pt['|""]>([\s\w\W]+?)</[span|SPAN]");

            if (rgx.IsMatch(text))
            {
                StringBuilder sb = new StringBuilder();

                foreach (Match m in rgx.Matches(text))
                {
                    int ix = m.Groups.Count;

                    for (int i = m.Groups.Count - 1; i > 0; i--)
                    {
                        Group g = m.Groups[i];
                        if (g.Success)
                        {
                            string word = g.Value;
                            if (!string.IsNullOrEmpty(word))
                            {
                                sb.AppendLine(word.ToLower().TrimEnd());
                                System.Diagnostics.Debug.WriteLine(word.TrimEnd());
                            }    
                        }
                    }
                }

                File.WriteAllText(outName, sb.ToString());
            }
        }
    }
}
