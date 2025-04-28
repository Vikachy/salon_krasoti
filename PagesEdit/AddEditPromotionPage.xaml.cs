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
    /// Логика взаимодействия для AddEditPromotionPage.xaml
    /// </summary>
    public partial class AddEditPromotionPage : Page
    {
        private Promotions _promotion;

        public AddEditPromotionPage(Promotions promotion)
        {
            InitializeComponent();
            _promotion = promotion;

            if (_promotion != null)
            {
                // Заполняем поля данными для редактирования
                TextBoxPromotionName.Text = _promotion.PromotionName;
                TextBoxDiscountPercentage.Text = _promotion.DiscountPercentage.ToString();
                DatePickerStartDate.SelectedDate = _promotion.StartDate;
                DatePickerEndDate.SelectedDate = _promotion.EndDate;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var context = Entities.GetContext();

                if (_promotion == null)
                {
                    // Добавляем новую акцию
                    _promotion = new Promotions
                    {
                        PromotionName = TextBoxPromotionName.Text,
                        DiscountPercentage = int.Parse(TextBoxDiscountPercentage.Text),
                        StartDate = DatePickerStartDate.SelectedDate.Value,
                        EndDate = DatePickerEndDate.SelectedDate.Value
                    };
                    context.Promotions.Add(_promotion);
                }
                else
                {
                    // Редактируем существующую акцию
                    _promotion.PromotionName = TextBoxPromotionName.Text;
                    _promotion.DiscountPercentage = int.Parse(TextBoxDiscountPercentage.Text);
                    _promotion.StartDate = DatePickerStartDate.SelectedDate.Value;
                    _promotion.EndDate = DatePickerEndDate.SelectedDate.Value;
                }

                context.SaveChanges();
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении акции: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack(); 
        }
    }
}
