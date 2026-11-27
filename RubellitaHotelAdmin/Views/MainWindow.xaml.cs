using System.Windows;
using RubellitaHotelAdmin.Models;
using RubellitaHotelAdmin.ViewModels;

namespace RubellitaHotelAdmin.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(AdminUser admin)
        {
            InitializeComponent();
            DataContext = new MainViewModel(admin);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWin = new LoginWindow();
            loginWin.Show();
            Close();
        }
    }
}
