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
    public partial class SalesPage : Page
    {
        public SalesPage()
        {
            InitializeComponent();
            LoadSales();
        }

        private void LoadSales()
        {
            // Загрузка данных с использованием навигационных свойств
            var sales = Entities.GetContext().Sales
            .Select(s => new
            {
                SaleID = s.SaleID,
                EmployeeName = s.Employees.FirstName + " " + s.Employees.LastName,
                ServiceName = s.Services.ServiceName,
                SaleDate = s.SaleDate,
                QuantitySold = s.QuantitySold
            })
            .ToList();

            DataGridSales.ItemsSource = sales;
        }

        private void EditSale_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridSales.SelectedItem is object selectedSale)
            {
                var saleId = selectedSale.GetType().GetProperty("SaleID")?.GetValue(selectedSale) as int?;

                if (saleId.HasValue)
                {
                    var sale = Entities.GetContext().Sales.Find(saleId.Value);

                    if (sale != null)
                    {
                        NavigationService.Navigate(new PagesEdit.AddEditSalePage(sale));
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите запись для редактирования");
            }
        }

        private void DeleteSale_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridSales.SelectedItem is object selectedSale)
            {
                if (MessageBox.Show("Удалить выбранную запись?", "Подтверждение",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        var saleId = selectedSale.GetType().GetProperty("SaleID")?.GetValue(selectedSale) as int?;

                        if (saleId.HasValue)
                        {
                            var sale = Entities.GetContext().Sales.Find(saleId.Value);

                            if (sale != null)
                            {
                                Entities.GetContext().Sales.Remove(sale);
                                Entities.GetContext().SaveChanges();
                                LoadSales();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка удаления: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите запись для удаления");
            }
        }

        private void AddSale_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PagesEdit.AddEditSalePage(null));
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Entities.GetContext().ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                LoadSales();
            }
        }
    }
}