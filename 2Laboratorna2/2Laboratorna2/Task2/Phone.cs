using System;

namespace Lab2.Task2
{
    public class Phone
    {
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public decimal Price { get; set; }
        public DateTime ReleaseDate { get; set; }

        public Phone(string name, string manufacturer, decimal price, DateTime releaseDate)
        {
            Name = name;
            Manufacturer = manufacturer;
            Price = price;
            ReleaseDate = releaseDate;
        }

        public override string ToString()
        {
            return "Модель: " + Name +
                   "\nВиробник: " + Manufacturer +
                   "\nЦіна: " + Price +
                   "\nДата випуску: " + ReleaseDate.ToShortDateString() + "\n";
        }
    }
}
