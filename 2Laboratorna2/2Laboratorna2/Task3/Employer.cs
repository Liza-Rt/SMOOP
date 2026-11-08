using System;

namespace Lab2.Task3
{
    public class Employer
    {
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public decimal Salary { get; set; }

        public Employer(string fullName, DateTime birthDate, decimal salary)
        {
            FullName = fullName;
            BirthDate = birthDate;
            Salary = salary;
        }

        public int Age
        {
            get
            {
                int age = DateTime.Today.Year - BirthDate.Year;
                if (BirthDate.Date > DateTime.Today.AddYears(-age))
                {
                    age--;
                }
                return age;
            }
        }
    }
}
