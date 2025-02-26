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
    /// Логика взаимодействия для ProductsPage.xaml
    /// </summary>
    public partial class ProductsPage : Page
    {
        public ProductsPage()
        {
            InitializeComponent();
            DataGridProducts.ItemsSource = Entities.GetContext().Products.ToList();
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            // Логика добавления продукта
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridProducts.SelectedItem is Products selectedProduct)
            {
                // Логика редактирования продукта
            }
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridProducts.SelectedItem is Products product)
            {
                var result = MessageBox.Show($"Удалить продукт {product.ProductName}?",
                                            "Подтверждение удаления",
                                            MessageBoxButton.YesNo,
                                            MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        Entities.GetContext().Products.Remove(product);
                        Entities.GetContext().SaveChanges();
                        DataGridProducts.ItemsSource = Entities.GetContext().Products.ToList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите продукт для удаления!", "Предупреждение",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }
    }
}
