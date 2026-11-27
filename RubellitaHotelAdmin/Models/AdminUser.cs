namespace RubellitaHotelAdmin.Models
{
    /// <summary>Адміністратор, який входить у систему за кодом доступу.</summary>
    public class AdminUser
    {
        public string Login { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string AccessCode { get; set; } = string.Empty;
        public string Role { get; set; } = "Адміністратор";

        public string DisplayTitle => $"{FullName} ({Login})";
    }
}
