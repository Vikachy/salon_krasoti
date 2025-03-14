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
            // Используем LINQ-запрос для загрузки данных
            var payments = from payment in Entities.GetContext().Payments
                           join service in Entities.GetContext().Services on payment.ServiceID equals service.ServiceID
                           select new
                           {
                               PaymentID = payment.PaymentID,
                               Service = service,
                               Amount = payment.Amount,
                               PaymentDate = payment.PaymentDate
                           };

            DataGridPayments.ItemsSource = payments.ToList();
        }

        private void EditPayment_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddPayment_Click(object sender, RoutedEventArgs e)
        {
            // Логика добавления платежа
            MessageBox.Show("Добавление платежа");
        }

        private void DeletePayment_Click(object sender, RoutedEventArgs e)
        {
           
        }
    }

}

