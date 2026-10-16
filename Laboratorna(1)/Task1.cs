using System;
using System.Collections.Generic;

namespace Laboratorna1
{
    public class Task1
    {
        class MilitaryCard
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public DateTime BirthDate { get; set; }
            public string Address { get; set; }
            public string Fitness { get; set; }
            public DateTime DemobilizationDate { get; set; }
            public string ServicePlace { get; set; }
            public MilitaryCard(int id, string fullName, DateTime birthDate,
            string address, string fitness, DateTime demobilizationDate,
            string servicePlace)
            {
                Id = id;
                FullName = fullName;
                BirthDate = birthDate;
                Address = address;
                Fitness = fitness;
                DemobilizationDate = demobilizationDate;
                ServicePlace = servicePlace;
            }

            public override string ToString()
            {
                return $"ID: {Id}, ПІБ: {FullName}, Дата народження: {BirthDate.ToShortDateString()}, " +
                    $"Адреса: {Address}, Придатність: {Fitness}, Дата демобілізації: {DemobilizationDate.ToShortDateString()}, " +
                    $"Місце служби: {ServicePlace}";
            }
            public override bool Equals(object obj)
            {
                if (obj is MilitaryCard other)
                {
                    return FullName == other.FullName &&
                        BirthDate == other.BirthDate &&
                        Address == other.Address &&
                        Fitness == other.Fitness &&
                        DemobilizationDate == other.DemobilizationDate &&
                        ServicePlace == other.ServicePlace;
                }
                return false;
            }
            public override int GetHashCode()
            {
                return HashCode.Combine(FullName, BirthDate, Address,
                    Fitness, DemobilizationDate, ServicePlace);
            }
        }

        static List<MilitaryCard> cards = new List<MilitaryCard>()
        {
        new MilitaryCard(1, "Іваненко Іван Іванович", new DateTime(2000, 5, 10),
            "м. Кременчук", "Придатний", new DateTime(2024, 5, 10), "м. Київ"),

        new MilitaryCard(2, "Петренко Петро Петрович", new DateTime(1999, 3, 15),
            "м. Полтава", "Придатний", new DateTime(2025, 2, 20), "м. Львів"),

        new MilitaryCard(3, "Сидоренко Олег Олегович", new DateTime(2001, 7, 8),
            "м. Харків", "Обмеження за станом здоров'я", new DateTime(2025, 6, 15), "м. Одеса")
        };

        public static void Run()
        {
            int choice;

            do
            {
                Console.WriteLine("\n--- ОБЛІКОВІ КАРТКИ ---");
                Console.WriteLine("1. Показати всі картки");
                Console.WriteLine("2. Додати картку");
                Console.WriteLine("3. Редагувати картку");
                Console.WriteLine("4. Видалити картку");
                Console.WriteLine("5. Знайти картку за ID");
                Console.WriteLine("6. Знайти за періодом демобілізації");
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
                        for (int i = 0; i < cards.Count; i++)
                        {
                            ShowCard(cards[i]);
                        }
                        break;

                    case 2:
                        AddCard();
                        break;

                    case 3:
                        EditCard();
                        break;

                    case 4:
                        DeleteCard();
                        break;

                    case 5:
                        SearchById();
                        break;

                    case 6:
                        SearchByDate();
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

        static void ShowCard(MilitaryCard card)
        {
            Console.WriteLine("-------------------------");
            Console.WriteLine(card);
        }

        static void AddCard()
        {
            int id;

            if (cards.Count == 0)
            {
                id = 1;
            }
            else
            {
                id = cards[cards.Count - 1].Id + 1;
            }

            Console.Write("ПІБ: ");
            string fullName = Console.ReadLine();

            Console.Write("Дата народження (дд.мм.рррр): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime birthDate))
            {
                Console.WriteLine("Невірна дата.");
                return;
            }

            Console.Write("Адреса: ");
            string address = Console.ReadLine();

            Console.Write("Придатність: ");
            string fitness = Console.ReadLine();

            Console.Write("Дата демобілізації (дд.мм.рррр): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime demobilizationDate))
            {
                Console.WriteLine("Невірна дата.");
                return;
            }

            Console.Write("Місце служби: ");
            string servicePlace = Console.ReadLine();

            MilitaryCard card = new MilitaryCard(id, fullName, birthDate, address,
                fitness, demobilizationDate, servicePlace);

            if (cards.Contains(card))
            {
                Console.WriteLine("Така картка вже існує.");
                return;
            }

            cards.Add(card);
            Console.WriteLine("Картку додано.");
        }

        static void EditCard()
        {
            Console.Write("Введіть ID картки для редагування: ");
            int id;

            if (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("Невірний ID.");
                return;
            }

            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].Id == id)
                {
                    Console.Write("Новий ПІБ: ");
                    cards[i].FullName = Console.ReadLine();

                    Console.Write("Нова адреса: ");
                    cards[i].Address = Console.ReadLine();

                    Console.Write("Нова придатність: ");
                    cards[i].Fitness = Console.ReadLine();

                    Console.Write("Нове місце служби: ");
                    cards[i].ServicePlace = Console.ReadLine();

                    Console.WriteLine("Картку змінено.");
                    return;
                }
            }

            Console.WriteLine("Картку не знайдено.");
        }

        static void DeleteCard()
        {
            Console.Write("Введіть ID картки для видалення: ");
            int id;

            if (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("Невірний ID.");
                return;
            }

            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].Id == id)
                {
                    cards.RemoveAt(i);
                    Console.WriteLine("Картку видалено.");
                    return;
                }
            }

            Console.WriteLine("Картку не знайдено.");
        }

        static void SearchById()
        {
            Console.Write("Введіть ID: ");
            int id;

            if (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("Невірний ID.");
                return;
            }

            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].Id == id)
                {
                    ShowCard(cards[i]);
                    return;
                }
            }

            Console.WriteLine("Картку не знайдено.");
        }

        static void SearchByDate()
        {
            DateTime startDate;
            DateTime endDate;

            Console.Write("Початкова дата (дд.мм.рррр): ");
            if (!DateTime.TryParse(Console.ReadLine(), out startDate))
            {
                Console.WriteLine("Невірна дата.");
                return;
            }

            Console.Write("Кінцева дата (дд.мм.рррр): ");
            if (!DateTime.TryParse(Console.ReadLine(), out endDate))
            {
                Console.WriteLine("Невірна дата.");
                return;
            }

            if (startDate > endDate)
            {
                Console.WriteLine("Початкова дата має бути раніше кінцевої.");
                return;
            }

            bool found = false;

            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].DemobilizationDate >= startDate &&
                    cards[i].DemobilizationDate <= endDate)
                {
                    ShowCard(cards[i]);
                    found = true;
                }
            }

            if (!found)
            {
                Console.WriteLine("Карток за цей період не знайдено.");
            }
        }
    }
}