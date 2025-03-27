using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace salon_krasoti.Pages
{
    public partial class ReportsPage : Page
    {
        public class ClientInfo
        {
            public int ClientID { get; set; }
            public string FullName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        private List<ClientInfo> _allClients;
        private List<Services> _allServices;

        public ReportsPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var context = Entities.GetContext();

                // Загрузка клиентов
                _allClients = context.Clients
                    .Select(c => new ClientInfo
                    {
                        ClientID = c.ClientID,
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        FullName = c.FirstName + " " + c.LastName
                    })
                    .ToList();

                cbClients.ItemsSource = _allClients;

                // Загрузка услуг
                _allServices = context.Services.ToList();
                cbServices.ItemsSource = _allServices;

                // Установка дат по умолчанию
                dpStartDate.SelectedDate = DateTime.Today.AddMonths(-1);
                dpEndDate.SelectedDate = DateTime.Today;

                LoadReports();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CbClients_TextChanged(object sender, TextChangedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            var text = comboBox.Text.ToLower();

            if (string.IsNullOrWhiteSpace(text))
            {
                comboBox.ItemsSource = _allClients;
                return;
            }

            comboBox.ItemsSource = _allClients
                .Where(c => c.FullName.ToLower().Contains(text) ||
                           c.FirstName.ToLower().Contains(text) ||
                           c.LastName.ToLower().Contains(text))
                .ToList();

            comboBox.IsDropDownOpen = true;
        }

        private void CbServices_TextChanged(object sender, TextChangedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            var text = comboBox.Text.ToLower();

            if (string.IsNullOrWhiteSpace(text))
            {
                comboBox.ItemsSource = _allServices;
                return;
            }

            comboBox.ItemsSource = _allServices
                .Where(s => s.ServiceName.ToLower().Contains(text))
                .ToList();

            comboBox.IsDropDownOpen = true;
        }

        private void CbClients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbClients.SelectedItem != null)
            {
                var selected = (ClientInfo)cbClients.SelectedItem;
                cbClients.Text = selected.FullName;
            }
        }

        private void CbServices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbServices.SelectedItem != null)
            {
                var selected = (Services)cbServices.SelectedItem;
                cbServices.Text = selected.ServiceName;
            }
        }

        private void ApplyFilters_Click(object sender, RoutedEventArgs e)
        {
            LoadReports();
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            dpStartDate.SelectedDate = DateTime.Today.AddMonths(-1);
            dpEndDate.SelectedDate = DateTime.Today;
            cbClients.SelectedItem = null;
            cbClients.Text = "";
            cbServices.SelectedItem = null;
            cbServices.Text = "";

            LoadReports();
        }

        private void LoadReports()
        {
            LoadServicesReport();
            LoadSalesReport();
        }

        private void LoadServicesReport()
        {
            try
            {
                var context = Entities.GetContext();
                var startDate = dpStartDate.SelectedDate;
                var endDate = dpEndDate?.SelectedDate?.AddDays(1);

                var query = from a in context.Appointments
                            join c in context.Clients on a.ClientID equals c.ClientID
                            join s in context.Services on a.ServiceID equals s.ServiceID
                            select new
                            {
                                ClientFullName = c.FirstName + " " + c.LastName,
                                a.AppointmentDateTime,
                                s.ServiceName,
                                s.Price,
                                a.Status
                            };

                if (startDate != null)
                    query = query.Where(a => a.AppointmentDateTime >= startDate);

                if (endDate != null)
                    query = query.Where(a => a.AppointmentDateTime < endDate);

                if (!string.IsNullOrWhiteSpace(cbClients.Text) && cbClients.SelectedItem != null)
                {
                    var selectedClient = (ClientInfo)cbClients.SelectedItem;
                    query = query.Where(a => a.ClientFullName == selectedClient.FullName);
                }

                if (!string.IsNullOrWhiteSpace(cbServices.Text) && cbServices.SelectedItem != null)
                {
                    var selectedService = (Services)cbServices.SelectedItem;
                    query = query.Where(a => a.ServiceName == selectedService.ServiceName);
                }

                dgServicesReport.ItemsSource = query.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки отчета об услугах: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadSalesReport()
        {
            try
            {
                var context = Entities.GetContext();
                var startDate = dpStartDate.SelectedDate;
                var endDate = dpEndDate?.SelectedDate?.AddDays(1);

                var query = from s in context.Sales
                            join e in context.Employees on s.EmployeeID equals e.EmployeeID
                            join sv in context.Services on s.ServiceID equals sv.ServiceID
                            select new
                            {
                                sv.ServiceName,
                                s.SaleDate,
                                s.QuantitySold,
                                TotalAmount = sv.Price * s.QuantitySold,
                                EmployeeFullName = e.FirstName + " " + e.LastName
                            };

                if (startDate != null)
                    query = query.Where(s => s.SaleDate >= startDate);

                if (endDate != null)
                    query = query.Where(s => s.SaleDate < endDate);

                if (!string.IsNullOrWhiteSpace(cbServices.Text) && cbServices.SelectedItem != null)
                {
                    var selectedService = (Services)cbServices.SelectedItem;
                    query = query.Where(s => s.ServiceName == selectedService.ServiceName);
                }

                dgSalesReport.ItemsSource = query.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки отчета о продажах: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}