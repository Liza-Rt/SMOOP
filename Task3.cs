using System;
using System.Collections.Generic;
using System.IO;

namespace Laboratorna1
{
    public class Task3
    {
        class PrintJob
        {
            public string User { get; set; }
            public string Document { get; set; }
            public bool IsPriority { get; set; }

            public PrintJob(string user, string document, bool isPriority)
            {
                User = user;
                Document = document;
                IsPriority = isPriority;
            }

            public override string ToString()
            {
                return User + " - " + Document;
            }
        }

        class PrintRecord
        {
            public string User { get; set; }
            public string Document { get; set; }
            public DateTime Time { get; set; }

            public PrintRecord(string user, string document, DateTime time)
            {
                User = user;
                Document = document;
                Time = time;
            }

            public override string ToString()
            {
                return User + " - " + Document + " - " +
                    Time.ToString("dd.MM.yyyy HH:mm:ss");
            }
        }

        static PriorityQueue<PrintJob, (int, int)> queue = new PriorityQueue<PrintJob, (int, int)>();
        static int order = 0;
        static List<PrintRecord> stat = new List<PrintRecord>();

        public static void Run()
        {
            int choice;

            do
            {
                Console.WriteLine("\n--- ЧЕРГА ДРУКУ ---");
                Console.WriteLine("1. Додати завдання на друк");
                Console.WriteLine("2. Надрукувати наступне завдання");
                Console.WriteLine("3. Показати чергу");
                Console.WriteLine("4. Показати статистику друку");
                Console.WriteLine("5. Зберегти статистику у файл");
                Console.WriteLine("0. Вихід");
                Console.Write("Ваш вибір: ");

                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Введіть число.");
                    choice = -1;
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        AddJob();
                        break;

                    case 2:
                        PrintNext();
                        break;

                    case 3:
                        Show();
                        break;

                    case 4:
                        ShowStat();
                        break;

                    case 5:
                        SaveStat();
                        break;

                    case 0:
                        Console.WriteLine("Завершення роботи.");
                        break;

                    default:
                        Console.WriteLine("Невірний пункт меню.");
                        break;
                }
            }
            while (choice != 0);
        }

        static void AddJob()
        {
            Console.Write("Користувач: ");
            string user = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(user))
            {
                Console.WriteLine("Ім'я користувача не може бути порожнім.");
                return;
            }

            Console.Write("Назва документа: ");
            string document = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(document))
            {
                Console.WriteLine("Назва документа не може бути порожньою.");
                return;
            }

            Console.Write("Пріоритет (1 - високий, 0 - звичайний): ");
            int priority;

            if (!int.TryParse(Console.ReadLine(), out priority) ||(priority != 0 && priority != 1))
            {
                Console.WriteLine("Невірний пріоритет.");
                return;
            }

            PrintJob job = new PrintJob(user, document, priority == 1);

            int level;

            if (job.IsPriority)
            {
                level = 0;
            }
            else
            {
                level = 1;
            }

            queue.Enqueue(job, (level, order));
            order++;

            Console.WriteLine("Завдання додано до черги.");
        }

        static void PrintNext()
        {
            if (queue.Count == 0)
            {
                Console.WriteLine("Черга порожня.");
                return;
            }
            PrintJob job = queue.Dequeue();

            Console.WriteLine("Друкується документ \"" + job.Document +
                "\" користувача " + job.User + "...");
            Console.WriteLine("Друк завершено.");

            PrintRecord record = new PrintRecord(job.User, job.Document, DateTime.Now);
            stat.Add(record);
        }

        static void Show()
        {
            if (queue.Count == 0)
            {
                Console.WriteLine("Черга порожня.");
                return;
            }

            Console.WriteLine("\nЗавдання в черзі:");

            foreach (var item in queue.UnorderedItems)
            {
                PrintJob job = item.Element;

                string mark;

                if (job.IsPriority)
                {
                    mark = " [високий пріоритет]";
                }
                else
                {
                    mark = " [звичайний]";
                }

                Console.WriteLine(job + mark);
            }
        }

        static void ShowStat()
        {
            if (stat.Count == 0)
            {
                Console.WriteLine("Статистика порожня.");
                return;
            }

            Console.WriteLine("\nСтатистика друку (користувач, документ, час):");

            for (int i = 0; i < stat.Count; i++)
            {
                Console.WriteLine(stat[i]);
            }
        }

        static void SaveStat()
        {
            if (stat.Count == 0)
            {
                Console.WriteLine("Статистика порожня, нічого зберігати.");
                return;
            }

            string outName = "print_stat.txt";

            using (StreamWriter writer = new StreamWriter(outName))
            {
                for (int i = 0; i < stat.Count; i++)
                {
                    writer.WriteLine(stat[i]);
                }
            }

            Console.WriteLine("Статистику збережено у файл " + outName);
        }
    }
}