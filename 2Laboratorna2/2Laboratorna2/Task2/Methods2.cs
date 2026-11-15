using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab2.Task2
{
    public static class Methods2
    {
        private static List<Phone> phones = new List<Phone>()
        {
            new Phone("iPhone 13", "Apple", 650, new DateTime(2021, 9, 24)),
            new Phone("iPhone 15", "Apple", 950, new DateTime(2023, 9, 22)),
            new Phone("Galaxy S21", "Samsung", 580, new DateTime(2021, 1, 29)),
            new Phone("Galaxy S22", "Samsung", 720, new DateTime(2022, 2, 25)),
            new Phone("Galaxy A52", "Samsung", 350, new DateTime(2021, 3, 26)),
            new Phone("Xperia 5", "Sony", 490, new DateTime(2020, 9, 25)),
            new Phone("Xperia 1 V", "Sony", 900, new DateTime(2023, 6, 1)),
            new Phone("Redmi Note 10", "Xiaomi", 180, new DateTime(2021, 3, 4)),
            new Phone("Redmi Note 12", "Xiaomi", 260, new DateTime(2023, 3, 23)),
            new Phone("iPhone 10", "Apple", 300, new DateTime(2017, 11, 3)),
            new Phone("Galaxy S23", "Samsung", 850, new DateTime(2023, 2, 17)),
            new Phone("Redmi 9", "Xiaomi", 120, new DateTime(2020, 6, 10)),
            new Phone("iPhone 13", "Apple", 620, new DateTime(2021, 9, 24)),
            new Phone("Galaxy S22", "Samsung", 700, new DateTime(2022, 2, 25)),
            new Phone("Galaxy A52", "Samsung", 330, new DateTime(2021, 3, 26)),
            new Phone("Redmi Note 10", "Xiaomi", 170, new DateTime(2021, 3, 4)),
            new Phone("iPhone 10", "Apple", 280, new DateTime(2017, 11, 3))
        };

        public static void Menu()
        {
            while (true)
            {
                Console.WriteLine("\n-Завдання 2: телефони-" +
                    "\n1 - Кількість телефонів" +
                    "\n2 - Кількість телефонів із ціною більше 100" +
                    "\n3 - Кількість телефонів із ціною від 400 до 700" +
                    "\n4 - Кількість телефонів виробника" +
                    "\n5 - Телефон із мінімальною ціною" +
                    "\n6 - Телефон із максимальною ціною" +
                    "\n7 - Найстаріший телефон" +
                    "\n8 - Найсвіжіший телефон" +
                    "\n9 - Середня ціна телефону" +
                    "\n10 - П'ять найдорожчих телефонів" +
                    "\n11 - П'ять найдешевших телефонів" +
                    "\n12 - Три найстаріші телефони" +
                    "\n13 - Три найновіші телефони" +
                    "\n14 - Статистика за виробниками" +
                    "\n15 - Статистика за моделями" +
                    "\n16 - Статистика за роками" +
                    "\n0 - Назад");
                Console.Write("Ваш вибір: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Введіть число.");
                    continue;
                }

                switch (choice)
                {
                    case 1: PhoneCount(); break;
                    case 2: PriceOver100(); break;
                    case 3: Price400To700(); break;
                    case 4: ManufacturerCount(); break;
                    case 5: MinPrice(); break;
                    case 6: MaxPrice(); break;
                    case 7: OldestPhone(); break;
                    case 8: NewestPhone(); break;
                    case 9: AveragePrice(); break;
                    case 10: FiveMostExpensive(); break;
                    case 11: FiveCheapest(); break;
                    case 12: ThreeOldest(); break;
                    case 13: ThreeNewest(); break;
                    case 14: StatByManufacturer(); break;
                    case 15: StatByModel(); break;
                    case 16: StatByYear(); break;
                    case 0: return;
                    default: Console.WriteLine("Немає такого пункту."); break;
                }
            }
        }

        private static void PhoneCount()
        {
            Console.WriteLine("\n1. Загальна кількість телефонів: " + phones.Count());
        }

        private static void PriceOver100()
        {
            Console.WriteLine("\n2. Кількість телефонів із ціною більше 100: " +
                phones.Count(p => p.Price > 100));
        }

        private static void Price400To700()
        {
            Console.WriteLine("\n3. Кількість телефонів із ціною від 400 до 700: " +
                phones.Count(p => p.Price >= 400 && p.Price <= 700));
        }

        private static void ManufacturerCount()
        {
            string manufacturer = "Samsung";
            Console.WriteLine("\n4. Кількість телефонів виробника " + manufacturer + ": " +
                phones.Count(p => p.Manufacturer == manufacturer));
        }

        private static void MinPrice()
        {
            Console.WriteLine("\n5. Телефон із мінімальною ціною:");
            Console.WriteLine(phones.OrderBy(p => p.Price).First());
        }

        private static void MaxPrice()
        {
            Console.WriteLine("\n6. Телефон із максимальною ціною:");
            Console.WriteLine(phones.OrderByDescending(p => p.Price).First());
        }

        private static void OldestPhone()
        {
            Console.WriteLine("\n7. Найстаріший телефон:");
            Console.WriteLine(phones.OrderBy(p => p.ReleaseDate).First());
        }

        private static void NewestPhone()
        {
            Console.WriteLine("\n8. Найсвіжіший телефон:");
            Console.WriteLine(phones.OrderByDescending(p => p.ReleaseDate).First());
        }

        private static void AveragePrice()
        {
            Console.WriteLine("\n9. Середня ціна телефону: " +
                phones.Average(p => p.Price).ToString("F2"));
        }

        private static void FiveMostExpensive()
        {
            Console.WriteLine("\n10. П'ять найдорожчих телефонів:");
            Show(phones.OrderByDescending(p => p.Price).Take(5).ToList());
        }

        private static void FiveCheapest()
        {
            Console.WriteLine("\n11. П'ять найдешевших телефонів:");
            Show(phones.OrderBy(p => p.Price).Take(5).ToList());
        }

        private static void ThreeOldest()
        {
            Console.WriteLine("\n12. Три найстаріші телефони:");
            Show(phones.OrderBy(p => p.ReleaseDate).Take(3).ToList());
        }

        private static void ThreeNewest()
        {
            Console.WriteLine("\n13. Три найновіші телефони:");
            Show(phones.OrderByDescending(p => p.ReleaseDate).Take(3).ToList());
        }

        private static void StatByManufacturer()
        {
            Console.WriteLine("\n14. Кількість телефонів кожного виробника:");
            var manufacturerGroups = phones.GroupBy(p => p.Manufacturer);
            foreach (var group in manufacturerGroups)
            {
                Console.WriteLine(group.Key + " – " + group.Count());
            }
        }

        private static void StatByModel()
        {
            Console.WriteLine("\n15. Кількість моделей телефонів:");
            var modelGroups = phones.GroupBy(p => p.Name);
            foreach (var group in modelGroups)
            {
                Console.WriteLine(group.Key + " – " + group.Count());
            }
        }

        private static void StatByYear()
        {
            Console.WriteLine("\n16. Кількість телефонів за роками:");
            var yearGroups = phones.GroupBy(p => p.ReleaseDate.Year)
                                   .OrderBy(p => p.Key);
            foreach (var group in yearGroups)
            {
                Console.WriteLine(group.Key + " – " + group.Count());
            }
        }

        private static void Show(List<Phone> list)
        {
            foreach (Phone phone in list)
            {
                Console.WriteLine(phone);
            }
        }
    }
}
