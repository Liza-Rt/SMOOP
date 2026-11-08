using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab2.Task3
{
    public static class Methods3
    {
        private static Company company = CreateCompany();

        private static Company CreateCompany()
        {
            Company c = new Company("IT Company");

            c.Employers.Add(new President("Олександр Петренко", new DateTime(1975, 4, 12), 120000));
            c.Employers.Add(new Manager("Ірина Коваленко", new DateTime(1985, 7, 20), 65000, "Розробка"));
            c.Employers.Add(new Manager("Андрій Мельник", new DateTime(1992, 3, 15), 55000, "Маркетинг"));

            c.Employers.Add(new Worker("Володимир Іваненко", new DateTime(1998, 10, 5), 30000, 12, "Вища", "Програміст"));
            c.Employers.Add(new Worker("Петро Шевченко", new DateTime(1988, 2, 10), 28000, 18, "Вища", "Аналітик"));
            c.Employers.Add(new Worker("Володимир Бондар", new DateTime(2001, 10, 18), 24000, 5, "Середня", "Тестувальник"));
            c.Employers.Add(new Worker("Марія Ткаченко", new DateTime(1995, 10, 22), 32000, 10, "Вища", "Програміст"));
            c.Employers.Add(new Worker("Олег Кравець", new DateTime(1990, 6, 7), 26000, 15, "Середня", "Системний адміністратор"));
            c.Employers.Add(new Worker("Наталія Мороз", new DateTime(2000, 10, 11), 25000, 4, "Вища", "Аналітик"));
            c.Employers.Add(new Worker("Володимир Савчук", new DateTime(1982, 12, 3), 35000, 20, "Вища", "Програміст"));
            c.Employers.Add(new Worker("Тарас Лисенко", new DateTime(1999, 1, 29), 23000, 6, "Фахова передвища", "Тестувальник"));
            c.Employers.Add(new Worker("Світлана Романюк", new DateTime(1993, 10, 2), 31000, 14, "Вища", "Аналітик"));
            c.Employers.Add(new Worker("Дмитро Павленко", new DateTime(1987, 8, 14), 33000, 17, "Вища", "Програміст"));
            c.Employers.Add(new Worker("Володимир Климчук", new DateTime(2003, 5, 9), 21000, 2, "Середня", "Тестувальник"));

            return c;
        }

        public static void Menu()
        {
            while (true)
            {
                Console.WriteLine("\n-Завдання 3: " + company.Name + "-" +
                    "\n1 - Кількість робітників" +
                    "\n2 - Загальна сума заробітної плати робітників" +
                    "\n3 - 10 робітників із найбільшим стажем, найменший за віком з вищою освітою" +
                    "\n4 - Наймолодший та найстарший менеджер" +
                    "\n5 - Робітники, народжені у жовтні (за професіями)" +
                    "\n6 - Усі Володимири та премія наймолодшому" +
                    "\n0 - Назад");
                Console.Write("Ваш вибір: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Введіть число.");
                    continue;
                }

                switch (choice)
                {
                    case 1: WorkerCount(); break;
                    case 2: WorkerSalaryTotal(); break;
                    case 3: ExperiencedWorkers(); break;
                    case 4: Managers(); break;
                    case 5: OctoberWorkers(); break;
                    case 6: Vladimirs(); break;
                    case 0: return;
                    default: Console.WriteLine("Немає такого пункту."); break;
                }
            }
        }

        private static void WorkerCount()
        {
            Console.WriteLine("\n1. Кількість робітників: " + company.GetWorkerCount());
        }

        private static void WorkerSalaryTotal()
        {
            Console.WriteLine("\n2. Загальна сума заробітної плати робітників: " +
                              company.GetWorkerSalaryTotal());
        }

        private static void ExperiencedWorkers()
        {
            Console.WriteLine("\n3. Десять робітників із найбільшим стажем:");

            List<Worker> experiencedWorkers = company.Employers.OfType<Worker>()
                .OrderByDescending(w => w.ExperienceYears)
                .Take(10)
                .ToList();

            ShowWorkers(experiencedWorkers);

            Worker youngestEducatedWorker = experiencedWorkers.Where(w => w.Education == "Вища").OrderByDescending(w => w.BirthDate).FirstOrDefault();

            Console.WriteLine("\nНаймолодший із цією десяткою, хто має вищу освіту:");

            if (youngestEducatedWorker != null)
            {
                ShowWorker(youngestEducatedWorker);
            }
            else
            {
                Console.WriteLine("Серед вибраних робітників немає працівника з вищою освітою.");
            }
        }

        private static void Managers()
        {
            Console.WriteLine("\n4. Наймолодший менеджер:");

            Manager youngestManager = company.Employers.OfType<Manager>().OrderByDescending(m => m.BirthDate).FirstOrDefault();

            ShowEmployer(youngestManager);

            Console.WriteLine("\nНайстарший менеджер:");

            Manager oldestManager = company.Employers.OfType<Manager>().OrderBy(m => m.BirthDate).FirstOrDefault();

            ShowEmployer(oldestManager);
        }

        private static void OctoberWorkers()
        {
            Console.WriteLine("\n5. Робітники, які народилися у жовтні (за професіями):");

            var octoberWorkers = company.Employers.OfType<Worker>().Where(w => w.BirthDate.Month == 10).GroupBy(w => w.Profession);

            foreach (var group in octoberWorkers)
            {
                Console.WriteLine("\nПрофесія: " + group.Key);

                foreach (Worker worker in group)
                {
                    ShowWorker(worker);
                }
            }
        }

        private static void Vladimirs()
        {
            Console.WriteLine("\n6. Усі Володимири, які працюють на підприємстві:");

            List<Employer> vladimirs = company.Employers.Where(e => e.FullName.Split(' ')[0] == "Володимир").ToList();

            foreach (Employer employer in vladimirs)
            {
                ShowEmployer(employer);
            }

            Employer youngestVladimir = vladimirs.OrderByDescending(e => e.BirthDate).FirstOrDefault();

            Console.WriteLine("\nНаймолодший Володимир отримує премію:");

            if (youngestVladimir != null)
            {
                decimal bonus = youngestVladimir.Salary / 3;

                Console.WriteLine("Працівник: " + youngestVladimir.FullName);
                Console.WriteLine("Посадовий оклад: " + youngestVladimir.Salary);
                Console.WriteLine("Премія (1/3 окладу): " + bonus);
                Console.WriteLine("Сума з премією: " + (youngestVladimir.Salary + bonus));
            }
            else
            {
                Console.WriteLine("Володимирів не знайдено.");
            }
        }

        private static void ShowWorkers(List<Worker> workers)
        {
            foreach (Worker worker in workers)
            {
                ShowWorker(worker);
            }
        }

        private static void ShowWorker(Worker worker)
        {
            Console.WriteLine("ПІБ: " + worker.FullName +
                              ", вік: " + worker.Age +
                              ", стаж: " + worker.ExperienceYears +
                              ", освіта: " + worker.Education +
                              ", професія: " + worker.Profession +
                              ", зарплата: " + worker.Salary);
        }

        private static void ShowEmployer(Employer employer)
        {
            if (employer != null)
            {
                Console.WriteLine("ПІБ: " + employer.FullName +
                                  ", вік: " + employer.Age +
                                  ", зарплата: " + employer.Salary);
            }
            else
            {
                Console.WriteLine("Працівників не знайдено.");
            }
        }
    }
}
