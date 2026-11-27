using System.Linq;
using System.Text;

namespace RubellitaHotelAdmin.Helpers
{
    /// <summary>Розставляє тире в номері телефону за шаблоном країни.</summary>
    public static class PhoneFormatter
    {
        /// <summary>
        /// Розставляє тире за шаблоном країни: цифри 994812345 + шаблон 00-000-00-00 -> 99-481-23-45.
        /// Будь-які символи, крім цифр, у вхідному номері ігноруються.
        /// </summary>
        public static string Format(string? phone, string? mask)
        {
            if (string.IsNullOrWhiteSpace(phone)) return string.Empty;

            string digits = new string(phone.Where(char.IsDigit).ToArray());
            if (string.IsNullOrEmpty(mask)) return digits;

            var sb = new StringBuilder();
            int index = 0;
            foreach (char m in mask)
            {
                if (index >= digits.Length) break;   // цифри закінчились - зайвих тире не ставимо
                if (m == '0') sb.Append(digits[index++]);
                else sb.Append(m);
            }

            if (index < digits.Length)               // якщо цифр більше, ніж у шаблоні
                sb.Append(digits, index, digits.Length - index);

            return sb.ToString();
        }
    }
}
