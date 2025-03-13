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

namespace salon_krasoti.Pages
{
    /// <summary>
    /// Логика взаимодействия для ReviewsPage.xaml
    /// </summary>
    public partial class ReviewsPage : Page
    {
        public ReviewsPage()
        {
            InitializeComponent();
            LoadReviews();
        }

        private void LoadReviews()
        {
            // Используем LINQ-запрос для загрузки данных
            var reviews = from review in Entities.GetContext().Reviews
                          join client in Entities.GetContext().Clients on review.ClientID equals client.ClientID
                          join service in Entities.GetContext().Services on review.ServiceID equals service.ServiceID
                          select new
                          {
                              ReviewID = review.ReviewID,
                              ClientName = client.FirstName + " " + client.LastName,
                              ServiceName = service.ServiceName,
                              Rating = review.Rating,
                              Comment = review.Comment
                          };
            DataGridReviews.ItemsSource = reviews.ToList();
        }

        private void AddReview_Click(object sender, RoutedEventArgs e)
        {
            // Логика добавления отзыва
            MessageBox.Show("Добавление отзыва");
        }

        private void EditReview_Click(object sender, RoutedEventArgs e)
        {
            // Логика редактирования отзыва
        }

        private void DeleteReview_Click(object sender, RoutedEventArgs e)
        {
            // Логика удаления отзыва
        }
    }
}

