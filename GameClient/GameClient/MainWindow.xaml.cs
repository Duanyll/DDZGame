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
#if DEBUG
            if(TBChat.Text == "$test$")
            {
                DoTest();
                return;
            }
#endif
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

        Random random = new Random();
        private void DoTest()
        {
            SetNowPlayer(random.Next(3));
        }

        private void SetNowPlayer(int now)
        {
            PIInd1.Visibility = Visibility.Hidden;
            PIInd2.Visibility = Visibility.Hidden;
            PIInd3.Visibility = Visibility.Hidden;
            switch (now)
            {
                case 0:
                    PIInd1.Visibility = Visibility.Visible;
                    break;
                case 1:
                    PIInd2.Visibility = Visibility.Visible;
                    break;
                case 2:
                    PIInd3.Visibility = Visibility.Visible;
                    break;
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
