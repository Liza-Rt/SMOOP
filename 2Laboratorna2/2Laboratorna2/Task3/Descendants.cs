using System;

namespace Lab2.Task3
{
    public class President : Employer
    {
        public President(string fullName, DateTime birthDate, decimal salary)
            : base(fullName, birthDate, salary)
        {
        }
    }

    public class Manager : Employer
    {
        public string Department { get; set; }

        public Manager(string fullName, DateTime birthDate, decimal salary, string department)
            : base(fullName, birthDate, salary)
        {
            Department = department;
        }
    }

    public class Worker : Employer
    {
        public int ExperienceYears { get; set; }
        public string Education { get; set; }
        public string Profession { get; set; }

        public Worker(string fullName, DateTime birthDate, decimal salary,
                      int experienceYears, string education, string profession)
            : base(fullName, birthDate, salary)
        {
            ExperienceYears = experienceYears;
            Education = education;
            Profession = profession;
        }
    }
}
