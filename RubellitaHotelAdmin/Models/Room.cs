using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RubellitaHotelAdmin.Models
{
    public static class RoomConstants
    {
        public const string StatusFree = "вільний";
        public const string StatusBooked = "заброньований";
        public const string StatusOccupied = "зайнятий";
    }

    /// <summary>Номер готелю: поверх, тип, місткість, ціна та статус.</summary>
    public class Room : INotifyPropertyChanged
    {
        private int _roomNumber;
        private int _floor;
        private string _type = "Стандарт (Standard)";
        private int _capacity = 2;
        private decimal _pricePerNight;
        private string _status = RoomConstants.StatusFree;
        private bool _isCorner;
        private string _wing = "Ліве крило";
        private string _description = string.Empty;

        public int RoomNumber
        {
            get => _roomNumber;
            set { _roomNumber = value; OnPropertyChanged(); }
        }

        public int Floor
        {
            get => _floor;
            set { _floor = value; OnPropertyChanged(); }
        }

        public string Type
        {
            get => _type;
            set { _type = value; OnPropertyChanged(); }
        }

        public int Capacity
        {
            get => _capacity;
            set { _capacity = value; OnPropertyChanged(); OnPropertyChanged(nameof(CapacityLabel)); }
        }

        public string CapacityLabel => $"{Capacity}-місний";

        public decimal PricePerNight
        {
            get => _pricePerNight;
            set { _pricePerNight = value; OnPropertyChanged(); }
        }

        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        public bool IsCorner
        {
            get => _isCorner;
            set { _isCorner = value; OnPropertyChanged(); OnPropertyChanged(nameof(CornerLabel)); }
        }

        public string CornerLabel => IsCorner ? "Кутовий" : "Звичайний";
        public bool IsMapSelected => false;

        public string Wing
        {
            get => _wing;
            set { _wing = value; OnPropertyChanged(); }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string? prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
