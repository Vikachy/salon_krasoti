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
using System.Windows.Shapes;

namespace salon_krasoti
{
    /// <summary>
    /// Логика взаимодействия для EmployeeWindow.xaml
    /// </summary>
    public partial class EmployeeWindow : Window
    {
        private UserAccounts _user;

        public EmployeeWindow(UserAccounts user)
        {
            InitializeComponent();
            _user = user;
            SetPermissions(user);
            // Инициализация данных сотрудника
        }
        private void SetPermissions(UserAccounts user)
        {
            if (user.RoleID == 2)
            {
                // Скрыть ненужные элементы
                ProductsButton.Visibility = Visibility.Collapsed;
            }
        }
    }
}
