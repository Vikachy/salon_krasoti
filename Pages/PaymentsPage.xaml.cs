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
    /// Логика взаимодействия для PaymentsPage.xaml
    /// </summary>
    public partial class PaymentsPage : Page
    {

        public PaymentsPage()
        {
            InitializeComponent();
            LoadPayments();
        }

        private void LoadPayments()
        {
            var payments = Entities.GetContext().Payments
                    .Select(p => new
                    {
                        p.PaymentID,
                        ServiceName = p.Services.ServiceName, 
                        p.Amount,
                        p.PaymentDate
                    })
                    .ToList();

            // Логирование для отладки
            foreach (var payment in payments)
            {
                Console.WriteLine($"PaymentID: {payment.PaymentID}, ServiceName: {payment.ServiceName}, Amount: {payment.Amount}, Date: {payment.PaymentDate}");
            }

            DataGridPayments.ItemsSource = payments;
        }

            private void EditPayment_Click(object sender, RoutedEventArgs e)
        {
            var selectedPayment = DataGridPayments.SelectedItem as dynamic; 
            if (selectedPayment != null)
            {
                int paymentId = selectedPayment.PaymentID;
                var payment = Entities.GetContext().Payments.Find(paymentId); 
                if (payment != null)
                {
                    NavigationService.Navigate(new PagesEdit.AddEditPaymentPage(payment));
                }
                else
                {
                    MessageBox.Show("Платеж не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Выберите платеж для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AddPayment_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PagesEdit.AddEditPaymentPage(null));
        }

        private void DeletePayment_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridPayments.SelectedItem is object selectedPayment)
            {
                if (selectedPayment.GetType().GetProperty("PaymentID") != null)
                {
                    int paymentId = (int)selectedPayment.GetType().GetProperty("PaymentID").GetValue(selectedPayment);

                    try
                    {
                        var payment = Entities.GetContext().Payments.Find(paymentId);

                        if (payment != null)
                        {
                            var result = MessageBox.Show($"Удалить платеж с ID {paymentId}?",
                                                        "Подтверждение удаления",
                                                        MessageBoxButton.YesNo,
                                                        MessageBoxImage.Question);

                            if (result == MessageBoxResult.Yes)
                            {
                                try
                                {
                                    Entities.GetContext().Payments.Remove(payment);
                                    Entities.GetContext().SaveChanges();
                                    LoadPayments();
                                    MessageBox.Show("Платеж успешно удален.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
                                {
                                    MessageBox.Show($"Невозможно удалить платеж: {ex.InnerException?.InnerException?.Message}",
                                                    "Ошибка",
                                                    MessageBoxButton.OK,
                                                    MessageBoxImage.Error);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Платеж не найден.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при обработке платежа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Невозможно удалить платеж.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите платеж для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

    private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                // Обновляем данные из базы данных
                Entities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                LoadPayments();
            }
        }
    }

}

