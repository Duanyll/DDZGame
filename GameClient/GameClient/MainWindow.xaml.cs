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

namespace GameClient
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            snakebar.MessageQueue = new MaterialDesignThemes.Wpf.SnackbarMessageQueue();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnSendChat_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TBChat.Text))
            {
                AddToChatBox(TBChat.Text);
                TBChat.Clear();
            }
            else
            {
                snakebar.MessageQueue.Enqueue("Please enter content to send.");
            }
            
        }

        private void AddToChatBox(string a)
        {
            ICChat.Items.Add(new TextBlock
            {
                Text = a,
                TextWrapping = TextWrapping.Wrap
            });
        }
    }
}
