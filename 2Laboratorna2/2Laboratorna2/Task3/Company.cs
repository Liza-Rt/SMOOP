using System.Collections.Generic;
using System.Linq;

namespace Lab2.Task3
{
    public class Company
    {
        public string Name { get; set; }
        public List<Employer> Employers { get; set; }

        public Company(string name)
        {
            Name = name;
            Employers = new List<Employer>();
        }

        public int GetWorkerCount()
        {
            return Employers.OfType<Worker>().Count();
        }

        public decimal GetWorkerSalaryTotal()
        {
            return Employers.OfType<Worker>().Sum(w => w.Salary);
        }
    }
}
