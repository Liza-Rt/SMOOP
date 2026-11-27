using System.Collections.Generic;
using System.Linq;

namespace RubellitaHotelAdmin.Models
{
    /// <summary>Країни, шаблони їхніх номерів та кількість цифр. Єдине джерело для форми, перевірки й відображення.</summary>
    public static class CountryPhoneCatalog
    {
        public static IReadOnlyList<CountryPhoneCode> All { get; } = new List<CountryPhoneCode>
        {
            new CountryPhoneCode { ShortCode = "UA", CountryName = "Україна",         DialCode = "+380", MaskHint = "00-000-00-00",  ExpectedDigits = 9 },
            new CountryPhoneCode { ShortCode = "PL", CountryName = "Польща",          DialCode = "+48",  MaskHint = "000-000-000",   ExpectedDigits = 9 },
            new CountryPhoneCode { ShortCode = "DE", CountryName = "Німеччина",       DialCode = "+49",  MaskHint = "000-0000-0000", ExpectedDigits = 11 },
            new CountryPhoneCode { ShortCode = "CZ", CountryName = "Чехія",           DialCode = "+420", MaskHint = "000-000-000",   ExpectedDigits = 9 },
            new CountryPhoneCode { ShortCode = "GB", CountryName = "Велика Британія", DialCode = "+44",  MaskHint = "0000-000000",   ExpectedDigits = 10 },
            new CountryPhoneCode { ShortCode = "US", CountryName = "США",             DialCode = "+1",   MaskHint = "000-000-0000",  ExpectedDigits = 10 },
            new CountryPhoneCode { ShortCode = "CA", CountryName = "Канада",          DialCode = "+1",   MaskHint = "000-000-0000",  ExpectedDigits = 10 },
            new CountryPhoneCode { ShortCode = "FR", CountryName = "Франція",         DialCode = "+33",  MaskHint = "0-00-00-00-00", ExpectedDigits = 9 },
            new CountryPhoneCode { ShortCode = "IT", CountryName = "Італія",          DialCode = "+39",  MaskHint = "000-000-0000",  ExpectedDigits = 10 },
            new CountryPhoneCode { ShortCode = "ES", CountryName = "Іспанія",         DialCode = "+34",  MaskHint = "000-000-000",   ExpectedDigits = 9 },
            new CountryPhoneCode { ShortCode = "AT", CountryName = "Австрія",         DialCode = "+43",  MaskHint = "000-0000000",   ExpectedDigits = 10 },
            new CountryPhoneCode { ShortCode = "NL", CountryName = "Нідерланди",      DialCode = "+31",  MaskHint = "00-0000000",    ExpectedDigits = 9 },
            new CountryPhoneCode { ShortCode = "RO", CountryName = "Румунія",         DialCode = "+40",  MaskHint = "000-000-000",   ExpectedDigits = 9 }
        };

        public static string? FindMask(string? shortCode)
            => All.FirstOrDefault(c => c.ShortCode == shortCode)?.MaskHint;
    }
}
