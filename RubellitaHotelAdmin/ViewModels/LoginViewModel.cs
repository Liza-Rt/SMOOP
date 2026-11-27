using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using RubellitaHotelAdmin.Models;
using RubellitaHotelAdmin.Services;

namespace RubellitaHotelAdmin.ViewModels
{
    /// <summary>Логіка вікна входу: вибір адміністратора та перевірка коду доступу.</summary>
    public class LoginViewModel : BaseViewModel
    {
        private readonly DataStorageService _storage;

        public ObservableCollection<AdminUser> Admins { get; } = new ObservableCollection<AdminUser>();

        private AdminUser? _selectedAdmin;
        public AdminUser? SelectedAdmin
        {
            get => _selectedAdmin;
            set
            {
                _selectedAdmin = value;
                OnPropertyChanged();
                ErrorMessage = string.Empty;
            }
        }

        private string _enteredCode = string.Empty;
        public string EnteredCode
        {
            get => _enteredCode;
            set
            {
                _enteredCode = value;
                OnPropertyChanged();
                ErrorMessage = string.Empty;
            }
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        private RelayCommand? _loginCommand;
        public RelayCommand LoginCommand
        {
            get
            {
                return _loginCommand ?? (_loginCommand = new RelayCommand(obj =>
                {
                    ExecuteLogin();
                }));
            }
        }

        public event System.Action<AdminUser>? LoginSucceeded;

        public LoginViewModel()
        {
            _storage = new DataStorageService();
            var loaded = _storage.LoadAdmins();
            foreach (var admin in loaded)
            {
                Admins.Add(admin);
            }
            SelectedAdmin = Admins.FirstOrDefault();
        }

        /// <summary>Перевіряє код доступу обраного адміністратора.</summary>
        private void ExecuteLogin()
        {
            if (SelectedAdmin == null)
            {
                ErrorMessage = "Оберіть адміністратора зі списку (дані завантажуються з admins.json).";
                return;
            }

            if (string.IsNullOrWhiteSpace(EnteredCode))
            {
                ErrorMessage = "Введіть код доступу.";
                return;
            }

            if (SelectedAdmin.AccessCode == EnteredCode.Trim())
            {
                ErrorMessage = string.Empty;
                LoginSucceeded?.Invoke(SelectedAdmin);
            }
            else
            {
                ErrorMessage = "Невірний код доступу для обраного адміністратора.";
            }
        }
    }
}
