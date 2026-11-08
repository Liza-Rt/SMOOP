using System;

namespace Lab2.Task1
{
    public class Firm
    {
        public string Name { get; set; }
        public DateTime FoundationDate { get; set; }
        public string BusinessProfile { get; set; }
        public string DirectorFullName { get; set; }
        public int EmployeeCount { get; set; }
        public string Address { get; set; }

        public Firm(string name, DateTime foundationDate, string businessProfile,
                    string directorFullName, int employeeCount, string address)
        {
            Name = name;
            FoundationDate = foundationDate;
            BusinessProfile = businessProfile;
            DirectorFullName = directorFullName;
            EmployeeCount = employeeCount;
            Address = address;
        }

        public override string ToString()
        {
            return "Назва: " + Name +
                   "\nДата заснування: " + FoundationDate.ToShortDateString() +
                   "\nПрофіль: " + BusinessProfile +
                   "\nДиректор: " + DirectorFullName +
                   "\nКількість співробітників: " + EmployeeCount +
                   "\nАдреса: " + Address + "\n";
        }
    }
}
