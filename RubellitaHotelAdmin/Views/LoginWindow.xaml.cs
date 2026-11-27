using System.Windows;
using System.Windows.Controls;
using RubellitaHotelAdmin.Models;
using RubellitaHotelAdmin.ViewModels;

namespace RubellitaHotelAdmin.Views
{
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel _vm;

        public LoginWindow()
        {
            InitializeComponent();
            _vm = new LoginViewModel();
            _vm.LoginSucceeded += OnLoginSucceeded;
            DataContext = _vm;
        }

        private void AccessCodeBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox pb)
            {
                _vm.EnteredCode = pb.Password;
            }
        }

        private void OnLoginSucceeded(AdminUser admin)
        {
            var mainWindow = new MainWindow(admin);
            mainWindow.Show();
            Close();
        }
    }
}
