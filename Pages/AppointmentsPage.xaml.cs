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
            DataGridAppointments.ItemsSource = Entities.GetContext().Appointments.ToList();
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
