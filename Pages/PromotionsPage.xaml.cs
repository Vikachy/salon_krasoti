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
    /// Логика взаимодействия для PromotionsPage.xaml
    /// </summary>
    public partial class PromotionsPage : Page
    {
        public PromotionsPage()
        {
            InitializeComponent();
            DataGridPromotions.ItemsSource = Entities.GetContext().Promotions.ToList();
        }

        private void AddPromotion_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditPromotion_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void DeletePromotion_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}

