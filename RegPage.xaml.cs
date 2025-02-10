using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для RegPage.xaml
    /// </summary>
    public partial class RegPage : Page
    {
        public RegPage()
        {
            InitializeComponent();
        }

        private void signUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (loginText.Text.Length == 0)
            {
                MessageBox.Show("Укажите логин!");
                return;
            }

            using (var db = new Entities())
            {
                var existingUser = db.UserAccounts
                    .AsNoTracking()
                    .FirstOrDefault(u => u.Username == loginText.Text);

                if (existingUser != null)
                {
                    MessageBox.Show("Пользователь уже существует!");
                    return;
                }

                bool hasLetter = false;
                bool hasNumber = false;
                foreach (char c in passwordText.Password)
                {
                    if (char.IsLetter(c)) hasLetter = true;
                    if (char.IsDigit(c)) hasNumber = true;
                }

                var phoneRegex = new Regex(@"^\+7\d{10}$");
                var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

                StringBuilder errors = new StringBuilder();

                if (passwordText.Password.Length < 6)
                    errors.AppendLine("Пароль должен быть длиннее 6 символов");
                if (!phoneRegex.IsMatch(phoneText.Text))
                    errors.AppendLine("Номер телефона должен быть в формате +7XXXXXXXXXX");
                if (!hasLetter)
                    errors.AppendLine("Пароль должен содержать буквы");
                if (!hasNumber)
                    errors.AppendLine("Пароль должен содержать цифры");
                if (!emailRegex.IsMatch(emailText.Text))
                    errors.AppendLine("Неверный формат email");

                if (errors.Length > 0)
                {
                    MessageBox.Show(errors.ToString());
                    return;
                }

                var newClient = new Clients
                {
                    FirstName = nameText.Text,
                    LastName = surnameText.Text,
                    Email = emailText.Text,
                    Phone = phoneText.Text,
                    RoleID = 3
                };

                db.Clients.Add(newClient);
                db.SaveChanges();

                var newUser = new UserAccounts
                {
                    Username = loginText.Text,
                    PasswordHash = AuthPage.GetHash(passwordText.Password),
                    RoleID = 3,
                    UserID = newClient.ClientID 
                };

                db.UserAccounts.Add(newUser);
                db.SaveChanges();

                MessageBox.Show("Регистрация успешна!");
                NavigationService.Navigate(new AuthPage());
            }
        }

        private void NavigateToAuth_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
