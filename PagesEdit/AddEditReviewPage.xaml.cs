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

namespace salon_krasoti.PagesEdit
{
    /// <summary>
    /// Логика взаимодействия для AddEditReviewPage.xaml
    /// </summary>
    public partial class AddEditReviewPage : Page
    {
        private Reviews _currentReview = new Reviews();

        public AddEditReviewPage(Reviews selectedReview = null)
        {
            InitializeComponent();

            if (selectedReview != null)
            {
                _currentReview = selectedReview;
            }

            DataContext = _currentReview;
            LoadClients();
            LoadServices();

            if (_currentReview.ClientID > 0)
            {
                ClientComboBox.SelectedValue = _currentReview.ClientID;
            }
            if (_currentReview.ServiceID > 0)
            {
                ServiceComboBox.SelectedValue = _currentReview.ServiceID;
            }
            RatingTextBox.Text = _currentReview.Rating.ToString();
            CommentTextBox.Text = _currentReview.Comment;
        }

        private void LoadClients()
        {
            ClientComboBox.ItemsSource = Entities.GetContext().Clients.ToList();
            ClientComboBox.DisplayMemberPath = "ClientFullName"; 
            ClientComboBox.SelectedValuePath = "ClientID";
        }

        private void LoadServices()
        {
            ServiceComboBox.ItemsSource = Entities.GetContext().Services.ToList();
            ServiceComboBox.DisplayMemberPath = "ServiceName";
            ServiceComboBox.SelectedValuePath = "ServiceID";
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (ClientComboBox.SelectedItem == null || ServiceComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите клиента и услугу.");
                return;
            }

            _currentReview.ClientID = (int)ClientComboBox.SelectedValue;
            _currentReview.ServiceID = (int)ServiceComboBox.SelectedValue;
            _currentReview.Rating = int.Parse(RatingTextBox.Text);
            _currentReview.Comment = CommentTextBox.Text;

            if (_currentReview.ReviewID == 0)
            {
                Entities.GetContext().Reviews.Add(_currentReview);
            }

            try
            {
                Entities.GetContext().SaveChanges();
                MessageBox.Show("Отзыв сохранен.");
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }

            private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

    }
}
