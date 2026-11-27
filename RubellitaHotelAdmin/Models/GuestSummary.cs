using System;

namespace RubellitaHotelAdmin.Models
{
    /// <summary>Зведені дані про гостя для вкладки «Гості та знижки».</summary>
    public class GuestSummary
    {
        public string GuestFullName { get; set; } = string.Empty;
        public string PhoneDisplay { get; set; } = string.Empty;
        public string PassportOrId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int StaysCount { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime LastCheckIn { get; set; }
        public DateTime LastCheckOut { get; set; }
        public string NextDiscountInfo => (StaysCount + 1) % 5 == 0
            ? "Наступне заселення — 5-те! Знижка 20%"
            : $"До знижки залишилось: {5 - ((StaysCount) % 5)} зас.";
    }
}
