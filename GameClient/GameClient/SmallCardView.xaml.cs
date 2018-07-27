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
    /// SmallCardView.xaml 的交互逻辑
    /// </summary>
    public partial class SmallCardView : UserControl
    {
        public SmallCardView()
        {
            InitializeComponent();
        }

        public const int COLOR_CNT = 5;
        public const int VALUE_CNT = 14;

        public enum Color
        {
            Hongtao,
            Fangpian,
            Heitao,
            Meihua,
            Joker
        };
        public Color color;
        public enum Value
        {
            Three,
            Four,
            Five,
            Six,
            Seven,
            Eight,
            Nine,
            Ten,
            J,
            Q,
            K,
            A,
            Two,
            Grey,
            Red
        }
        public Value value;
        public override string ToString()
        {
            string ret = "";
            char col = (char)('A' + (int)color);
            ret += col;
            char val = (char)('a' + (int)value);
            ret += val;
            return ret;
        }
        public SmallCardView(char col, char val)
        {
            InitializeComponent();
            color = (Color)(col - 'A');
            if ((int)color > COLOR_CNT)
            {
                throw new ArgumentOutOfRangeException();
            }
            value = (Value)(val - 'a');
            if ((int)value > VALUE_CNT)
            {
                throw new ArgumentOutOfRangeException();
            }
            SetColor();
        }
        public SmallCardView(Color a, Value b)
        {
            InitializeComponent();
            color = a;
            value = b;
            SetColor();
        }

        private void SetColor()
        {
            switch (color)
            {
                case Color.Fangpian:
                    //TBColor.Text = "♦";
                    //TBColor.Foreground = Brushes.Red;
                    TBValue.Foreground = Brushes.Red;
                    break;
                case Color.Heitao:
                    //TBColor.Text = "♠";
                    //TBColor.Foreground = Brushes.Black;
                    TBValue.Foreground = Brushes.Black;
                    break;
                case Color.Hongtao:
                    //TBColor.Text = "♥";
                    //TBColor.Foreground = Brushes.Red;
                    TBValue.Foreground = Brushes.Red;
                    break;
                case Color.Meihua:
                    //TBColor.Text = "♣";
                    //TBColor.Foreground = Brushes.Black;
                    TBValue.Foreground = Brushes.Black;
                    break;
                case Color.Joker:
                    TBValue.Text = "♛";
                    break;
            }
            if (value <= Value.Ten)
            {
                TBValue.Text = ((int)value + 3).ToString();
            }
            else
            {
                switch (value)
                {
                    case Value.J:
                        TBValue.Text = "J";
                        break;
                    case Value.Q:
                        TBValue.Text = "Q";
                        break;
                    case Value.K:
                        TBValue.Text = "K";
                        break;
                    case Value.A:
                        TBValue.Text = "A";
                        break;
                    case Value.Two:
                        TBValue.Text = "2";
                        break;
                    case Value.Grey:
                        //TBValue.Text = "Joker";
                        TBValue.Foreground = Brushes.Black;
                        //TBColor.Foreground = Brushes.Black;
                        break;
                    case Value.Red:
                        //TBValue.Text = "Joker";
                        TBValue.Foreground = Brushes.Red;
                        //TBColor.Foreground = Brushes.Red;
                        break;
                }
            }
        }
    }
}
