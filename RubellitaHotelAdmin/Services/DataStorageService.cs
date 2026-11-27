using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using RubellitaHotelAdmin.Models;

namespace RubellitaHotelAdmin.Services
{
    /// <summary>Читання та запис даних у JSON-файли (адміністратори, номери, бронювання).</summary>
    public class DataStorageService
    {
        private readonly string _dataFolder;
        private readonly string _adminsFile;
        private readonly string _roomsFile;
        private readonly string _bookingsFile;

        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public DataStorageService()
        {
            var projectRoot = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.FullName;
            var rootFolder = !string.IsNullOrWhiteSpace(projectRoot) && File.Exists(Path.Combine(projectRoot, "RubellitaHotelAdmin.csproj"))
                ? projectRoot
                : AppContext.BaseDirectory;

            _dataFolder = Path.Combine(rootFolder, "HotelData");
            Directory.CreateDirectory(_dataFolder);

            _adminsFile = Path.Combine(_dataFolder, "admins.json");
            _roomsFile = Path.Combine(_dataFolder, "rooms.json");
            _bookingsFile = Path.Combine(_dataFolder, "bookings.json");

            EnsureSeedData();
        }

        /// <summary>Якщо JSON-файлів ще немає, створює їх зі стартовими даними.</summary>
        private void EnsureSeedData()
        {
            if (!File.Exists(_adminsFile))
            {
                var defaultAdmins = new List<AdminUser>
                {
                    new AdminUser { Login = "admin_olena", FullName = "Олена Коваленко", AccessCode = "1111", Role = "Старший адміністратор" },
                    new AdminUser { Login = "admin_taras", FullName = "Тарас Шевчук", AccessCode = "2222", Role = "Адміністратор зміни" },
                    new AdminUser { Login = "admin_irina", FullName = "Ірина Мельник", AccessCode = "3333", Role = "Адміністратор" },
                    new AdminUser { Login = "manager_rubellita", FullName = "Максим Бондаренко", AccessCode = "7777", Role = "Керуючий готелем" }
                };
                SaveAdmins(defaultAdmins);
            }

            if (!File.Exists(_roomsFile))
            {
                var defaultRooms = new List<Room>
                {
                    // 1-й поверх
                    new Room { RoomNumber = 101, Floor = 1, Type = "Стандарт (Standard)", Capacity = 2, PricePerNight = 1400, Status = RoomConstants.StatusFree, IsCorner = true, Wing = "Ліве крило", Description = "Кутовий світлий номер з видом на сад" },
                    new Room { RoomNumber = 102, Floor = 1, Type = "Стандарт (Standard)", Capacity = 1, PricePerNight = 1100, Status = RoomConstants.StatusOccupied, IsCorner = false, Wing = "Ліве крило", Description = "Одномісний номер біля рецепції" },
                    new Room { RoomNumber = 103, Floor = 1, Type = "Покращений (Superior)", Capacity = 2, PricePerNight = 1850, Status = RoomConstants.StatusBooked, IsCorner = false, Wing = "Ліве крило", Description = "Двоспальне ліжко King-Size" },
                    new Room { RoomNumber = 104, Floor = 1, Type = "Покращений (Superior)", Capacity = 2, PricePerNight = 1850, Status = RoomConstants.StatusFree, IsCorner = false, Wing = "Праве крило", Description = "Вид у внутрішній дворик" },
                    new Room { RoomNumber = 105, Floor = 1, Type = "Сімейний (Family)", Capacity = 4, PricePerNight = 2600, Status = RoomConstants.StatusFree, IsCorner = false, Wing = "Праве крило", Description = "Дві суміжні кімнати для сім'ї" },
                    new Room { RoomNumber = 106, Floor = 1, Type = "Напівлюкс (Junior Suite)", Capacity = 3, PricePerNight = 2900, Status = RoomConstants.StatusOccupied, IsCorner = true, Wing = "Праве крило", Description = "Кутовий панорамний номер" },

                    // 2-й поверх
                    new Room { RoomNumber = 201, Floor = 2, Type = "Напівлюкс (Junior Suite)", Capacity = 2, PricePerNight = 2800, Status = RoomConstants.StatusOccupied, IsCorner = true, Wing = "Ліве крило", Description = "Кутовий номер з балконом" },
                    new Room { RoomNumber = 202, Floor = 2, Type = "Стандарт (Standard)", Capacity = 2, PricePerNight = 1500, Status = RoomConstants.StatusFree, IsCorner = false, Wing = "Ліве крило", Description = "Два окремі ліжка Twin" },
                    new Room { RoomNumber = 203, Floor = 2, Type = "Покращений (Superior)", Capacity = 2, PricePerNight = 1950, Status = RoomConstants.StatusFree, IsCorner = false, Wing = "Ліве крило", Description = "Робоча зона та кавомашина" },
                    new Room { RoomNumber = 204, Floor = 2, Type = "Люкс (Suite)", Capacity = 3, PricePerNight = 3800, Status = RoomConstants.StatusBooked, IsCorner = false, Wing = "Праве крило", Description = "Вітальня + спальня, джакузі" },
                    new Room { RoomNumber = 205, Floor = 2, Type = "Сімейний (Family)", Capacity = 4, PricePerNight = 2750, Status = RoomConstants.StatusFree, IsCorner = false, Wing = "Праве крило", Description = "Просторий номер для 4 гостей" },
                    new Room { RoomNumber = 206, Floor = 2, Type = "Люкс (Suite)", Capacity = 2, PricePerNight = 4100, Status = RoomConstants.StatusFree, IsCorner = true, Wing = "Праве крило", Description = "Кутовий люкс Rubellita" },

                    // 3-й поверх
                    new Room { RoomNumber = 301, Floor = 3, Type = "Люкс (Suite)", Capacity = 2, PricePerNight = 4300, Status = RoomConstants.StatusFree, IsCorner = true, Wing = "Ліве крило", Description = "Кутовий панорамний люкс" },
                    new Room { RoomNumber = 302, Floor = 3, Type = "Покращений (Superior)", Capacity = 2, PricePerNight = 2100, Status = RoomConstants.StatusOccupied, IsCorner = false, Wing = "Ліве крило", Description = "Тихий поверх, ортопедичний матрац" },
                    new Room { RoomNumber = 303, Floor = 3, Type = "Напівлюкс (Junior Suite)", Capacity = 3, PricePerNight = 3100, Status = RoomConstants.StatusFree, IsCorner = false, Wing = "Ліве крило", Description = "Тераса з видом на місто" },
                    new Room { RoomNumber = 304, Floor = 3, Type = "Президентський (Presidential)", Capacity = 4, PricePerNight = 6500, Status = RoomConstants.StatusFree, IsCorner = true, Wing = "Праве крило", Description = "Апартаменти вищого класу з терасою" },
                    new Room { RoomNumber = 305, Floor = 3, Type = "Президентський (Presidential)", Capacity = 4, PricePerNight = 6800, Status = RoomConstants.StatusFree, IsCorner = false, Wing = "Праве крило", Description = "Другий президентський люкс з камінним залом та балконом" }
                };
                SaveRooms(defaultRooms);
            }

            if (!File.Exists(_bookingsFile))
            {
                var today = DateTime.Today;
                var defaultBookings = new List<Booking>
                {
                    // Попередні 4 завершені візити постійного гостя (відображаються у статистиці)
                    new Booking()
                    {
                        RoomNumber = 101,
                        GuestFullName = "Савченко Андрій Вікторович",
                        CountryShortCode = "UA",
                        DialCode = "+380",
                        PhoneNumber = "67-512-34-89",
                        PassportOrId = "КН 458912",
                        Email = "savchenko.a@ukr.net",
                        GuestNotes = "Постійний гість, любить тихі номери",
                        CheckInDate = today.AddDays(-60),
                        CheckInTime = "14:00",
                        CheckOutDate = today.AddDays(-58),
                        CheckOutTime = "12:00",
                        BasePricePerNight = 1400,
                        DiscountPercent = 0,
                        TotalCost = 2800,
                        VisitNumber = 1,
                        IsActive = false,
                        BookingState = "Завершено"
                    },
                    new Booking()
                    {
                        RoomNumber = 202,
                        GuestFullName = "Савченко Андрій Вікторович",
                        CountryShortCode = "UA",
                        DialCode = "+380",
                        PhoneNumber = "67-512-34-89",
                        PassportOrId = "КН 458912",
                        Email = "savchenko.a@ukr.net",
                        GuestNotes = "Ранній сніданок о 07:30",
                        CheckInDate = today.AddDays(-45),
                        CheckInTime = "15:00",
                        CheckOutDate = today.AddDays(-42),
                        CheckOutTime = "11:00",
                        BasePricePerNight = 1500,
                        DiscountPercent = 0,
                        TotalCost = 4500,
                        VisitNumber = 2,
                        IsActive = false,
                        BookingState = "Завершено"
                    },
                    new Booking()
                    {
                        RoomNumber = 203,
                        GuestFullName = "Савченко Андрій Вікторович",
                        CountryShortCode = "UA",
                        DialCode = "+380",
                        PhoneNumber = "67-512-34-89",
                        PassportOrId = "КН 458912",
                        Email = "savchenko.a@ukr.net",
                        GuestNotes = "Відрядження",
                        CheckInDate = today.AddDays(-25),
                        CheckInTime = "14:00",
                        CheckOutDate = today.AddDays(-23),
                        CheckOutTime = "12:00",
                        BasePricePerNight = 1950,
                        DiscountPercent = 0,
                        TotalCost = 3900,
                        VisitNumber = 3,
                        IsActive = false,
                        BookingState = "Завершено"
                    },
                    new Booking()
                    {
                        RoomNumber = 104,
                        GuestFullName = "Савченко Андрій Вікторович",
                        CountryShortCode = "UA",
                        DialCode = "+380",
                        PhoneNumber = "67-512-34-89",
                        PassportOrId = "КН 458912",
                        Email = "savchenko.a@ukr.net",
                        GuestNotes = "Наступний візит буде 5-м (знижка 20%)",
                        CheckInDate = today.AddDays(-10),
                        CheckInTime = "13:00",
                        CheckOutDate = today.AddDays(-8),
                        CheckOutTime = "12:00",
                        BasePricePerNight = 1850,
                        DiscountPercent = 0,
                        TotalCost = 3700,
                        VisitNumber = 4,
                        IsActive = false,
                        BookingState = "Завершено"
                    },

                    // Активні проживання та бронювання
                    new Booking()
                    {
                        RoomNumber = 102,
                        GuestFullName = "Лисенко Марія Олександрівна",
                        CountryShortCode = "UA",
                        DialCode = "+380",
                        PhoneNumber = "50-987-65-43",
                        PassportOrId = "004829174",
                        Email = "m.lysenko@gmail.com",
                        GuestNotes = "Потрібна додаткова подушка",
                        CheckInDate = today.AddDays(-1),
                        CheckInTime = "14:00",
                        CheckOutDate = today.AddDays(2),
                        CheckOutTime = "12:00",
                        BasePricePerNight = 1100,
                        DiscountPercent = 0,
                        TotalCost = 3300,
                        VisitNumber = 1,
                        IsActive = true,
                        BookingState = "Активне (Заселений)"
                    },
                    new Booking()
                    {
                        RoomNumber = 106,
                        GuestFullName = "Kowalski Piotr",
                        CountryShortCode = "PL",
                        DialCode = "+48",
                        PhoneNumber = "501-234-567",
                        PassportOrId = "PL8849201",
                        Email = "p.kowalski@wp.pl",
                        GuestNotes = "Бізнес-візит, паркомісце №4",
                        CheckInDate = today.AddDays(-2),
                        CheckInTime = "15:00",
                        CheckOutDate = today.AddDays(1),
                        CheckOutTime = "11:00",
                        BasePricePerNight = 2900,
                        DiscountPercent = 0,
                        TotalCost = 8700,
                        VisitNumber = 1,
                        IsActive = true,
                        BookingState = "Активне (Заселений)"
                    },
                    new Booking()
                    {
                        RoomNumber = 201,
                        GuestFullName = "Демченко Вікторія Ігорівна",
                        CountryShortCode = "UA",
                        DialCode = "+380",
                        PhoneNumber = "93-444-11-22",
                        PassportOrId = "007123456",
                        Email = "vika.dem@icloud.com",
                        GuestNotes = "Святкування річниці",
                        CheckInDate = today,
                        CheckInTime = "14:00",
                        CheckOutDate = today.AddDays(3),
                        CheckOutTime = "12:00",
                        BasePricePerNight = 2800,
                        DiscountPercent = 0,
                        TotalCost = 8400,
                        VisitNumber = 1,
                        IsActive = true,
                        BookingState = "Активне (Заселений)"
                    },
                    new Booking()
                    {
                        RoomNumber = 302,
                        GuestFullName = "Müller Hans",
                        CountryShortCode = "DE",
                        DialCode = "+49",
                        PhoneNumber = "151-2345-6789",
                        PassportOrId = "DE9931024",
                        Email = "h.mueller@gmx.de",
                        GuestNotes = "Англомовний супровід",
                        CheckInDate = today.AddDays(-1),
                        CheckInTime = "16:00",
                        CheckOutDate = today.AddDays(4),
                        CheckOutTime = "12:00",
                        BasePricePerNight = 2100,
                        DiscountPercent = 0,
                        TotalCost = 10500,
                        VisitNumber = 1,
                        IsActive = true,
                        BookingState = "Активне (Заселений)"
                    },
                    new Booking()
                    {
                        RoomNumber = 103,
                        GuestFullName = "Ткачук Олександр Сергійович",
                        CountryShortCode = "UA",
                        DialCode = "+380",
                        PhoneNumber = "63-222-33-44",
                        PassportOrId = "МЕ 741852",
                        Email = "tkachuk.alex@gmail.com",
                        GuestNotes = "Пізній заїзд після 21:00",
                        CheckInDate = today.AddDays(1),
                        CheckInTime = "21:00",
                        CheckOutDate = today.AddDays(4),
                        CheckOutTime = "12:00",
                        BasePricePerNight = 1850,
                        DiscountPercent = 0,
                        TotalCost = 5550,
                        VisitNumber = 1,
                        IsActive = true,
                        BookingState = "Заброньовано"
                    },
                    new Booking()
                    {
                        RoomNumber = 204,
                        GuestFullName = "Бойко Наталія Василівна",
                        CountryShortCode = "UA",
                        DialCode = "+380",
                        PhoneNumber = "97-888-77-66",
                        PassportOrId = "003951753",
                        Email = "n.boyko@ukr.net",
                        GuestNotes = "VIP-обслуговування",
                        CheckInDate = today.AddDays(2),
                        CheckInTime = "14:00",
                        CheckOutDate = today.AddDays(5),
                        CheckOutTime = "12:00",
                        BasePricePerNight = 3800,
                        DiscountPercent = 0,
                        TotalCost = 11400,
                        VisitNumber = 1,
                        IsActive = true,
                        BookingState = "Заброньовано"
                    }
                };
                SaveBookings(defaultBookings);
            }
        }

