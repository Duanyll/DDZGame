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

namespace GameClient
{
    /// <summary>
    /// CardView.xaml 的交互逻辑
    /// </summary>
    public partial class CardView : UserControl
    {
        public CardView()
        {
            InitializeComponent();
        }

        private void Card_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ChkBox.IsChecked = !ChkBox.IsChecked;
        }

        private void ChkBox_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
