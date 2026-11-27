using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using RubellitaHotelAdmin.Models;

namespace RubellitaHotelAdmin.Converters
{
    /// <summary>Колір фону рядка залежно від статусу номера.</summary>
    public class StatusToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value == null ? string.Empty : value.ToString()!;

            if (status == RoomConstants.StatusFree)
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E8F5E9"));
            }
            if (status == RoomConstants.StatusBooked)
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF3E0"));
            }
            if (status == RoomConstants.StatusOccupied)
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FCE4EC"));
            }
            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>Колір тексту залежно від статусу номера.</summary>
    public class StatusToForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value == null ? string.Empty : value.ToString()!;

            if (status == RoomConstants.StatusFree)
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2E7D32"));
            }
            if (status == RoomConstants.StatusBooked)
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EF6C00"));
            }
            if (status == RoomConstants.StatusOccupied)
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#AA1945"));
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>true - елемент видно, false - ховаємо.</summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && (bool)value)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Visibility && (Visibility)value == Visibility.Visible;
        }
    }

    /// <summary>true - ховаємо елемент, false - показуємо (навпаки до попереднього).</summary>
    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && (bool)value)
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Visibility && (Visibility)value != Visibility.Visible;
        }
    }

    /// <summary>Перевіряє, чи два значення однакові (для підсвічування обраного номера на плані).</summary>
    public class EqualityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2 || values[0] == null || values[1] == null)
            {
                return false;
            }
            return ReferenceEquals(values[0], values[1]) || Equals(values[0], values[1]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>Повертає власний векторний прапор країни за її кодом (UA, PL, ...).</summary>
    public class FlagConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string? code = value == null ? null : value.ToString();
            if (string.IsNullOrWhiteSpace(code))
            {
                return null;
            }
            return Application.Current.TryFindResource("Flag_" + code.ToUpperInvariant());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
