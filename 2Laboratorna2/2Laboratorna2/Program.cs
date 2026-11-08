using System;
using Lab2.Task1;
using Lab2.Task2;
using Lab2.Task3;

namespace Lab2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            while (true)
            {
                Console.WriteLine("\n1 - Завдання 1: фірми" +
                    "\n2 - Завдання 2: телефони" +
                    "\n3 - Завдання 3: підприємство" +
                    "\n0 - Вихід");
                Console.Write("Ваш вибір: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Введіть число.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        Methods1.Menu();
                        break;
                    case 2:
                        Methods2.Menu();
                        break;
                    case 3:
                        Methods3.Menu();
                        break;
                    case 0:
                        return;
                    default:
                        Console.WriteLine("Немає такого пункту.");
                        break;
                }
            }
        }
    }
}