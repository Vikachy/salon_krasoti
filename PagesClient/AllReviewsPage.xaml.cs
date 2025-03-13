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
    /// Логика взаимодействия для AllReviewsPage.xaml
    /// </summary>
    public partial class AllReviewsPage : Page
    {
        public AllReviewsPage()
        {
            InitializeComponent();
            LoadReviews();
        }

        private void LoadReviews()
        {
            try
            {
                var reviews = Entities.GetContext().Reviews
                    .Select(r => new
                    {
                        ClientName = r.Clients.FirstName + " " + r.Clients.LastName,
                        ServiceName = r.Services.ServiceName,
                        r.Rating,
                        r.Comment
                    })
                    .ToList();

                DataGridReviews.ItemsSource = reviews;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке отзывов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
