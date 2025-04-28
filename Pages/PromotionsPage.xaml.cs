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
    /// Логика взаимодействия для PromotionsPage.xaml
    /// </summary>
    public partial class PromotionsPage : Page
    {
        public PromotionsPage()
        {
            InitializeComponent();
            DataGridPromotions.ItemsSource = Entities.GetContext().Promotions.ToList();
        }

        private void AddPromotion_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PagesEdit.AddEditPromotionPage(null));
        }

        private void EditPromotion_Click(object sender, RoutedEventArgs e)
        {
            var selectedPromotion = DataGridPromotions.SelectedItem as Promotions;

            if (selectedPromotion != null)
            {
                NavigationService.Navigate(new PagesEdit.AddEditPromotionPage(selectedPromotion));
            }
            else
            {
                MessageBox.Show("Выберите акцию для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeletePromotion_Click(object sender, RoutedEventArgs e)
        {
            var selectedPromotion = DataGridPromotions.SelectedItem as Promotions;

            if (selectedPromotion != null)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить эту акцию?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var context = Entities.GetContext();
                        context.Promotions.Remove(selectedPromotion);
                        context.SaveChanges();

                        DataGridPromotions.ItemsSource = Entities.GetContext().Promotions.ToList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении акции: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите акцию для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                // Обновляем данные из базы данных
                Entities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                DataGridPromotions.ItemsSource = Entities.GetContext().Promotions.ToList();
            }
        }
    }
}

