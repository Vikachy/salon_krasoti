using salon_krasoti.PagesEdit;
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
            NavigationService.Navigate(new PagesEdit.AddEditReviewPage(null));
        }

        private void EditReview_Click(object sender, RoutedEventArgs e)
        {
            var selectedReview = DataGridReviews.SelectedItem as dynamic;
            if (selectedReview == null)
            {
                MessageBox.Show("Выберите отзыв для редактирования.");
                return;
            }

            // Находим отзыв в базе данных по его ID
            var review = Entities.GetContext().Reviews.Find(selectedReview.ReviewID);
            if (review == null)
            {
                MessageBox.Show("Отзыв не найден.");
                return;
            }

            // Передаем отзыв на страницу редактирования
            NavigationService.Navigate(new AddEditReviewPage(review));
        }

        private void DeleteReview_Click(object sender, RoutedEventArgs e)
        {
            var selectedReview = DataGridReviews.SelectedItem as dynamic;
            if (selectedReview == null)
            {
                MessageBox.Show("Выберите отзыв для удаления.");
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите удалить этот отзыв?", "Подтверждение", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var review = Entities.GetContext().Reviews.Find(selectedReview.ReviewID);
                    Entities.GetContext().Reviews.Remove(review);
                    Entities.GetContext().SaveChanges();
                    LoadReviews(); // Обновляем данные
                    MessageBox.Show("Отзыв удален.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}");
                }
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                // Обновляем данные из базы данных
                Entities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                LoadReviews();
            }
        }
    }
}

