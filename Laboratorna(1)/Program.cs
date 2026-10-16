using System;

namespace Laboratorna1
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;
            int choice;
            Console.WriteLine("ЛАБОРАТОРНА РОБОТА №1");
            do
            {
                Console.WriteLine("1 - Завдання 1: список екземплярів класу" +
                    "\n2 - Завдання 2: статистика слів у тексті" +
                    "\n3 - Завдання 3: черга друку принтера" +
                    "\n0 - Вихід");
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
                        Task1.Run();
                        break;
                    case 2:
                        Task2.Run();
                        break;
                    case 3:
                        Task3.Run();
                        break;
                    case 0:
                        Console.WriteLine("Вихід з програми.");
                        break;
                    default:
                        Console.WriteLine("Цього завдання немає в списку:(. Виберіть інше завдання.");
                        break;
                }
            } while (choice != 0);
        }
    }
}