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

        public bool Register(string login, string name, string surname, string email, string phone, string password, string confirmPassword)
        {
            // Проверка на пустые поля
            if (string.IsNullOrEmpty(login))
            {
                MessageBox.Show("Укажите логин!");
                return false;
            }

            using (var db = new Entities())
            {
                // Проверка существующего пользователя
                var existingUser = db.UserAccounts
                    .AsNoTracking()
                    .FirstOrDefault(u => u.Username == login);

                if (existingUser != null)
                {
                    MessageBox.Show("Пользователь уже существует!");
                    return false;
                }

                // Проверка совпадения паролей
                if (password != confirmPassword)
                {
                    MessageBox.Show("Пароли не совпадают!");
                    return false;
                }

                // Валидация пароля
                bool hasLetter = false;
                bool hasNumber = false;
                foreach (char c in password)
                {
                    if (char.IsLetter(c)) hasLetter = true;
                    if (char.IsDigit(c)) hasNumber = true;
                }

                var phoneRegex = new Regex(@"^\+7\d{10}$");
                var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

                StringBuilder errors = new StringBuilder();

                if (password.Length < 6)
                    errors.AppendLine("Пароль должен быть длиннее 6 символов");
                if (!phoneRegex.IsMatch(phone))
                    errors.AppendLine("Номер телефона должен быть в формате +7XXXXXXXXXX");
                if (!hasLetter)
                    errors.AppendLine("Пароль должен содержать буквы");
                if (!hasNumber)
                    errors.AppendLine("Пароль должен содержать цифры");
                if (!emailRegex.IsMatch(email))
                    errors.AppendLine("Неверный формат email");

                // Проверка существующего email
                var existingEmail = db.Clients
                    .AsNoTracking()
                    .FirstOrDefault(c => c.Email == email);

                if (existingEmail != null)
                {
                    errors.AppendLine("Email уже используется");
                }

                if (errors.Length > 0)
                {
                    MessageBox.Show(errors.ToString());
                    return false;
                }

                try
                {
                    var newClient = new Clients
                    {
                        FirstName = name,
                        LastName = surname,
                        Email = email,
                        Phone = phone,
                        RoleID = 3
                    };

                    db.Clients.Add(newClient);
                    db.SaveChanges();

                    var newUser = new UserAccounts
                    {
                        Username = login,
                        PasswordHash = AuthPage.GetHash(password),
                        RoleID = 3,
                        ClientID = newClient.ClientID
                    };

                    db.UserAccounts.Add(newUser);
                    db.SaveChanges();

                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при регистрации: {ex.Message}");
                    return false;
                }
            }
        }

        private void signUpButton_Click(object sender, RoutedEventArgs e)
        {
            bool success = Register(
                loginText.Text,
                nameText.Text,
                surnameText.Text,
                emailText.Text,
                phoneText.Text,
                passwordText.Password,
                confirmPasswordText.Password);

            if (success)
            {
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