using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using RubellitaHotelAdmin.Helpers;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using RubellitaHotelAdmin.Models;
using RubellitaHotelAdmin.Services;

namespace RubellitaHotelAdmin.ViewModels
{
    /// <summary>Головна ViewModel: номери, бронювання, гості та статистика.</summary>
    public class MainViewModel : BaseViewModel
    {
        private readonly DataStorageService _storage;

        public AdminUser CurrentAdmin { get; }

        // Колекції даних
        public ObservableCollection<Room> Rooms { get; } = new ObservableCollection<Room>();
        public ObservableCollection<Booking> Bookings { get; } = new ObservableCollection<Booking>();
        public ObservableCollection<CountryPhoneCode> CountryCodes { get; } = new ObservableCollection<CountryPhoneCode>();
        public ObservableCollection<GuestSummary> GuestSummaries { get; } = new ObservableCollection<GuestSummary>();

        public ICollectionView FilteredRoomsView { get; }
        public ICollectionView FilteredBookingsView { get; }

        // Прив'язка між вкладками (SelectedTabIndex: 0 - План поверхів, 1 - Номерний фонд, 2 - Бронювання, 3 - Гості та знижки, 4 - Статистика)
        private int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set
            {
                _selectedTabIndex = value;
                OnPropertyChanged();
            }
        }

        // Поверхи для візуального плану
        public ObservableCollection<int> AvailableFloors { get; } = new ObservableCollection<int> { 1, 2, 3 };
        private int _selectedFloor = 1;
        public int SelectedFloor
        {
            get => _selectedFloor;
            set
            {
                _selectedFloor = value;
                OnPropertyChanged();
                RefreshFloorMap();
            }
        }

        public ObservableCollection<Room> FloorLeftWingRooms { get; } = new ObservableCollection<Room>();
        public ObservableCollection<Room> FloorRightWingRooms { get; } = new ObservableCollection<Room>();

