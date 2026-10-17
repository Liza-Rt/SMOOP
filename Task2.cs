using System;
using System.Collections.Generic;
using System.IO;

namespace Laboratorna1
{
    public class Task2
    {
        const string InfoFile = "firstFile.txt";

        public static void Run()
        {
            List<string> files = ReadFileList();

            if (files.Count == 0)
            {
                return;
            }

            int again;

            do
            {
                Console.WriteLine("\n СТАТИСТИКА СЛІВ ");

                for (int i = 0; i < files.Count; i++)
                {
                    Console.WriteLine((i + 1) + ". " + files[i]);
                }

                Console.Write("Оберіть номер файлу: ");
                int number;

                if (!int.TryParse(Console.ReadLine(), out number) ||
                    number < 1 || number > files.Count)
                {
                    Console.WriteLine("Невірний номер файлу.");
                }
                else
                {
                    string path = files[number - 1];
                    Dictionary<string, int> stat = CountWords(path);

                    if (stat != null)
                    {
                        ShowStat(stat);
                        SaveStat(path, stat);
                    }
                }

                Console.Write("Проаналізувати ще один файл? (1 - так, 0 - ні): ");
                if (!int.TryParse(Console.ReadLine(), out again))
                {
                    again = 0;
                }
            }
            while (again == 1);
        }

        static List<string> ReadFileList()
        {
            List<string> files = new List<string>();

            if (!File.Exists(InfoFile))
            {
                Console.WriteLine("Файл " + InfoFile + " не знайдено.");
                return files;
            }

            using (StreamReader reader = new StreamReader(InfoFile))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();

                    if (line != "")
                    {
                        files.Add(line);
                    }
                }
            }

            if (files.Count == 0)
            {
                Console.WriteLine("У файлі " + InfoFile + " немає жодного запису.");
            }

            return files;
        }

        static Dictionary<string, int> CountWords(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine("Файл не знайдено: " + path);
                return null;
            }

            Dictionary<string, int> stat = new Dictionary<string, int>();

            using (StreamReader reader = new StreamReader(path))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    string word = "";

                    for (int i = 0; i < line.Length; i++)
                    {
                        if (char.IsLetter(line[i]) || line[i] == '\'' || line[i] == '’')
                        {
                            word += char.ToLower(line[i]);
                        }
                        else
                        {
                            AddWord(stat, ref word);
                        }
                    }

                    AddWord(stat, ref word);
                }
            }

            return stat;
        }

        static void AddWord(Dictionary<string, int> stat, ref string word)
        {
            if (word == "")
            {
                return;
            }

            if (stat.ContainsKey(word))
            {
                stat[word]++;
            }
            else
            {
                stat.Add(word, 1);
            }

            word = "";
        }

        
        static void SaveStat(string path, Dictionary<string, int> stat)
        {
            Console.Write("Зберегти статистику у файл? (1 - так, 0 - ні): ");
            int save;

            if (!int.TryParse(Console.ReadLine(), out save) || save != 1)
            {
                return;
            }

            string outName = "stat_" + Path.GetFileNameWithoutExtension(path) + "_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".txt";

            List<KeyValuePair<string, int>> words = GetSorted(stat);

            using (StreamWriter writer = new StreamWriter(outName))
            {
                foreach (KeyValuePair<string, int> pair in words)
                {
                    writer.WriteLine(pair.Key + ": " + pair.Value);
                }
            }
            Console.WriteLine("Статистику збережено у файл " + outName);
        }
        static List<KeyValuePair<string, int>> GetSorted(Dictionary<string, int> stat)
        {
            List<KeyValuePair<string, int>> words =
                new List<KeyValuePair<string, int>>(stat);

            words.Sort(delegate (KeyValuePair<string, int> a, KeyValuePair<string, int> b)
            {
                if (a.Value != b.Value)
                {
                    return b.Value.CompareTo(a.Value);
                }

                return a.Key.CompareTo(b.Key);
            });

            return words;
        }

        static void ShowStat(Dictionary<string, int> stat)
        {
            if (stat.Count == 0)
            {
                Console.WriteLine("У файлі не знайдено жодного слова.");
                return;
            }

            List<KeyValuePair<string, int>> words = GetSorted(stat);

            Console.WriteLine("\nСлово: кількість використань");

            foreach (KeyValuePair<string, int> pair in words)
            {
                Console.WriteLine(pair.Key + ": " + pair.Value);
            }
        }
        
    }
}