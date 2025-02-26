using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
    /// Логика взаимодействия для AppointmentsPage.xaml
    /// </summary>
    public partial class AppointmentsPage : Page
    {
        public AppointmentsPage()
        {
            InitializeComponent();
            LoadAppointments();
        }

        private void LoadAppointments()
        {
            // Используем LINQ-запрос для загрузки данных
            var appointments = from appointment in Entities.GetContext().Appointments
                               join client in Entities.GetContext().Clients on appointment.ClientID equals client.ClientID
                               join employee in Entities.GetContext().Employees on appointment.EmployeeID equals employee.EmployeeID
                               join service in Entities.GetContext().Services on appointment.ServiceID equals service.ServiceID
                               select new
                               {
                                   AppointmentID = appointment.AppointmentID,
                                   ClientName = client.FirstName + " " + client.LastName,
                                   EmployeeName = employee.FirstName + " " + employee.LastName,
                                   ServiceName = service.ServiceName,
                                   AppointmentDateTime = appointment.AppointmentDateTime,
                                   Status = appointment.Status
                               };

            // Привязываем данные к DataGrid
            DataGridAppointments.ItemsSource = appointments.ToList();
        }


        private void AddAppointment_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditAppointment_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteAppointment_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