        private Room? _selectedMapRoom;
        public Room? SelectedMapRoom
        {
            get => _selectedMapRoom;
            set
            {
                _selectedMapRoom = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedMapRoomBooking));
                foreach (var r in Rooms)
                {
                    r.OnPropertyChanged(nameof(Room.IsMapSelected));
                }
                if (value != null)
                {
                    SelectedRoom = value;
                    BookingRoomNumber = value.RoomNumber;
                }
            }
        }

        public Booking? SelectedMapRoomBooking
        {
            get
            {
                if (SelectedMapRoom == null) return null;
                return Bookings
                    .Where(b => b.RoomNumber == SelectedMapRoom.RoomNumber && b.IsActive)
                    .OrderBy(b => b.GetCheckInDateTime())
                    .FirstOrDefault();
            }
        }

        // Фільтри та пошук номерів
        private string _roomSearchText = string.Empty;
        public string RoomSearchText
        {
            get => _roomSearchText;
            set { _roomSearchText = value; OnPropertyChanged(); FilteredRoomsView.Refresh(); }
        }

        private string _filterStatus = "Усі";
        public string FilterStatus
        {
            get => _filterStatus;
            set { _filterStatus = value; OnPropertyChanged(); FilteredRoomsView.Refresh(); }
        }

        private string _filterType = "Усі";
        public string FilterType
        {
            get => _filterType;
            set { _filterType = value; OnPropertyChanged(); FilteredRoomsView.Refresh(); }
        }

        private string _filterCapacity = "Усі";
        public string FilterCapacity
        {
            get => _filterCapacity;
            set { _filterCapacity = value; OnPropertyChanged(); FilteredRoomsView.Refresh(); }
        }

        private string _filterMaxPrice = string.Empty;
        public string FilterMaxPrice
        {
            get => _filterMaxPrice;
            set { _filterMaxPrice = value; OnPropertyChanged(); FilteredRoomsView.Refresh(); }
        }

        private string _roomSortOption = "За номером (зростання)";
        public string RoomSortOption
        {
            get => _roomSortOption;
            set { _roomSortOption = value; OnPropertyChanged(); ApplyRoomSorting(); }
        }

        public List<string> StatusFilterOptions { get; } = new List<string> { "Усі", RoomConstants.StatusFree, RoomConstants.StatusBooked, RoomConstants.StatusOccupied };
        public List<string> TypeFilterOptions { get; } = new List<string> { "Усі", "Стандарт (Standard)", "Покращений (Superior)", "Напівлюкс (Junior Suite)", "Люкс (Suite)", "Сімейний (Family)", "Президентський (Presidential)" };
        public List<string> CapacityFilterOptions { get; } = new List<string> { "Усі", "1-місний", "2-місний", "3-місний", "4-місний" };
        public List<string> RoomSortOptions { get; } = new List<string>
        {
            "За номером (зростання)",
            "За ціною (від дешевших)",
            "За ціною (від дорожчих)",
            "За місткістю (зростання)",
            "За місткістю (спадання)"
        };

        // Обраний номер (лише перегляд)
        private Room? _selectedRoom;
        public Room? SelectedRoom
        {
            get => _selectedRoom;
            set { _selectedRoom = value; OnPropertyChanged(); }
        }

        // Форма бронювання та гостя
        private Booking? _selectedBooking;
        public Booking? SelectedBooking
        {
            get => _selectedBooking;
            set
            {
                _selectedBooking = value;
                OnPropertyChanged();
                if (value != null)
                {
                    BookingRoomNumber = value.RoomNumber;
                    GuestFullName = value.GuestFullName;
                    SelectedCountryCode = CountryCodes.FirstOrDefault(c => c.ShortCode == value.CountryShortCode) ?? CountryCodes.FirstOrDefault(c => c.DialCode == value.DialCode) ?? CountryCodes.First();
                    GuestPhoneNumber = value.PhoneNumber;
                    GuestPassport = value.PassportOrId;
                    GuestEmail = value.Email;
                    GuestNotes = value.GuestNotes;
                    CheckInDate = value.CheckInDate;
                    SelectedCheckInTime = value.CheckInTime;
                    CheckOutDate = value.CheckOutDate;
                    SelectedCheckOutTime = value.CheckOutTime;
                    SelectedBookingState = value.BookingState;
                    RecalculateBookingPrice();
                }
            }
        }

        private int _bookingRoomNumber = 101;
        public int BookingRoomNumber
        {
            get => _bookingRoomNumber;
            set { _bookingRoomNumber = value; OnPropertyChanged(); RecalculateBookingPrice(); }
        }

        private string _guestFullName = string.Empty;
        public string GuestFullName
        {
            get => _guestFullName;
            set { _guestFullName = value; OnPropertyChanged(); RecalculateBookingPrice(); }
        }

        private CountryPhoneCode _selectedCountryCode = new CountryPhoneCode();
        public CountryPhoneCode SelectedCountryCode
        {
            get => _selectedCountryCode;
            set
            {
                _selectedCountryCode = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PhoneWatermarkText));
                OnPropertyChanged(nameof(MaxPhoneDigits));
            }
        }

        public string PhoneWatermarkText => SelectedCountryCode?.MaskHint ?? "00-000-00-00";
        public int MaxPhoneDigits => SelectedCountryCode?.ExpectedDigits ?? 9;

        private string _guestPhoneNumber = string.Empty;
        public string GuestPhoneNumber
        {
            get => _guestPhoneNumber;
            set { _guestPhoneNumber = value; OnPropertyChanged(); OnPropertyChanged(nameof(ShowPhonePlaceholder)); }
        }

        public bool ShowPhonePlaceholder => string.IsNullOrEmpty(GuestPhoneNumber);

        private string _guestPassport = string.Empty;
        public string GuestPassport { get => _guestPassport; set { _guestPassport = value; OnPropertyChanged(); } }

        private string _guestEmail = string.Empty;
        public string GuestEmail { get => _guestEmail; set { _guestEmail = value; OnPropertyChanged(); } }

        private string _guestNotes = string.Empty;
        public string GuestNotes { get => _guestNotes; set { _guestNotes = value; OnPropertyChanged(); } }

        private DateTime _checkInDate = DateTime.Today;
        public DateTime CheckInDate
        {
            get => _checkInDate;
            set { _checkInDate = value.Date; OnPropertyChanged(); RecalculateBookingPrice(); }
        }

        private string _selectedCheckInTime = "14:00";
        public string SelectedCheckInTime
        {
            get => _selectedCheckInTime;
            set { _selectedCheckInTime = value; OnPropertyChanged(); }
        }

        private DateTime _checkOutDate = DateTime.Today.AddDays(2);
        public DateTime CheckOutDate
        {
            get => _checkOutDate;
            set { _checkOutDate = value.Date; OnPropertyChanged(); RecalculateBookingPrice(); }
        }

        private string _selectedCheckOutTime = "12:00";
        public string SelectedCheckOutTime
        {
            get => _selectedCheckOutTime;
            set { _selectedCheckOutTime = value; OnPropertyChanged(); }
        }

        public List<string> AvailableTimeSlots { get; } = new List<string>
        {
            "06:00", "07:00", "08:00", "09:00", "10:00", "11:00",
            "12:00", "13:00", "14:00", "15:00", "16:00", "17:00",
            "18:00", "19:00", "20:00", "21:00", "22:00", "23:00"
        };

        private string _selectedBookingState = "Активне (Заселений)";
        public string SelectedBookingState
        {
            get => _selectedBookingState;
            set { _selectedBookingState = value; OnPropertyChanged(); }
        }

        public List<string> BookingStatesList { get; } = new List<string>
        {
            "Заброньовано",
            "Активне (Заселений)",
            "Завершено",
            "Скасовано"
        };

        // Автоматичний розрахунок ціни та знижки за кожне 5-те заселення
        private int _calculatedNights;
        public int CalculatedNights { get => _calculatedNights; set { _calculatedNights = value; OnPropertyChanged(); } }

        private decimal _calculatedDiscountPercent;
        public decimal CalculatedDiscountPercent { get => _calculatedDiscountPercent; set { _calculatedDiscountPercent = value; OnPropertyChanged(); } }

        private decimal _calculatedTotalPrice;
        public decimal CalculatedTotalPrice { get => _calculatedTotalPrice; set { _calculatedTotalPrice = value; OnPropertyChanged(); } }

        private string _loyaltyStatusMessage = string.Empty;
        public string LoyaltyStatusMessage { get => _loyaltyStatusMessage; set { _loyaltyStatusMessage = value; OnPropertyChanged(); } }

        // Пошук бронювань
        private string _bookingSearchText = string.Empty;
        public string BookingSearchText
        {
            get => _bookingSearchText;
            set { _bookingSearchText = value; OnPropertyChanged(); FilteredBookingsView.Refresh(); }
        }

        // Пошук гостей (історія заїздів/виїздів та частота)
        private string _guestSearchQuery = string.Empty;
        public string GuestSearchQuery
        {
            get => _guestSearchQuery;
            set { _guestSearchQuery = value; OnPropertyChanged(); RefreshGuestSummaries(); }
        }

        // Статистика за період (включно з завершеними бронюваннями)
        private DateTime _statsStartDate = DateTime.Today.AddDays(-60);
        public DateTime StatsStartDate
        {
            get => _statsStartDate;
            set { _statsStartDate = value.Date; OnPropertyChanged(); RefreshStatistics(); }
        }

        private DateTime _statsEndDate = DateTime.Today.AddDays(30);
        public DateTime StatsEndDate
        {
            get => _statsEndDate;
            set { _statsEndDate = value.Date; OnPropertyChanged(); RefreshStatistics(); }
        }

        private string _statsStateFilter = "Усі (включно із завершеними)";
        public string StatsStateFilter
        {
            get => _statsStateFilter;
            set { _statsStateFilter = value; OnPropertyChanged(); RefreshStatistics(); }
        }

        public List<string> StatsStateFilterOptions { get; } = new List<string>
        {
            "Усі (включно із завершеними)",
            "Тільки завершені",
            "Активні та заброньовані"
        };

        private int _totalRoomsCount;
        public int TotalRoomsCount { get => _totalRoomsCount; set { _totalRoomsCount = value; OnPropertyChanged(); } }

        private int _freeRoomsCount;
        public int FreeRoomsCount { get => _freeRoomsCount; set { _freeRoomsCount = value; OnPropertyChanged(); } }

        private int _bookedRoomsCount;
        public int BookedRoomsCount { get => _bookedRoomsCount; set { _bookedRoomsCount = value; OnPropertyChanged(); } }

        private int _occupiedRoomsCount;
        public int OccupiedRoomsCount { get => _occupiedRoomsCount; set { _occupiedRoomsCount = value; OnPropertyChanged(); } }

        private double _occupancyRatePercent;
        public double OccupancyRatePercent { get => _occupancyRatePercent; set { _occupancyRatePercent = value; OnPropertyChanged(); } }

        private int _activeBookingsCount;
        public int ActiveBookingsCount { get => _activeBookingsCount; set { _activeBookingsCount = value; OnPropertyChanged(); } }

        private int _completedBookingsCount;
        public int CompletedBookingsCount { get => _completedBookingsCount; set { _completedBookingsCount = value; OnPropertyChanged(); } }

        private decimal _periodRevenue;
        public decimal PeriodRevenue { get => _periodRevenue; set { _periodRevenue = value; OnPropertyChanged(); } }

        public ObservableCollection<Booking> PeriodMovementBookings { get; } = new ObservableCollection<Booking>();

        // Команди (створюються при першому зверненні, як у прикладі carMVVM_Command)
        private RelayCommand? _clearRoomFiltersCommand;
        public RelayCommand ClearRoomFiltersCommand
        {
            get
            {
                return _clearRoomFiltersCommand ?? (_clearRoomFiltersCommand = new RelayCommand(obj =>
                {
                    ClearRoomFilters();
                }));
            }
        }

        private RelayCommand? _createBookingCommand;
        public RelayCommand CreateBookingCommand
        {
            get
            {
                return _createBookingCommand ?? (_createBookingCommand = new RelayCommand(obj =>
                {
                    CreateBooking();
                }));
            }
        }

        private RelayCommand? _updateBookingCommand;
        public RelayCommand UpdateBookingCommand
        {
            get
            {
                return _updateBookingCommand ?? (_updateBookingCommand = new RelayCommand(obj =>
                {
                    UpdateBooking();
                }));
            }
        }

        private RelayCommand? _completeBookingCommand;
        public RelayCommand CompleteBookingCommand
        {
            get
            {
                return _completeBookingCommand ?? (_completeBookingCommand = new RelayCommand(obj =>
                {
                    CompleteBooking();
                }));
            }
        }

        private RelayCommand? _cancelBookingCommand;
        public RelayCommand CancelBookingCommand
        {
            get
            {
                return _cancelBookingCommand ?? (_cancelBookingCommand = new RelayCommand(obj =>
                {
                    CancelBooking();
                }));
            }
        }

        private RelayCommand? _clearBookingFormCommand;
        public RelayCommand ClearBookingFormCommand
        {
            get
            {
                return _clearBookingFormCommand ?? (_clearBookingFormCommand = new RelayCommand(obj =>
                {
                    ClearBookingForm();
                }));
            }
        }

        // Вибір номера на плані поверху (номер приходить параметром з кнопки)
        private RelayCommand? _selectMapRoomCommand;
        public RelayCommand SelectMapRoomCommand
        {
            get
            {
                return _selectMapRoomCommand ?? (_selectMapRoomCommand = new RelayCommand(obj =>
                {
                    Room? room = obj as Room;
                    if (room != null)
                    {
                        SelectedMapRoom = room;
                    }
                }));
            }
        }

        // Перехід на вкладку «Бронювання» з обраним номером
        private RelayCommand? _bookSelectedMapRoomCommand;
        public RelayCommand BookSelectedMapRoomCommand
        {
            get
            {
                return _bookSelectedMapRoomCommand ?? (_bookSelectedMapRoomCommand = new RelayCommand(obj =>
                {
                    if (SelectedMapRoom != null)
                    {
                        BookingRoomNumber = SelectedMapRoom.RoomNumber;
                        ClearBookingForm(keepRoom: true);
                        SelectedTabIndex = 2;
                    }
                }));
            }
        }

        // Перехід на вкладку «Бронювання» з даними обраного гостя
        private RelayCommand? _bookForSelectedGuestCommand;
        public RelayCommand BookForSelectedGuestCommand
        {
            get
            {
                return _bookForSelectedGuestCommand ?? (_bookForSelectedGuestCommand = new RelayCommand(obj =>
                {
                    GuestSummary? guest = obj as GuestSummary;
                    if (guest != null)
                    {
                        GuestFullName = guest.GuestFullName;
                        GuestPassport = guest.PassportOrId;
                        GuestEmail = guest.Email;
                        SelectedTabIndex = 2;
                    }
                }));
            }
        }

        public MainViewModel(AdminUser admin)
        {
            CurrentAdmin = admin;
            _storage = new DataStorageService();

            InitCountryCodes();
            SelectedCountryCode = CountryCodes.First();

            foreach (var r in _storage.LoadRooms()) Rooms.Add(r);
            foreach (var b in _storage.LoadBookings()) Bookings.Add(b);

            FilteredRoomsView = CollectionViewSource.GetDefaultView(Rooms);
            FilteredRoomsView.Filter = FilterRoomPredicate;

            FilteredBookingsView = CollectionViewSource.GetDefaultView(Bookings);
            FilteredBookingsView.Filter = FilterBookingPredicate;
            FilteredBookingsView.SortDescriptions.Add(new SortDescription(nameof(Booking.CheckInDate), ListSortDirection.Descending));

            SelectedRoom = Rooms.FirstOrDefault();
            SelectedMapRoom = Rooms.FirstOrDefault(r => r.Floor == 1);
            BookingRoomNumber = Rooms.FirstOrDefault()?.RoomNumber ?? 101;

            RefreshFloorMap();
            RefreshGuestSummaries();
            RefreshStatistics();
            RecalculateBookingPrice();
        }

        /// <summary>Заповнює список країн для вибору коду телефону.</summary>
        private void InitCountryCodes()
        {
            foreach (var country in CountryPhoneCatalog.All)
                CountryCodes.Add(country);
        }

        // План поверхів
        /// <summary>Оновлює план поверху: розкладає номери обраного поверху за крилами.</summary>
        public void RefreshFloorMap()
        {
            FloorLeftWingRooms.Clear();
            FloorRightWingRooms.Clear();

            var floorRooms = Rooms.Where(r => r.Floor == SelectedFloor).OrderBy(r => r.RoomNumber).ToList();
            foreach (var room in floorRooms)
            {
                if (room.Wing == "Праве крило")
                    FloorRightWingRooms.Add(room);
                else
                    FloorLeftWingRooms.Add(room);
            }

            if (SelectedMapRoom == null || SelectedMapRoom.Floor != SelectedFloor)
            {
                SelectedMapRoom = floorRooms.FirstOrDefault();
            }
            OnPropertyChanged(nameof(SelectedMapRoomBooking));
        }

        // Фільтрація та сортування номерів
        /// <summary>Перевіряє, чи проходить номер через пошук і фільтри.</summary>
        private bool FilterRoomPredicate(object obj)
        {
            Room? room = obj as Room;
            if (room == null) return false;

            if (!string.IsNullOrWhiteSpace(RoomSearchText))
            {
                var q = RoomSearchText.Trim().ToLowerInvariant();
                bool match = room.RoomNumber.ToString().Contains(q)
                             || room.Type.ToLowerInvariant().Contains(q)
                             || room.Status.ToLowerInvariant().Contains(q)
                             || room.Description.ToLowerInvariant().Contains(q);
                if (!match) return false;
            }

            if (FilterStatus != "Усі" && room.Status != FilterStatus)
                return false;

            if (FilterType != "Усі" && room.Type != FilterType)
                return false;

            if (FilterCapacity != "Усі" && room.CapacityLabel != FilterCapacity)
                return false;

            if (!string.IsNullOrWhiteSpace(FilterMaxPrice) && decimal.TryParse(FilterMaxPrice, out decimal maxPrice))
            {
                if (room.PricePerNight > maxPrice) return false;
            }

            return true;
        }

        /// <summary>Застосовує обраний спосіб сортування номерів.</summary>
        private void ApplyRoomSorting()
        {
            FilteredRoomsView.SortDescriptions.Clear();
            switch (RoomSortOption)
            {
                case "За ціною (від дешевших)":
                    FilteredRoomsView.SortDescriptions.Add(new SortDescription(nameof(Room.PricePerNight), ListSortDirection.Ascending));
                    break;
                case "За ціною (від дорожчих)":
                    FilteredRoomsView.SortDescriptions.Add(new SortDescription(nameof(Room.PricePerNight), ListSortDirection.Descending));
                    break;
                case "За місткістю (зростання)":
                    FilteredRoomsView.SortDescriptions.Add(new SortDescription(nameof(Room.Capacity), ListSortDirection.Ascending));
                    break;
                case "За місткістю (спадання)":
                    FilteredRoomsView.SortDescriptions.Add(new SortDescription(nameof(Room.Capacity), ListSortDirection.Descending));
                    break;
                default:
                    FilteredRoomsView.SortDescriptions.Add(new SortDescription(nameof(Room.RoomNumber), ListSortDirection.Ascending));
                    break;
            }
        }

        /// <summary>Скидає пошук, фільтри та сортування номерів.</summary>
        private void ClearRoomFilters()
        {
            RoomSearchText = string.Empty;
            FilterStatus = "Усі";
            FilterType = "Усі";
            FilterCapacity = "Усі";
            FilterMaxPrice = string.Empty;
            RoomSortOption = "За номером (зростання)";
        }

        // Розрахунок вартості та система знижок за кожне 5-те заселення
        /// <summary>Рахує кількість діб, знижку (кожне 5-те заселення гостя) та підсумкову вартість.</summary>
        public void RecalculateBookingPrice()
        {
            var room = Rooms.FirstOrDefault(r => r.RoomNumber == BookingRoomNumber);
            decimal pricePerNight = room?.PricePerNight ?? 0m;

            int nights = (CheckOutDate.Date - CheckInDate.Date).Days;
            CalculatedNights = nights > 0 ? nights : 0;

            int pastStays = 0;
            if (!string.IsNullOrWhiteSpace(GuestFullName))
            {
                pastStays = Bookings.Count(b =>
                    b.GuestFullName.Equals(GuestFullName.Trim(), StringComparison.OrdinalIgnoreCase)
                    && b.BookingState != "Скасовано"
                    && (SelectedBooking == null || b.Id != SelectedBooking.Id));
            }

            int currentVisitNumber = pastStays + 1;
            if (currentVisitNumber % 5 == 0)
            {
                CalculatedDiscountPercent = 20m;
                LoyaltyStatusMessage = $"Це {currentVisitNumber}-те заселення гостя! Застосовано знижку 20%!";
            }
            else
            {
                CalculatedDiscountPercent = 0m;
                int remaining = 5 - (currentVisitNumber % 5);
                LoyaltyStatusMessage = $"Візит №{currentVisitNumber}. До знижки 20% залишилось візитів: {remaining}.";
            }

            decimal rawSum = pricePerNight * CalculatedNights;
            decimal discountAmount = rawSum * (CalculatedDiscountPercent / 100m);
            CalculatedTotalPrice = rawSum - discountAmount;
        }

        // Управління бронюваннями
        // У вкладці «Бронювання» показуємо лише «Заброньовано» та «Активне (Заселений)».
        // Завершені та скасовані бронювання дивимось у вкладці «Статистика».
        /// <summary>Залишає у списку лише активні та заброньовані записи і застосовує пошук.</summary>
        private bool FilterBookingPredicate(object obj)
        {
            Booking? b = obj as Booking;
            if (b == null) return false;

            if (b.BookingState != "Заброньовано" && b.BookingState != "Активне (Заселений)")
                return false;

            if (string.IsNullOrWhiteSpace(BookingSearchText)) return true;

            string q = BookingSearchText.Trim().ToLowerInvariant();
            return b.GuestFullName.ToLowerInvariant().Contains(q)
                   || b.RoomNumber.ToString().Contains(q)
                   || b.PhoneNumber.ToLowerInvariant().Contains(q)
                   || b.PassportOrId.ToLowerInvariant().Contains(q)
                   || b.BookingState.ToLowerInvariant().Contains(q);
        }

        // Перевірка конфліктів з обов'язковим буфером мінімум 1 година між виїздом та наступним в'їздом
        /// <summary>Шукає бронювання, яке перетинається за часом. Між виїздом і новим заїздом має бути не менше 1 години.</summary>
        private Booking? FindConflictingBooking(int roomNumber, DateTime newStart, DateTime newEnd, Guid? ignoreBookingId = null)
        {
            TimeSpan minBuffer = TimeSpan.FromHours(1);
            return Bookings.FirstOrDefault(b =>
                b.RoomNumber == roomNumber
                && b.IsActive
                && (!ignoreBookingId.HasValue || b.Id != ignoreBookingId.Value)
                && newStart < (b.GetCheckOutDateTime() + minBuffer)
                && newEnd > (b.GetCheckInDateTime() - minBuffer));
        }

        /// <summary>Перевіряє, що кількість цифр у номері відповідає обраній країні.</summary>
        private bool ValidatePhoneDigitsCount()
        {
            if (string.IsNullOrWhiteSpace(GuestPhoneNumber)) return true; // порожній номер дозволений
            int digitsCount = GuestPhoneNumber.Count(char.IsDigit);
            if (digitsCount != SelectedCountryCode.ExpectedDigits)
            {
                MessageBox.Show(
                    $"Для країни {SelectedCountryCode.CountryName} ({SelectedCountryCode.DialCode}) номер телефону має містити рівно {SelectedCountryCode.ExpectedDigits} цифр (формат: {SelectedCountryCode.MaskHint}).\nЗараз введено цифр: {digitsCount}.",
                    "Невірна кількість цифр у номері",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        /// <summary>Об'єднує дату та час із випадного списку в один DateTime.</summary>
        private DateTime CombineDateAndTime(DateTime date, string timeText, int defaultHour)
        {
            if (TimeSpan.TryParse(timeText, out var ts))
                return date.Date.Add(ts);
            return date.Date.AddHours(defaultHour);
        }

        /// <summary>Створює нове бронювання після всіх перевірок.</summary>
        private void CreateBooking()
        {
            if (string.IsNullOrWhiteSpace(GuestFullName))
            {
                MessageBox.Show("Ім'я гостя не може бути порожнім!", "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidatePhoneDigitsCount()) return;
            if (!ValidateGuestEmail()) return;

            DateTime startDateTime = CombineDateAndTime(CheckInDate, SelectedCheckInTime, 14);
            DateTime endDateTime = CombineDateAndTime(CheckOutDate, SelectedCheckOutTime, 12);

            if (endDateTime <= startDateTime)
            {
                MessageBox.Show("Дата та час виїзду мають бути пізніше за дату та час заселення!", "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var room = Rooms.FirstOrDefault(r => r.RoomNumber == BookingRoomNumber);
            if (room == null)
            {
                MessageBox.Show("Оберіть існуючий номер з каталогу!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool willBeActive = SelectedBookingState == "Заброньовано" || SelectedBookingState == "Активне (Заселений)";
            if (willBeActive && CheckInDate.Date < DateTime.Today)
            {
                ShowPastDateWarning();
                return;
            }

            if (willBeActive)
            {
                var conflict = FindConflictingBooking(BookingRoomNumber, startDateTime, endDateTime);
                if (conflict != null)
                {
                    MessageBox.Show(
                        $"Конфлікт часу бронювання для номера {BookingRoomNumber}!\n" +
                        $"Кімната зайнята гостем: {conflict.GuestFullName}\n" +
                        $"Період: {conflict.CheckInDateTimeDisplay} — {conflict.CheckOutDateTimeDisplay}.\n" +
                        $"Між виїздом попереднього гостя ({conflict.CheckOutDateTimeDisplay}) та новим заїздом має бути не менше 1 години (заселення можливе не раніше {conflict.GetCheckOutDateTime().AddHours(1):dd.MM.yyyy HH:mm}).",
                        "Конфлікт заїзду та виїзду",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }
            }

            RecalculateBookingPrice();
            int visitNum = Bookings.Count(b => b.GuestFullName.Equals(GuestFullName.Trim(), StringComparison.OrdinalIgnoreCase) && b.BookingState != "Скасовано") + 1;

            var booking = new Booking
            {
                RoomNumber = BookingRoomNumber,
                GuestFullName = GuestFullName.Trim(),
                CountryShortCode = SelectedCountryCode.ShortCode,
                DialCode = SelectedCountryCode.DialCode,
                PhoneNumber = PhoneFormatter.Format(GuestPhoneNumber, SelectedCountryCode.MaskHint),
                PassportOrId = GuestPassport.Trim(),
                Email = GuestEmail.Trim(),
                GuestNotes = GuestNotes.Trim(),
                CheckInDate = CheckInDate,
                CheckInTime = SelectedCheckInTime,
                CheckOutDate = CheckOutDate,
                CheckOutTime = SelectedCheckOutTime,
                BasePricePerNight = room.PricePerNight,
                DiscountPercent = CalculatedDiscountPercent,
                TotalCost = CalculatedTotalPrice,
                VisitNumber = visitNum,
                IsActive = willBeActive,
                BookingState = SelectedBookingState
            };

            Bookings.Add(booking);
            SyncRoomStatusWithBookings(room);
            PersistAllData();

            MessageBox.Show($"Бронювання для гостя {booking.GuestFullName} (Номер {booking.RoomNumber}) оформлено!\nЗаїзд: {booking.CheckInDateTimeDisplay}\nВиїзд: {booking.CheckOutDateTimeDisplay}\nВартість: {booking.TotalCost:N2} грн.",
                "Готель Rubellita", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Перевірка e-mail. Поле необов'язкове, але якщо воно заповнене,
        /// адреса має бути латиницею та містити символ «@».
        /// </summary>
        private bool ValidateGuestEmail()
        {
            string email = GuestEmail.Trim();
            if (email.Length == 0)
            {
                return true;
            }

            bool isCorrect = true;

            // 1. Дозволені лише латинські літери, цифри та символи . _ - + @
            foreach (char symbol in email)
            {
                bool isLatinLetter = (symbol >= 'a' && symbol <= 'z') || (symbol >= 'A' && symbol <= 'Z');
                bool isDigit = symbol >= '0' && symbol <= '9';
                bool isAllowedSymbol = symbol == '.' || symbol == '_' || symbol == '-' || symbol == '+' || symbol == '@';
                if (!isLatinLetter && !isDigit && !isAllowedSymbol)
                {
                    isCorrect = false;
                }
            }

            // 2. Рівно один символ «@», не на початку і не в кінці
            int atIndex = email.IndexOf('@');
            if (atIndex <= 0 || atIndex != email.LastIndexOf('@') || atIndex == email.Length - 1)
            {
                isCorrect = false;
            }
            else
            {
                // 3. Після «@» має бути крапка, а після неї ще щонайменше 2 символи (example.com)
                string domain = email.Substring(atIndex + 1);
                int dotIndex = domain.LastIndexOf('.');
                if (dotIndex <= 0 || domain.Length - dotIndex - 1 < 2)
                {
                    isCorrect = false;
                }
            }

            if (!isCorrect)
            {
                MessageBox.Show("E-mail має бути записаний латиницею та містити символ «@», наприклад: name@example.com.",
                    "Невірний e-mail", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return isCorrect;
        }

        /// <summary>Повідомлення про те, що дата заселення вже минула.</summary>
        private static void ShowPastDateWarning()
        {
            MessageBox.Show("Дата заселення не може бути раніше за сьогоднішню. Оберіть сьогоднішню або майбутню дату.",
                "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>Зберігає зміни в обраному бронюванні.</summary>
        private void UpdateBooking()
        {
            if (SelectedBooking == null)
            {
                MessageBox.Show("Оберіть бронювання зі списку для редагування.", "Увага", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(GuestFullName))
            {
                MessageBox.Show("Ім'я гостя не може бути порожнім!", "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidatePhoneDigitsCount()) return;
            if (!ValidateGuestEmail()) return;


            DateTime startDateTime = CombineDateAndTime(CheckInDate, SelectedCheckInTime, 14);
            DateTime endDateTime = CombineDateAndTime(CheckOutDate, SelectedCheckOutTime, 12);

            if (endDateTime <= startDateTime)
            {
                MessageBox.Show("Дата та час виїзду мають бути пізніше за дату та час заселення!", "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool willBeActive = SelectedBookingState == "Заброньовано" || SelectedBookingState == "Активне (Заселений)";
            // у вже існуючого бронювання стару дату заселення можна лишити, але не можна змінити її на минулу
            if (willBeActive && CheckInDate.Date < DateTime.Today && CheckInDate.Date != SelectedBooking.CheckInDate.Date)
            {
                ShowPastDateWarning();
                return;
            }

            if (willBeActive)
            {
                var conflict = FindConflictingBooking(BookingRoomNumber, startDateTime, endDateTime, SelectedBooking.Id);
                if (conflict != null)
                {
                    MessageBox.Show(
                        $"Конфлікт часу бронювання! Номер {BookingRoomNumber} зайнятий з {conflict.CheckInDateTimeDisplay} по {conflict.CheckOutDateTimeDisplay} ({conflict.GuestFullName}).",
                        "Конфлікт часу",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }
            }

            var oldRoom = Rooms.FirstOrDefault(r => r.RoomNumber == SelectedBooking.RoomNumber);
            var newRoom = Rooms.FirstOrDefault(r => r.RoomNumber == BookingRoomNumber);
            if (newRoom == null)
            {
                MessageBox.Show("Оберіть існуючий номер з каталогу!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            RecalculateBookingPrice();

            SelectedBooking.RoomNumber = BookingRoomNumber;
            SelectedBooking.GuestFullName = GuestFullName.Trim();
            SelectedBooking.CountryShortCode = SelectedCountryCode.ShortCode;
            SelectedBooking.DialCode = SelectedCountryCode.DialCode;
            SelectedBooking.PhoneNumber = PhoneFormatter.Format(GuestPhoneNumber, SelectedCountryCode.MaskHint);
            SelectedBooking.PassportOrId = GuestPassport.Trim();
            SelectedBooking.Email = GuestEmail.Trim();
            SelectedBooking.GuestNotes = GuestNotes.Trim();
            SelectedBooking.CheckInDate = CheckInDate;
            SelectedBooking.CheckInTime = SelectedCheckInTime;
            SelectedBooking.CheckOutDate = CheckOutDate;
            SelectedBooking.CheckOutTime = SelectedCheckOutTime;
            SelectedBooking.BasePricePerNight = newRoom.PricePerNight;
            SelectedBooking.DiscountPercent = CalculatedDiscountPercent;
            SelectedBooking.TotalCost = CalculatedTotalPrice;
            SelectedBooking.BookingState = SelectedBookingState;
            SelectedBooking.IsActive = willBeActive;

            if (oldRoom != null) SyncRoomStatusWithBookings(oldRoom);
            SyncRoomStatusWithBookings(newRoom);

            PersistAllData();
            if (!willBeActive)
            {
                SelectedBooking = null;
                MessageBox.Show("Бронювання переведено у статус «" + SelectedBookingState + "» та переміщено у розділ «Статистика».", "Готель Rubellita", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Бронювання оновлено та збережено у JSON.", "Готель Rubellita", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>Завершує проживання (виселення гостя) і звільняє номер.</summary>
        private void CompleteBooking()
        {
            if (SelectedBooking == null)
            {
                MessageBox.Show("Оберіть активне бронювання для завершення (виселення гостя).", "Увага", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            SelectedBooking.BookingState = "Завершено";
            SelectedBooking.IsActive = false;

            var room = Rooms.FirstOrDefault(r => r.RoomNumber == SelectedBooking.RoomNumber);
            if (room != null)
            {
                SyncRoomStatusWithBookings(room);
            }

            SelectedBooking = null;
            PersistAllData();
            MessageBox.Show("Проживання завершено! Запис перенесено до розділу «Статистика та звітність», а номер звільнено.", "Готель Rubellita", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>Скасовує обране бронювання і звільняє номер.</summary>
        private void CancelBooking()
        {
            if (SelectedBooking == null)
            {
                MessageBox.Show("Оберіть бронювання для скасування.", "Увага", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            SelectedBooking.BookingState = "Скасовано";
            SelectedBooking.IsActive = false;

            var room = Rooms.FirstOrDefault(r => r.RoomNumber == SelectedBooking.RoomNumber);
            if (room != null)
            {
                SyncRoomStatusWithBookings(room);
            }

            SelectedBooking = null;
            PersistAllData();
            MessageBox.Show("Бронювання скасовано та перенесено до історії у статистиці.", "Готель Rubellita", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>Очищає форму бронювання.</summary>
        private void ClearBookingForm(bool keepRoom = false)
        {
            SelectedBooking = null;
            if (!keepRoom)
                BookingRoomNumber = Rooms.FirstOrDefault()?.RoomNumber ?? 101;
            GuestFullName = string.Empty;
            GuestPhoneNumber = string.Empty;
            GuestPassport = string.Empty;
            GuestEmail = string.Empty;
            GuestNotes = string.Empty;
            CheckInDate = DateTime.Today;
            SelectedCheckInTime = "14:00";
            CheckOutDate = DateTime.Today.AddDays(2);
            SelectedCheckOutTime = "12:00";
            SelectedBookingState = "Активне (Заселений)";
            RecalculateBookingPrice();
        }

        /// <summary>Оновлює статус номера за його активними бронюваннями.</summary>
        private void SyncRoomStatusWithBookings(Room room)
        {
            var activeForRoom = Bookings.Where(b => b.RoomNumber == room.RoomNumber && b.IsActive).ToList();
            if (!activeForRoom.Any())
            {
                room.Status = RoomConstants.StatusFree;
                return;
            }

            if (activeForRoom.Any(b => b.BookingState == "Активне (Заселений)"))
                room.Status = RoomConstants.StatusOccupied;
            else
                room.Status = RoomConstants.StatusBooked;
        }

        // Пошук гостей та частота візитів
        /// <summary>Групує бронювання за гостями (кількість заселень, витрати) та застосовує пошук.</summary>
        public void RefreshGuestSummaries()
        {
            GuestSummaries.Clear();

            var validBookings = Bookings.Where(b => b.BookingState != "Скасовано").ToList();
            var grouped = validBookings
                .GroupBy(b => b.GuestFullName.Trim(), StringComparer.OrdinalIgnoreCase)
                .Select(g =>
                {
                    var latest = g.OrderByDescending(x => x.CheckInDate).First();
                    return new GuestSummary
                    {
                        GuestFullName = latest.GuestFullName,
                        PhoneDisplay = latest.FullPhoneDisplay,
                        PassportOrId = latest.PassportOrId,
                        Email = latest.Email,
                        StaysCount = g.Count(),
                        TotalSpent = g.Sum(x => x.TotalCost),
                        LastCheckIn = latest.CheckInDate,
                        LastCheckOut = latest.CheckOutDate
                    };
                })
                .OrderByDescending(gs => gs.StaysCount)
                .ThenBy(gs => gs.GuestFullName);

            string q = GuestSearchQuery?.Trim().ToLowerInvariant() ?? string.Empty;

            foreach (var gs in grouped)
            {
                if (string.IsNullOrEmpty(q)
                    || gs.GuestFullName.ToLowerInvariant().Contains(q)
                    || gs.PhoneDisplay.ToLowerInvariant().Contains(q)
                    || gs.PassportOrId.ToLowerInvariant().Contains(q))
                {
                    GuestSummaries.Add(gs);
                }
            }
        }

        // Статистика за період
        /// <summary>Перераховує статистику за обраний період.</summary>
        public void RefreshStatistics()
        {
            TotalRoomsCount = Rooms.Count;
            FreeRoomsCount = Rooms.Count(r => r.Status == RoomConstants.StatusFree);
            BookedRoomsCount = Rooms.Count(r => r.Status == RoomConstants.StatusBooked);
            OccupiedRoomsCount = Rooms.Count(r => r.Status == RoomConstants.StatusOccupied);

            int busyCount = BookedRoomsCount + OccupiedRoomsCount;
            OccupancyRatePercent = TotalRoomsCount > 0
                ? Math.Round((double)busyCount / TotalRoomsCount * 100.0, 1)
                : 0;

            ActiveBookingsCount = Bookings.Count(b => b.IsActive);
            CompletedBookingsCount = Bookings.Count(b => b.BookingState == "Завершено");

            var query = Bookings
                .Where(b => b.BookingState != "Скасовано"
                            && b.CheckInDate.Date <= StatsEndDate.Date
                            && b.CheckOutDate.Date >= StatsStartDate.Date);

            if (StatsStateFilter == "Тільки завершені")
                query = query.Where(b => b.BookingState == "Завершено");
            else if (StatsStateFilter == "Активні та заброньовані")
                query = query.Where(b => b.IsActive);

            var periodList = query.OrderByDescending(b => b.CheckInDate).ToList();
            PeriodRevenue = periodList.Sum(b => b.TotalCost);

            PeriodMovementBookings.Clear();
            foreach (var item in periodList)
            {
                PeriodMovementBookings.Add(item);
            }
        }

        // Автоматичне збереження після кожної зміни
        /// <summary>Зберігає номери й бронювання у JSON та оновлює списки на екрані.</summary>
        private void PersistAllData()
        {
            _storage.SaveRooms(Rooms);
            _storage.SaveBookings(Bookings);

            FilteredRoomsView.Refresh();
            FilteredBookingsView.Refresh();
            RefreshFloorMap();
            RefreshGuestSummaries();
            RefreshStatistics();
            OnPropertyChanged(nameof(SelectedMapRoomBooking));
        }
    }
}
