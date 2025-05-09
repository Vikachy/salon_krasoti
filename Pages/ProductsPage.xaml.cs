﻿using System;
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
            NavigationService.Navigate(new PagesEdit.AddEditProductPage(null));
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = DataGridProducts.SelectedItem as Products;
            if (selectedProduct != null)
            {
                NavigationService.Navigate(new PagesEdit.AddEditProductPage(selectedProduct));
            }
            else
            {
                MessageBox.Show("Выберите продукт для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        private void UpdateProducts()
        {
            var currentProducts = Entities.GetContext().Products.ToList();

            // Фильтрация по поиску
            if (!string.IsNullOrWhiteSpace(SearchProductName.Text))
            {
                currentProducts = currentProducts.Where(p =>
                    p.ProductName.ToLower().Contains(SearchProductName.Text.ToLower()) ||
                    p.Price.ToString().Contains(SearchProductName.Text) ||
                    p.StockQuantity.ToString().Contains(SearchProductName.Text)).ToList();
            }

            // Сортировка
            if (SortProductBy.SelectedItem != null)
            {
                switch (((ComboBoxItem)SortProductBy.SelectedItem).Content.ToString())
                {
                    case "По названию (А-Я)":
                        currentProducts = currentProducts.OrderBy(p => p.ProductName).ToList();
                        break;
                    case "По названию (Я-А)":
                        currentProducts = currentProducts.OrderByDescending(p => p.ProductName).ToList();
                        break;
                    case "По цене (↑)":
                        currentProducts = currentProducts.OrderBy(p => p.Price).ToList();
                        break;
                    case "По цене (↓)":
                        currentProducts = currentProducts.OrderByDescending(p => p.Price).ToList();
                        break;
                    case "По количеству (↑)":
                        currentProducts = currentProducts.OrderBy(p => p.StockQuantity).ToList();
                        break;
                    case "По количеству (↓)":
                        currentProducts = currentProducts.OrderByDescending(p => p.StockQuantity).ToList();
                        break;
                }
            }

            DataGridProducts.ItemsSource = currentProducts;
        }

        private void SearchProductName_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateProducts();
        }

        private void SortProductBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProducts();
        }

        private void CleanFilter_Click(object sender, RoutedEventArgs e)
        {
            SearchProductName.Text = "";
            SortProductBy.SelectedIndex = -1;
            UpdateProducts();
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                // Обновляем данные из базы данных
                Entities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                DataGridProducts.ItemsSource = Entities.GetContext().Products.ToList();
            }
        }
    }
}
