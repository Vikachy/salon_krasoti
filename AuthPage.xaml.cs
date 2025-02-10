using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace salon_krasoti
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }

        private void signInButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(loginTextBox.Text) ||
                string.IsNullOrEmpty(passwordText.Password))
            {
                MessageBox.Show("Введите логин и пароль!");
                return;
            }

            string passwordHash = GetHash(passwordText.Password);

            using (var db = new Entities())
            {
                var user = db.UserAccounts
                    .AsNoTracking()
                    .FirstOrDefault(u => u.Username == loginTextBox.Text &&
                                       u.PasswordHash == passwordHash);

                if (user == null)
                {
                    MessageBox.Show("Пользователь не найден!");
                    return;
                }

                Window newWindow;
                switch (user.RoleID)
                {
                    case 1:
                        newWindow = new AdminWindow(user);
                        break;
                    case 2:
                        newWindow = new EmployeeWindow(user);
                        break;
                    case 3:
                        newWindow = new ClientWindow(user);
                        break;
                    default:
                        MessageBox.Show("Неизвестная роль пользователя!");
                        return;
                }

                newWindow.Show();
                Application.Current.MainWindow.Close();
            }
        }

        private void NavigateToReg_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegPage());
        }

        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(hash
                    .ComputeHash(Encoding.UTF8.GetBytes(password))
                    .Select(x => x.ToString("X2")));
            }
        }
    }
}

