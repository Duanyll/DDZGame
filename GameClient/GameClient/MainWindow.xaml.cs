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
using System.Windows.Threading;

namespace GameClient
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        NetClient.NetworkClient client = new NetClient.NetworkClient();
        int MyID = -1;
        string UserName;
        DispatcherTimer timer = new DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();
            snakebar.MessageQueue = new MaterialDesignThemes.Wpf.SnackbarMessageQueue();
            client.FailureCaused += NetWorkFailure;
            client.MessageRecieved += MsgRecieved;
            TBUserName.Text = Properties.Settings.Default.LastUserName;
            TBServerIP.Text = Properties.Settings.Default.LastServerIP;
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;
        }

        int CountDown = 60;
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (CountDown > 0)
            {
                CountDown--;
                TBCountDown.Text = CountDown.ToString();
            }
            else
            {
                TBCountDown.Text = "";
                timer.Stop();
            }
        }

        private void BtnSendChat_Click(object sender, RoutedEventArgs e)
        {
#if DEBUG
            string[] vs = TBChat.Text.Split(' ');
            if (vs.Count() == 3 && vs[0] == "$connect$")
            {
                UserName = vs[1];
                client.Connect(vs[1], vs[2]);
                return;
            }
            if (vs.Count() == 1 && vs[0] == "$stop$")
            {
                client.Stop();
                return;
            }
            if (vs.Count()>1 && vs[0] == "$send$")
            {
                client.SendMessage(vs[1]);
                return;
            }
            if (vs.Count()>1 && vs[0] == "$recv$")
            {
                MsgRecieved(vs[1]);
                return;
            }
#endif
            if (!string.IsNullOrWhiteSpace(TBChat.Text))
            {
                if (TBChat.Text.Contains("|")||TBChat.Text.Contains(":")||TBChat.Text.Contains("$"))
                {
                    snakebar.MessageQueue.Enqueue("包含非法字符");
                    return;
                }
                client.SendMessage("CHAT|" + TBChat.Text);
                TBChat.Clear();
            }
            else
            {
                snakebar.MessageQueue.Enqueue("请输入要发送的内容");
            }
            
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

        private void NetWorkFailure(string msg)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                snakebar.MessageQueue.Enqueue(msg);
            })); 
        }

        private void MsgRecieved(string msg)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                string[] vs = msg.Split('|');
                switch (vs[0])
                {
                    case "CHAT":
                        vs = vs[1].Split(':');
                        ICChat.Items.Add(new TextBlock
                        {
                            Text = vs[0] + " " + DateTime.Now.ToString(),
                            Foreground = Brushes.DarkGray,
                            TextWrapping = TextWrapping.Wrap
                        });
                        ICChat.Items.Add(new TextBlock
                        {
                            Text = vs[1],
                            TextWrapping = TextWrapping.Wrap
                        });
                        break;
                    case "COWN":
                        if (vs[1].Length % 2 != 0)
                        {
                            NetWorkFailure("收到非法内容");
                            return;
                        }
                        LVCardOwn.Items.Clear();
                        for(int i = 0; i < vs[1].Length; i+=2)
                        {
                            LVCardOwn.Items.Add(new CardView(vs[1][i], vs[1][i + 1]));
                        }
                        break;
                    case "CLST":
                        vs = vs[1].Split(':');
                        ICChat.Items.Add(new TextBlock
                        {
                            Text = vs[0] + " " + DateTime.Now.ToString(),
                            Foreground = Brushes.DarkGray,
                            TextWrapping = TextWrapping.Wrap
                        });
                        if (vs[1].Length % 2 != 0)
                        {
                            NetWorkFailure("收到非法内容");
                            return;
                        }
                        if(vs[1].Length > 0)
                        {
                            LVCardLast.Items.Clear();
                            WrapPanel panel = new WrapPanel();
                            for (int i = 0; i < vs[1].Length; i += 2)
                            {
                                LVCardLast.Items.Add(new MidCardView(vs[1][i], vs[1][i + 1]));
                                panel.Children.Add(new SmallCardView(vs[1][i], vs[1][i + 1]));
                            }
                            ICChat.Items.Add(panel);
                        }
                        break;
                    case "NPLR":
                        SetNowPlayer(int.Parse(vs[1]));
                        break;
                    case "GCRD":
                        BtnSendCard.IsEnabled = true;
                        CountDown = 60;
                        timer.Start();
                        //BtnSendCard.Effect.
                        break;
                    case "SMSG":
                        snakebar.MessageQueue.Enqueue(vs[1]);
                        break;
                    case "SLOG":
                        ICChat.Items.Add(new TextBlock
                        {
                            Text = DateTime.Now.ToString(),
                            Foreground = Brushes.DarkGray,
                            TextWrapping = TextWrapping.Wrap
                        });
                        ICChat.Items.Add(new TextBlock
                        {
                            Text = vs[1],
                            TextWrapping = TextWrapping.Wrap
                        });
                        break;
                    case "QLDL":
                        if(MessageBox.Show("你想要地主吗", UserName, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            client.SendMessage("LDOK|");
                        }
                        else
                        {
                            client.SendMessage("LDNO|");
                        }
                        break;
                    case "PNAM":
                        TBName1.Text = vs[1] + ((vs[1] == UserName) ? "(YOU)" : "");
                        TBName2.Text = vs[2] + ((vs[2] == UserName) ? "(YOU)" : "");
                        TBName3.Text = vs[3] + ((vs[3] == UserName) ? "(YOU)" : "");
                        break;
                    case "CBAS":
                        WPBaseCard.Children.Clear();
                        WPBaseCard.Children.Add(new SmallCardView(vs[1][0], vs[1][1]));
                        WPBaseCard.Children.Add(new SmallCardView(vs[1][2], vs[1][3]));
                        WPBaseCard.Children.Add(new SmallCardView(vs[1][4], vs[1][5]));
                        break;
                    case "PLDL":
                        switch (int.Parse(vs[1]))
                        {
                            case 0:
                                TBName1.Text += "(♚)";
                                break;
                            case 1:
                                TBName2.Text += "(♚)";
                                break;
                            case 2:
                                TBName3.Text += "(♚)";
                                break;
                        }
                        break;
                    case "SGCR":
                        BtnSendCard.IsEnabled = false;
                        break;
                    case "GRDY":
                        GBStartGame.Visibility = Visibility.Visible;
                        break;
                    case "SGRD":
                        GBStartGame.Visibility = Visibility.Collapsed;
                        break;
                    case "PSCR":
                        TBScore1.Text = vs[1];
                        TBScore2.Text = vs[2];
                        TBScore3.Text = vs[3];
                        break;
                }
            }));
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            client.Stop();
        }

        private void LVCardOwn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(LVCardOwn.SelectedItems.Count == 0)
            {
                PIBtnSend.Kind = MaterialDesignThemes.Wpf.PackIconKind.SkipNext;
            }
            else
            {
                PIBtnSend.Kind = MaterialDesignThemes.Wpf.PackIconKind.Send;
            }
        }

        private void BtnSendCard_Click(object sender, RoutedEventArgs e)
        {
            string sel = "";
            foreach(object i in LVCardOwn.SelectedItems)
            {
                CardView card = i as CardView;
                if(card != null)
                {
                    sel += card.ToString();
                }
            }
            timer.Stop();
            TBCountDown.Text = "";
            client.SendMessage("SLCT|" + sel);
            BtnSendCard.IsEnabled = false;
        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (TBUserName.Text != "" && TBServerIP.Text != "")
            {
                Properties.Settings.Default.LastUserName = TBUserName.Text;
                Properties.Settings.Default.LastServerIP = TBServerIP.Text;
                UserName = TBUserName.Text;
                if(client.Connect(TBUserName.Text, TBServerIP.Text))
                {
                    Properties.Settings.Default.Save();
                }
            }
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            client.Stop();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ICChat.Items.Add(new TextBlock
            {
                Text = "当前程序版本:" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version,
                TextWrapping = TextWrapping.Wrap
            });
        }

        private void ICChat_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SVChat.ScrollToBottom();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            snakebar.MessageQueue.Enqueue("确实要清除记录吗？", "清除", ICChat.Items.Clear);
        }

        private void BtnStartGame_Click(object sender, RoutedEventArgs e)
        {
            client.SendMessage("REDY|");
            GBStartGame.Visibility = Visibility.Collapsed;
        }
    }
}
