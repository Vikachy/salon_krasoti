using System;
using System.Collections.Generic;
using System.Linq;
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

namespace salon_krasoti.PagesClient
{
    /// <summary>
    /// Логика взаимодействия для MyReviewsPage.xaml
    /// </summary>
    public partial class MyReviewsPage : Page
    {
        private int _currentClientId; 

        public MyReviewsPage(int clientId)
        {
            InitializeComponent();
            _currentClientId = clientId;
            LoadReviews();
        }

        private void LoadReviews()
        {
            try
            {
                // Загрузка отзывов для текущего клиента
                var reviews = Entities.GetContext().Reviews
                    .Where(r => r.ClientID == _currentClientId)
                    .Select(r => new
                    {
                        r.ReviewID,
                        ServiceName = r.Services.ServiceName,
                        r.Rating,
                        r.Comment
                    })
                    .ToList();

                if (reviews.Any())
                {
                    // Если отзывы есть, показываем таблицу
                    DataGridReviews.ItemsSource = reviews;
                    NoReviewsMessage.Visibility = Visibility.Collapsed; // Скрываем сообщение
                }
                else
                {
                    // Если отзывов нет, показываем сообщение
                    DataGridReviews.Visibility = Visibility.Collapsed; // Скрываем таблицу
                    NoReviewsMessage.Visibility = Visibility.Visible; // Показываем сообщение
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке отзывов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

