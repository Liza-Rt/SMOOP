namespace RubellitaHotelAdmin.Models
{
    /// <summary>Країна для вибору коду телефону.</summary>
    public class CountryPhoneCode
    {
        public string CountryName { get; set; } = string.Empty;
        public string ShortCode { get; set; } = string.Empty; // UA, PL... - за ним підбирається прапор Flag_XX з Resources/Flags.xaml
        public string DialCode { get; set; } = string.Empty;
        public string MaskHint { get; set; } = string.Empty;  // шаблон номера, напр. 00-000-00-00 (кожен 0 = одна цифра)
        public int ExpectedDigits { get; set; }
    }
}
