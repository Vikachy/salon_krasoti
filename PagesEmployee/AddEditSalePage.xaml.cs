using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    /// Логика взаимодействия для AddEditSalePage.xaml
    /// </summary>
    public partial class AddEditSalePage : Page
    {
        private int _currentEmployeeId;
        private Sales _currentSale;

        public AddEditSalePage(int employeeId, Sales selectedSale = null)
        {
            InitializeComponent();
            _currentEmployeeId = employeeId;
            _currentSale = selectedSale ?? new Sales();

            LoadData();
        }

        private void LoadData()
        {
            using (var context = new Entities())
            {
                // Загрузка услуг
                ComboBoxServices.ItemsSource = context.Services.ToList();

                // Если это редактирование, заполняем поля
                if (_currentSale.SaleID != 0)
                {
                    ComboBoxServices.SelectedValue = _currentSale.ServiceID;
                    DatePickerSaleDate.SelectedDate = _currentSale.SaleDate;
                    TextBoxQuantity.Text = _currentSale.QuantitySold.ToString();
                }
                else
                {
                    // Если это новая продажа, устанавливаем текущую дату
                    DatePickerSaleDate.SelectedDate = DateTime.Now;
                }
            }
        }

        private void SaveSale_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка заполнения полей
                if (ComboBoxServices.SelectedItem == null ||
                    DatePickerSaleDate.SelectedDate == null ||
                    string.IsNullOrWhiteSpace(TextBoxQuantity.Text) ||
                    !int.TryParse(TextBoxQuantity.Text, out int quantity))
                {
                    MessageBox.Show("Заполните все обязательные поля корректно!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Сохранение данных
                _currentSale.ServiceID = (int)ComboBoxServices.SelectedValue;
                _currentSale.SaleDate = DatePickerSaleDate.SelectedDate.Value;
                _currentSale.QuantitySold = quantity;
                _currentSale.EmployeeID = _currentEmployeeId;

                using (var context = new Entities())
                {
                    if (_currentSale.SaleID == 0)
                    {
                        context.Sales.Add(_currentSale);
                    }
                    else
                    {
                        context.Entry(_currentSale).State = EntityState.Modified;
                    }
                    context.SaveChanges();
                }

                MessageBox.Show("Продажа успешно сохранена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении продажи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
