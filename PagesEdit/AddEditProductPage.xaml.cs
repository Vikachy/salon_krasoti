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
    /// Логика взаимодействия для AddEditProductPage.xaml
    /// </summary>
    public partial class AddEditProductPage : Page
    {
        private Products _currentProduct;

        public AddEditProductPage(Products selectedProduct)
        {
            InitializeComponent();
            _currentProduct = selectedProduct ?? new Products();

            if (_currentProduct.ProductID != 0)
            {
                ProductNameTextBox.Text = _currentProduct.ProductName;
                PriceTextBox.Text = _currentProduct.Price.ToString();
                StockQuantityTextBox.Text = _currentProduct.StockQuantity.ToString();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _currentProduct.ProductName = ProductNameTextBox.Text;
                _currentProduct.Price = decimal.Parse(PriceTextBox.Text);
                _currentProduct.StockQuantity = int.Parse(StockQuantityTextBox.Text);

                if (_currentProduct.ProductID == 0)
                {
                    Entities.GetContext().Products.Add(_currentProduct);
                }

                Entities.GetContext().SaveChanges();
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack(); 
        }
    }
}
