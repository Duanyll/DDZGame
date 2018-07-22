using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace GameServer
{
    class GameruleHandler
    {
        private struct Card:IComparable
        {
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
            public Card(char col,char val)
            {
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
            }
            public Card(Color a,Value b)
            {
                color = a;
                value = b;
            }
            public static bool operator ==(Card A,Card B)
            {
                return A.Equals(B);
            }
            public static bool operator !=(Card A,Card B)
            {
                return !(A == B);
            }
            public static bool operator <(Card A,Card B)
            {
                return A.value < B.value;
            }
            public static bool operator >(Card A,Card B)
            {
                return A.value > B.value;
            }
            public static bool operator >=(Card A,Card B)
            {
                return !(A < B);
            }
            public static bool operator <=(Card A,Card B)
            {
                return !(A > B);
            }
            public int CompareTo(object obj)
            {
                if (obj == null) return 1;
                Card otherCard = (Card)obj;
                if (otherCard == null)
                    throw new ArgumentNullException();
                else
                    return this.value.CompareTo(otherCard.value);

            }
        }

        class Selection : List<Card>
        {
            public enum Type
            {
                Null,
                None,
                Illegal,
                Single,
                Double,
                ThreeWithOne,
                Chair,
                Line,
                Plane,
                Bomb
            };
            private Type _type = Type.Null;
            public Card.Value value;
            public int Length;

            public Type type {
                get
                {
                    if (_type != Type.Null)
                    {
                        return _type;
                    }
                    else
                    {
                        if(Count == 0)      //判断是否什么都没选
                        {
                            _type = Type.None;
                            return Type.None;
                        }
                        if(Count == 1)      //判断是否单牌
                        {
                            _type = Type.Single;
                            value = this[0].value;
                            return Type.Single;
                        }
                        if(Count == 2)    
                        {
                            if (this[0].value == this[1].value)//判断是否对子
                            {
                                _type = Type.Double;
                                value = this[0].value;
                                return Type.Double;
                            }
                            if (this[0].color == Card.Color.Joker && this[1].color == Card.Color.Joker)//判断对鬼
                            {
                                _type = Type.Bomb;
                                value = this[0].value;
                                return Type.Bomb;
                            }
                        }
                        if (Count == 4)
                        {
                            if ((this[0].value == this[1].value) && (this[1].value == this[2].value) && (this[2].value == this[3].value))//判断炸弹
                            {
                                _type = Type.Bomb;
                                value = this[0].value;
                                return Type.Bomb;
                            }
                            Sort();
                            if (((this[0].value == this[1].value) && (this[1].value == this[2].value)) || ((this[1].value == this[2].value) && (this[2].value == this[3].value)))//三带一
                            {
                                _type = Type.ThreeWithOne;
                                value = this[1].value;
                                return Type.ThreeWithOne;
                            }
                        }
                        if(Count >= 5)//判断顺子
                        {
                            Sort();
                            bool OK = true;
                            for(int i = 1; i < Count; i++)
                            {
                                if (this[i - 1].value != this[i].value - 1 || this[i].value > Card.Value.A) 
                                {
                                    OK = false;
                                    break;
                                }
                            }
                            if (OK)
                            {
                                _type = Type.Line;
                                Length = Count;
                                value = this[0].value;
                                return Type.Line;
                            }
                        }
                        if (Count >= 6 && Count % 2 == 0) //判断板凳
                        {
                            Sort();
                            bool OK = true;
                            for(int i = 0; i < Count; i += 2)
                            {
                                if(this[i].value > Card.Value.A)
                                {
                                    OK = false;
                                    break;
                                }
                                if (i >= 2)
                                {
                                    if (this[i].value != this[i - 1].value + 1)
                                    {
                                        OK = false;
                                        break;
                                    }
                                }
                                if (this[i].value != this[i + 1].value)
                                {
                                    OK = false;
                                    break;
                                }
                            }
                            if (OK)
                            {
                                _type = Type.Chair;
                                Length = Count;
                                value = this[0].value;
                                return Type.Chair;
                            }
                        }
                        if (Count == 8)//判断飞机
                        {
                            //Sort();(之前肯定排过了）
                            int cnt = 1;
                            //找出333444之类的结构
                            for(int i = 1; i < Count; i++)
                            {
                                if (cnt == 3)
                                {
                                    if (this[i].value == this[i - 1].value + 1)
                                    {
                                        cnt++;
                                    }
                                    else
                                    {
                                        cnt = 1;
                                    }
                                }
                                else
                                {
                                    if (this[i - 1].value == this[i].value)
                                    {
                                        cnt++;
                                    }
                                    else
                                    {
                                        cnt = 1;
                                    }
                                    if (cnt == 6)
                                    {
                                        break;
                                    }
                                }
                            }
                            if(cnt == 6)
                            {
                                _type = Type.Plane;
                                value = this[2].value;
                                return Type.Plane;
                            }
                        }
                        _type = Type.Illegal;
                        return _type;
                    }
                }
            }

            public Selection()
            {
                _type = Type.None;
            }

            public Selection(string a)
            {
                if (a.Length % 2 != 0)
                {
                    throw new FormatException();                                            
                }
                for(int i = 0; i < a.Length; i += 2)
                {
                    Add(new Card(a[i], a[i + 1]));
                }
            }

            public override string ToString()
            {
                string ans = "";
                foreach(Card i in this)
                {
                    ans += i.ToString();
                }
                return ans;
            }

            public bool CanMatch(Selection Last)
            {
                if (type == Type.None||Last.type == Type.None)
                {
                    return true;
                }
                if (type == Type.Illegal||type == Type.Null)
                {
                    return false;
                }
                if(type == Type.Bomb && Last.type != Type.Bomb)
                {
                    return true;
                }
                if (!(type == Type.Line)||(type == Type.Chair)&&type == Last.type)
                {
                    return value > Last.value;
                }
                return type == Last.type && Length == Last.Length && value > Last.value;
            }
        }

        const int CARDS_PER_PERSON = 17; 

        Selection[] cards;
        Selection baseCard;
        Random rand = new Random();

        public GameruleHandler()
        {
            Program.Log("已调用GameruleHandler构造函数");
            MainThread = new Thread(Work);
        }

        bool IsSubSet(Selection a,Selection b)
        {
            foreach (var i in a)
            {
                if (!b.Contains(i))
                    return false;
            }
            return true;
        }

        public void Work()
        {
            Program.Log("已开局");
            CreateCards();
            cards[0].Sort();
            cards[1].Sort();
            cards[2].Sort();
            SendCardList(0);
            SendCardList(1);
            SendCardList(2);
            int now = rand.Next(3);
            bool LandlordSelected = false;
            for(int i = 1; i <= 3; i++)
            {
                if (AskLandlord(now))
                {
                    LandlordSelected = true;
                    break;
                }
                else
                {
                    now++;
                    now %= 3;
                }
            }
            if (LandlordSelected)
            {
                Program.Log(String.Format("{0}号玩家是地主", now));
                cards[now].AddRange(baseCard);
                cards[now].Sort();
                SendBaseCard();
                Selection Last = new Selection();
                int Owner = now;
                while (true)
                {
                    AnnounceRound(now);
                    SendCardList(now);
                    Selection selection = GetSelection(now);
                    while (!SelectionOK(now, Last, Owner, selection))
                    {
                        TellSelectionFail(now);
                        selection = GetSelection(now);
                    }
                    if (selection.type != Selection.Type.None)
                    {
                        Last = selection;
                        Owner = now;
                    }
                    AnnounceSelection(now, selection);
                    foreach (Card i in selection)
                    {
                        cards[now].Remove(i);
                    }
                    if (cards[now].Count == 0)
                    {
                        AnnounceWinner(now);
                        break;
                    }
                    else
                    {
                        SendCardList(now);
                        now++;
                        now %= 3;
                    }
                }
            }
            else
            {
                CalculateScore();
            }
            Program.Log("本局结束");
        }

        private bool SelectionOK(int now, Selection Last, int Owner, Selection selection)
        {
            if (!IsSubSet(selection, cards[now]))
            {
                return false;
            }
            else
            {
                if(now == Owner)
                {
                    return selection.type != Selection.Type.None;
                }
                else
                {
                    return selection.CanMatch(Last);
                }
            }
        }

        private void AnnounceWinner(int now)
        {
            Program.Log(string.Format("玩家{0}赢了",now));
        }

        private void AnnounceSelection(int now, Selection selection)
        {
            Program.Log(string.Format("玩家{0}的出牌是:",now) + selection.ToString());
        }

        private void TellSelectionFail(int now)
        {
            Program.Log(string.Format("玩家{0}的出牌无效",now));
        }

        private Selection GetSelection(int now)
        {
            Program.Log(string.Format("正在等待玩家{0}出牌", now));
            Program.Log("请输入出牌");
            return new Selection(Console.ReadLine());
        }

        private void AnnounceRound(int now)
        {
            Program.Log(string.Format("现在轮到玩家{0}出牌", now));
        }

        private void SendBaseCard()
        {
            Program.Log("底牌是：" + baseCard.ToString());
        }

        private void CalculateScore()
        {
            Program.Log("应该算分，但功能未开发");
        }

        private bool AskLandlord(int now)
        {
            Program.Log(string.Format("询问玩家{0}是否成为地主",now));
            return System.Windows.Forms.MessageBox.Show("是否成为地主？", "提示", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes;
        }

        private void SendCardList(int v)
        {
            Program.Log(string.Format("向玩家{0}发送手牌列表：", v) + cards[v].ToString());
        }

        private void CreateCards()
        {
            Program.Log("开始洗牌");
            cards = new Selection[3];
            for (int i = 0; i <= 2; i++)
            {
                cards[i] = new Selection();
            }
            baseCard = new Selection();
            Selection Full = new Selection();
            for(Card.Color i = Card.Color.Hongtao; i <= Card.Color.Meihua; i++)
            {
                for(Card.Value j = Card.Value.Three; j <= Card.Value.Two; j++)
                {
                    Full.Add(new Card(i, j));
                }
            }
            Full.Add(new Card(Card.Color.Joker, Card.Value.Grey));
            Full.Add(new Card(Card.Color.Joker, Card.Value.Red));
            Reshuffle(ref Full);
            cards[0].AddRange(Full.GetRange(0, CARDS_PER_PERSON));
            cards[1].AddRange(Full.GetRange(CARDS_PER_PERSON, CARDS_PER_PERSON));
            cards[2].AddRange(Full.GetRange(CARDS_PER_PERSON * 2, CARDS_PER_PERSON));
            //Debug.Assert(cards[0] is Selection);
            baseCard.AddRange(Full.GetRange(CARDS_PER_PERSON * 3, 3));
            Program.Log("洗牌结束");
        }

        private void Reshuffle(ref Selection listtemp)
        {
            //随机交换
            Random ram = new Random();
            int currentIndex;
            Card tempValue;
            for (int i = 0; i < listtemp.Count; i++)
            {
                currentIndex = ram.Next(0, listtemp.Count - i);
                tempValue = listtemp[currentIndex];
                listtemp[currentIndex] = listtemp[listtemp.Count - 1 - i];
                listtemp[listtemp.Count - 1 - i] = tempValue;
            }
        }

        Thread MainThread;
        public void StartGame()
        {
            MainThread.Start();
            Program.Log("已启动主线程");
        }

        public void AbortGame()
        {
            MainThread.Abort();
            Program.Log("已强制终止主线程");
        }

#if DEBUG
        public void DoTest()
        {
            //Program.Log("当前测试项目：Card结构");
            //Card a = new Card('B', 'c');
            //Program.Log(string.Format("花色：{0}，大小：{1}", a.color, a.value));
            //Card b = new Card('C', 'e');
            //Program.Log(Convert.ToString(a < b));
            //Card c = new Card('F', 'z');
            //Program.Log(string.Format("花色：{0}，大小：{1}", c.color, c.value));

            //Program.Log("测试洗牌程序");
            //CreateCards();
            //Program.Log(cards[0].ToString());
            //Program.Log(cards[1].ToString());
            //Program.Log(cards[2].ToString());
            //Program.Log(baseCard.ToString());

            //Program.Log("测试牌型判断");
            //Selection a = new Selection(Console.ReadLine());
            //Program.Log(a.type.ToString());

            //Program.Log("测试大小比较");
            //while (true)
            //{
            //    Selection a = new Selection(Console.ReadLine());
            //    Selection b = new Selection(Console.ReadLine());
            //    Program.Log(b.CanMatch(a).ToString());
            //}

            Program.Log("测试游戏逻辑");
            StartGame();
        }
#endif
    }
}
