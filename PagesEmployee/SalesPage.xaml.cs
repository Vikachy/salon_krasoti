﻿using salon_krasoti.PagesEdit;
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

namespace salon_krasoti.PagesEmployee
{
    /// <summary>
    /// Логика взаимодействия для SalesPage.xaml
    /// </summary>
    public partial class SalesPage : Page
    {
        private int _currentEmployeeId;

        public SalesPage(int employeeId)
        {
            InitializeComponent();
            _currentEmployeeId = employeeId;
            LoadSales();
        }

        private void LoadSales()
        {
            try
            {
                using (var context = new Entities())
                {
                    // Загружаем продажи для текущего сотрудника
                    var sales = context.Sales
                        .Where(s => s.EmployeeID == _currentEmployeeId)
                        .ToList();

                    // Создаем список для отображения в DataGrid
                    var salesView = sales.Select(sale => new
                    {
                        SaleID = sale.SaleID,
                        ServiceName = context.Services
                            .FirstOrDefault(service => service.ServiceID == sale.ServiceID)?.ServiceName, // Название услуги
                        SaleDate = sale.SaleDate,
                        QuantitySold = sale.QuantitySold
                    }).ToList();

                    // Привязываем данные к DataGrid
                    DataGridSales.ItemsSource = salesView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке продаж: {ex.Message}");
            }
        }

        private void AddSale_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Переход на страницу добавления продажи
                NavigationService.Navigate(new AddEditSalePage(_currentEmployeeId));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении продажи: {ex.Message}");
            }
        }

        private void EditSale_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получаем выбранный анонимный объект
                var selectedSale = DataGridSales.SelectedItem as dynamic;
                if (selectedSale == null)
                {
                    MessageBox.Show("Выберите продажу для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Получаем ID выбранной продажи
                int saleId = selectedSale.SaleID;

                // Находим продажу в базе данных
                using (var context = new Entities())
                {
                    var saleToEdit = context.Sales.FirstOrDefault(s => s.SaleID == saleId);
                    if (saleToEdit == null)
                    {
                        MessageBox.Show("Продажа не найдена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Переход на страницу редактирования продажи
                    NavigationService.Navigate(new AddEditSalePage(_currentEmployeeId, saleToEdit));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при редактировании продажи: {ex.Message}");
            }
        }

        private void DeleteSale_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получаем выбранный анонимный объект
                var selectedSale = DataGridSales.SelectedItem as dynamic;
                if (selectedSale == null)
                {
                    MessageBox.Show("Выберите продажу для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Получаем ID выбранной продажи
                int saleId = selectedSale.SaleID;

                // Подтверждение удаления
                var result = MessageBox.Show("Вы уверены, что хотите удалить продажу?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    using (var context = new Entities())
                    {
                        // Находим продажу в базе данных
                        var saleToDelete = context.Sales.FirstOrDefault(s => s.SaleID == saleId);
                        if (saleToDelete == null)
                        {
                            MessageBox.Show("Продажа не найдена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        // Удаляем продажу
                        context.Sales.Remove(saleToDelete);
                        context.SaveChanges();
                    }

                    // Обновляем данные
                    LoadSales();
                    MessageBox.Show("Продажа успешно удалена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении продажи: {ex.Message}");
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                // Обновляем данные из базы данных
                Entities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                LoadSales();
            }
        }
    }
}
