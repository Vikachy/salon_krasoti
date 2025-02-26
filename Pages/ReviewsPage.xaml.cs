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
            DataGridReviews.ItemsSource = Entities.GetContext().Reviews
                        .Include("Client") // Загрузка связанных данных
                        .Include("Service") // Загрузка связанных данных
                        .ToList();
        }

        private void AddReview_Click(object sender, RoutedEventArgs e)
        {
            // Логика добавления отзыва
        }
    }
}
