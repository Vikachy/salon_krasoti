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
    /// Логика взаимодействия для AddReviewPage.xaml
    /// </summary>
    public partial class AddReviewPage : Page
    {
        private int _clientId;
        private int _serviceId;

        public AddReviewPage(int clientId, int serviceId)
        {
            InitializeComponent();
            _clientId = clientId;
            _serviceId = serviceId;
        }

        private void SaveReview_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (RatingComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите рейтинг.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int rating = int.Parse(((ComboBoxItem)RatingComboBox.SelectedItem).Content.ToString());
                string comment = CommentTextBox.Text;

                var review = new Reviews
                {
                    ClientID = _clientId,
                    ServiceID = _serviceId,
                    Rating = rating,
                    Comment = comment
                };

                Entities.GetContext().Reviews.Add(review);
                Entities.GetContext().SaveChanges();

                MessageBox.Show("Отзыв успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении отзыва: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

    }
}
