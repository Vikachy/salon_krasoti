using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace salon_krasoti
{
    public partial class AuthPage : Page
    {
        public int FailedLoginAttempts { get; private set; } = 0;
        public string GeneratedCaptcha { get; private set; } = "";
        public const int MaxAttemptsBeforeCaptcha = 3;

        // Публичные свойства для доступа к элементам UI из тестов
        public TextBox LoginTextBox => loginTextBox;
        public PasswordBox PasswordBox => passwordText;
        public StackPanel CaptchaPanel => captchaPanel;
        public TextBox CaptchaTextBox => captchaText;
        public Canvas CaptchaCanvas => captchaCanvas;

        public AuthPage()
        {
            InitializeComponent();
            captchaPanel.Visibility = Visibility.Collapsed;
        }

        private void signInButton_Click(object sender, RoutedEventArgs e)
        {
            bool success = PerformLogin();

            if (success)
            {
                var user = GetAuthenticatedUser(LoginTextBox.Text, PasswordBox.Password);
                if (user != null)
                {
                    OpenUserWindow(user);
                }
            }
        }

        public bool AttemptLogin(string login, string password, string captcha = null)
        {
            LoginTextBox.Text = login;
            PasswordBox.Password = password;

            if (CaptchaPanel.Visibility == Visibility.Visible)
            {
                if (string.IsNullOrEmpty(captcha))
                    return false;
                CaptchaTextBox.Text = captcha;
            }

            return PerformLogin();
        }

        private bool PerformLogin()
        {
            if (CaptchaPanel.Visibility == Visibility.Visible &&
                CaptchaTextBox.Text != GeneratedCaptcha)
            {
                MessageBox.Show("Неверная CAPTCHA!");
                GenerateNewCaptcha();
                return false;
            }

            if (string.IsNullOrEmpty(LoginTextBox.Text) ||
                string.IsNullOrEmpty(PasswordBox.Password))
            {
                MessageBox.Show("Введите логин и пароль!");
                return false;
            }

            string passwordHash = GetHash(PasswordBox.Password);

            using (var db = new Entities())
            {
                var user = db.UserAccounts
                    .AsNoTracking()
                    .FirstOrDefault(u => u.Username == LoginTextBox.Text &&
                                       u.PasswordHash == passwordHash);

                if (user == null)
                {
                    FailedLoginAttempts++;

                    if (FailedLoginAttempts >= MaxAttemptsBeforeCaptcha)
                    {
                        CaptchaPanel.Visibility = Visibility.Visible;
                        GenerateNewCaptcha();
                    }

                    MessageBox.Show("Неверный логин или пароль!");
                    return false;
                }

                // Успешная авторизация
                FailedLoginAttempts = 0;
                CaptchaPanel.Visibility = Visibility.Collapsed;
                return true;
            }
        }

        private UserAccounts GetAuthenticatedUser(string login, string password)
        {
            string passwordHash = GetHash(password);

            using (var db = new Entities())
            {
                return db.UserAccounts
                    .AsNoTracking()
                    .FirstOrDefault(u => u.Username == login &&
                                       u.PasswordHash == passwordHash);
            }
        }

        private void GenerateNewCaptcha()
        {
            CaptchaCanvas.Children.Clear();
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var random = new Random();

            GeneratedCaptcha = new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            // Отрисовка текста CAPTCHA
            for (int i = 0; i < GeneratedCaptcha.Length; i++)
            {
                var textBlock = new TextBlock
                {
                    Text = GeneratedCaptcha[i].ToString(),
                    FontSize = 20 + random.Next(5),
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Color.FromRgb(
                        (byte)random.Next(100, 200),
                        (byte)random.Next(100, 200),
                        (byte)random.Next(100, 200)))
                };

                Canvas.SetLeft(textBlock, 10 + i * 30 + random.Next(-5, 5));
                Canvas.SetTop(textBlock, 15 + random.Next(-10, 10));
                CaptchaCanvas.Children.Add(textBlock);
            }

            // Добавление шума (линии)
            for (int i = 0; i < 8; i++)
            {
                var line = new Line
                {
                    X1 = random.Next(0, 200),
                    Y1 = random.Next(0, 50),
                    X2 = random.Next(0, 200),
                    Y2 = random.Next(0, 50),
                    Stroke = new SolidColorBrush(Color.FromRgb(
                        (byte)random.Next(150, 220),
                        (byte)random.Next(150, 220),
                        (byte)random.Next(150, 220))),
                    StrokeThickness = 1
                };
                CaptchaCanvas.Children.Add(line);
            }

            
            for (int i = 0; i < 30; i++)
            {
                var ellipse = new Ellipse
                {
                    Width = 1,
                    Height = 1,
                    Fill = new SolidColorBrush(Color.FromRgb(
                        (byte)random.Next(150, 220),
                        (byte)random.Next(150, 220),
                        (byte)random.Next(150, 220)))
                };
                Canvas.SetLeft(ellipse, random.Next(0, 200));
                Canvas.SetTop(ellipse, random.Next(0, 50));
                CaptchaCanvas.Children.Add(ellipse);
            }
        }

        private void OpenUserWindow(UserAccounts user)
        {
            Window newWindow = null;

            using (var db = new Entities())
            {
                switch (user.RoleID)
                {
                    case 1: // Администратор
                        newWindow = new AdminWindow(user);
                        break;
                    case 2: // Сотрудник
                        var employee = db.Employees
                            .FirstOrDefault(emp => emp.EmployeeID == user.EmployeeID);
                        if (employee != null)
                            newWindow = new EmployeeWindow(employee);
                        break;
                    case 3: // Клиент
                        newWindow = new ClientWindow(user);
                        break;
                }
            }

            if (newWindow != null)
            {

                var currentWindow = Window.GetWindow(this);

                newWindow.Show();

                currentWindow?.Close();
            }
            else
            {
                MessageBox.Show("Ошибка определения роли пользователя!");
            }
        }

        public static string GetHash(string password)
        {
            using (var sha1 = SHA1.Create())
            {
                return string.Concat(sha1
                    .ComputeHash(Encoding.UTF8.GetBytes(password))
                    .Select(b => b.ToString("X2")));
            }
        }

        private void NavigateToReg_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new RegPage());
        }
    }
}