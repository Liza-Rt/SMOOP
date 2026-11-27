using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RubellitaHotelAdmin.ViewModels
{
    /// <summary>Базовий клас для ViewModel: повідомляє інтерфейс про зміну властивостей.</summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
