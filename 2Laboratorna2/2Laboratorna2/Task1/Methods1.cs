using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab2.Task1
{
    public static class Methods1
    {
        private static List<Firm> firms = new List<Firm>()
        {
            new Firm("Food Market", DateTime.Today.AddYears(-5), "Торгівля", "John White", 120, "London"),
            new Firm("White House", DateTime.Today.AddYears(-1), "IT", "David Black", 80, "Kyiv"),
            new Firm("Bright Media", DateTime.Today.AddDays(-200), "Маркетинг", "Peter Brown", 250, "London"),
            new Firm("IT Solutions", DateTime.Today.AddYears(-3), "IT", "Michael White", 350, "Warsaw"),
            new Firm("Food Service", DateTime.Today.AddYears(-4), "Маркетинг", "James Green", 90, "Lviv"),
            new Firm("White Design", DateTime.Today.AddDays(-100), "Дизайн", "Robert Black", 150, "London"),
            new Firm("Market Plus", DateTime.Today.AddYears(-6), "Маркетинг", "William Smith", 500, "Odesa"),
            new Firm("Tech World", DateTime.Today.AddYears(-2).AddDays(-10), "IT", "Daniel Black", 220, "London")
        };

        public static void Menu()
        {
            while (true)
            {
                Console.WriteLine("\n-Завдання 1: фірми-" +
                    "\n1 - Інформація про всі фірми" +
                    "\n2 - Фірми з назвою Food" +
                    "\n3 - Фірми у галузі маркетингу" +
                    "\n4 - Фірми у галузі маркетингу або IT" +
                    "\n5 - Фірми з кількістю співробітників більше 100" +
                    "\n6 - Фірми з кількістю співробітників від 100 до 300" +
                    "\n7 - Фірми у Лондоні" +
                    "\n8 - Фірми з прізвищем директора White" +
                    "\n9 - Фірми, засновані понад два роки тому" +
                    "\n10 - Фірми, з дня заснування яких минуло більше 150 днів" +
                    "\n11 - Прізвище директора Black та назва містить White" +
                    "\n0 - Назад");
                Console.Write("Ваш вибір: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Введіть число.");
                    continue;
                }

                switch (choice)
                {
                    case 1: AllFirms(); break;
                    case 2: FoodFirms(); break;
                    case 3: MarketingFirms(); break;
                    case 4: MarketingOrItFirms(); break;
                    case 5: MoreThan100Employees(); break;
                    case 6: From100To300Employees(); break;
                    case 7: LondonFirms(); break;
                    case 8: DirectorWhite(); break;
                    case 9: FoundedOverTwoYearsAgo(); break;
                    case 10: FoundedOver150DaysAgo(); break;
                    case 11: DirectorBlackNameWhite(); break;
                    case 0: return;
                    default: Console.WriteLine("Немає такого пункту."); break;
                }
            }
        }

        private static void AllFirms()
        {
            Console.WriteLine("\n1. Інформація про всі фірми:");
            Show(firms);
        }

        private static void FoodFirms()
        {
            Console.WriteLine("\n2. Фірми з назвою Food:");
            Show(firms.Where(f => f.Name.Contains("Food")).ToList());
        }

        private static void MarketingFirms()
        {
            Console.WriteLine("\n3. Фірми, що працюють у галузі маркетингу:");
            Show(firms.Where(f => f.BusinessProfile == "Маркетинг").ToList());
        }

        private static void MarketingOrItFirms()
        {
            Console.WriteLine("\n4. Фірми у галузі маркетингу або IT:");
            Show(firms.Where(f => f.BusinessProfile == "Маркетинг" || f.BusinessProfile == "IT").ToList());
        }

        private static void MoreThan100Employees()
        {
            Console.WriteLine("\n5. Фірми, у яких більше 100 співробітників:");
            Show(firms.Where(f => f.EmployeeCount > 100).ToList());
        }

        private static void From100To300Employees()
        {
            Console.WriteLine("\n6. Фірми, у яких від 100 до 300 співробітників:");
            Show(firms.Where(f => f.EmployeeCount >= 100 && f.EmployeeCount <= 300).ToList());
        }

        private static void LondonFirms()
        {
            Console.WriteLine("\n7. Фірми, що знаходяться у Лондоні:");
            Show(firms.Where(f => f.Address.Contains("London")).ToList());
        }

        private static void DirectorWhite()
        {
            Console.WriteLine("\n8. Фірми, у яких прізвище директора White:");
            Show(firms.Where(f => f.DirectorFullName.Split(' ').Last() == "White").ToList());
        }

        private static void FoundedOverTwoYearsAgo()
        {
            Console.WriteLine("\n9. Фірми, засновані понад два роки тому:");
            Show(firms.Where(f => f.FoundationDate < DateTime.Today.AddYears(-2)).ToList());
        }

        private static void FoundedOver150DaysAgo()
        {
            Console.WriteLine("\n10. Фірми, з дня заснування яких минуло більше 150 днів:");
            Show(firms.Where(f => f.FoundationDate < DateTime.Today.AddDays(-150)).ToList());
        }

        private static void DirectorBlackNameWhite()
        {
            Console.WriteLine("\n11. Прізвище директора Black та назва містить White:");
            Show(firms.Where(f => f.DirectorFullName.Split(' ').Last() == "Black" && f.Name.Contains("White")).ToList());
        }

        private static void Show(List<Firm> list)
        {
            if (list.Count == 0)
            {
                Console.WriteLine("Записів не знайдено.");
                return;
            }

            foreach (Firm firm in list)
            {
                Console.WriteLine(firm);
            }
        }
    }
}