        public List<AdminUser> LoadAdmins()
        {
            if (!File.Exists(_adminsFile)) return new List<AdminUser>();
            var json = File.ReadAllText(_adminsFile);
            return JsonSerializer.Deserialize<List<AdminUser>>(json, _jsonOptions) ?? new List<AdminUser>();
        }

        public void SaveAdmins(IEnumerable<AdminUser> admins)
        {
            var json = JsonSerializer.Serialize(admins, _jsonOptions);
            File.WriteAllText(_adminsFile, json);
        }

        public List<Room> LoadRooms()
        {
            if (!File.Exists(_roomsFile)) return new List<Room>();
            var json = File.ReadAllText(_roomsFile);
            return JsonSerializer.Deserialize<List<Room>>(json, _jsonOptions) ?? new List<Room>();
        }

        public void SaveRooms(IEnumerable<Room> rooms)
        {
            var json = JsonSerializer.Serialize(rooms, _jsonOptions);
            File.WriteAllText(_roomsFile, json);
        }

        public List<Booking> LoadBookings()
        {
            if (!File.Exists(_bookingsFile)) return new List<Booking>();
            var json = File.ReadAllText(_bookingsFile);
            return JsonSerializer.Deserialize<List<Booking>>(json, _jsonOptions) ?? new List<Booking>();
        }

        public void SaveBookings(IEnumerable<Booking> bookings)
        {
            var json = JsonSerializer.Serialize(bookings, _jsonOptions);
            File.WriteAllText(_bookingsFile, json);
        }
    }
}
