using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using RubellitaHotelAdmin.Helpers;

namespace RubellitaHotelAdmin.Models
{
    /// <summary>Бронювання номера: гість, контакти, дати, ціна та стан.</summary>
    public class Booking : INotifyPropertyChanged
    {
        private Guid _id = Guid.NewGuid();
        private int _roomNumber;
        private string _guestFullName = string.Empty;
        private string _countryShortCode = "UA";
        private string _dialCode = "+380";
        private string _phoneNumber = string.Empty;
        private string _passportOrId = string.Empty;
        private string _email = string.Empty;
        private string _guestNotes = string.Empty;
        private DateTime _checkInDate = DateTime.Today;
        private string _checkInTime = "14:00";
        private DateTime _checkOutDate = DateTime.Today.AddDays(1);
        private string _checkOutTime = "12:00";
        private decimal _basePricePerNight;
        private decimal _discountPercent;
        private decimal _totalCost;
        private int _visitNumber = 1;
        private bool _isActive = true;
        private string _bookingState = "Активне (Заселений)";

        public Guid Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        public int RoomNumber
        {
            get => _roomNumber;
            set { _roomNumber = value; OnPropertyChanged(); }
        }

        public string GuestFullName
        {
            get => _guestFullName;
            set { _guestFullName = value; OnPropertyChanged(); }
        }

        public string CountryShortCode
        {
            get => _countryShortCode;
            set { _countryShortCode = value; OnPropertyChanged(); OnPropertyChanged(nameof(FormattedPhone)); OnPropertyChanged(nameof(FullPhoneDisplay)); }
        }

        public string DialCode
        {
            get => _dialCode;
            set { _dialCode = value; OnPropertyChanged(); OnPropertyChanged(nameof(FullPhoneDisplay)); }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set { _phoneNumber = value; OnPropertyChanged(); OnPropertyChanged(nameof(FormattedPhone)); OnPropertyChanged(nameof(FullPhoneDisplay)); }
        }

        // Номер з тире за шаблоном країни (працює і для старих записів, де цифри збережені без тире)
        public string FormattedPhone => PhoneFormatter.Format(PhoneNumber, CountryPhoneCatalog.FindMask(CountryShortCode));

        public string FullPhoneDisplay => $"[{CountryShortCode}] {DialCode} {FormattedPhone}";

        public string PassportOrId
        {
            get => _passportOrId;
            set { _passportOrId = value; OnPropertyChanged(); }
        }

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        public string GuestNotes
        {
            get => _guestNotes;
            set { _guestNotes = value; OnPropertyChanged(); }
        }

        public DateTime CheckInDate
        {
            get => _checkInDate;
            set
            {
                _checkInDate = value.Date;
                OnPropertyChanged();
                OnPropertyChanged(nameof(NightsCount));
                OnPropertyChanged(nameof(CheckInDateTimeDisplay));
            }
        }

        public string CheckInTime
        {
            get => _checkInTime;
            set
            {
                _checkInTime = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CheckInDateTimeDisplay));
            }
        }

        public string CheckInDateTimeDisplay => $"{CheckInDate:dd.MM.yyyy} {CheckInTime}";

        public DateTime CheckOutDate
        {
            get => _checkOutDate;
            set
            {
                _checkOutDate = value.Date;
                OnPropertyChanged();
                OnPropertyChanged(nameof(NightsCount));
                OnPropertyChanged(nameof(CheckOutDateTimeDisplay));
            }
        }

        public string CheckOutTime
        {
            get => _checkOutTime;
            set
            {
                _checkOutTime = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CheckOutDateTimeDisplay));
            }
        }

        public string CheckOutDateTimeDisplay => $"{CheckOutDate:dd.MM.yyyy} {CheckOutTime}";

        public int NightsCount => Math.Max(1, (CheckOutDate.Date - CheckInDate.Date).Days);

        public decimal BasePricePerNight
        {
            get => _basePricePerNight;
            set { _basePricePerNight = value; OnPropertyChanged(); }
        }

        public decimal DiscountPercent
        {
            get => _discountPercent;
            set { _discountPercent = value; OnPropertyChanged(); OnPropertyChanged(nameof(DiscountLabel)); }
        }

        // Відсоток знижки для відображення у списку
        public string DiscountLabel => DiscountPercent > 0 ? $"-{DiscountPercent:0}%" : "0%";

        public decimal TotalCost
        {
            get => _totalCost;
            set { _totalCost = value; OnPropertyChanged(); }
        }

        public int VisitNumber
        {
            get => _visitNumber;
            set { _visitNumber = value; OnPropertyChanged(); }
        }

        public bool IsActive
        {
            get => _isActive;
            set { _isActive = value; OnPropertyChanged(); }
        }

        public string BookingState
        {
            get => _bookingState;
            set { _bookingState = value; OnPropertyChanged(); }
        }

        public DateTime GetCheckInDateTime()
        {
            if (TimeSpan.TryParse(CheckInTime, out var ts))
                return CheckInDate.Date.Add(ts);
            return CheckInDate.Date.AddHours(14);
        }

        public DateTime GetCheckOutDateTime()
        {
            if (TimeSpan.TryParse(CheckOutTime, out var ts))
                return CheckOutDate.Date.Add(ts);
            return CheckOutDate.Date.AddHours(12);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
