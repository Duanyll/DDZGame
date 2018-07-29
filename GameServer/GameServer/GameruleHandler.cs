using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using NetServer;

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
        NetServer.NetServer server = new NetServer.NetServer();

        public GameruleHandler()
        {
            Program.Log("已调用GameruleHandler构造函数");
            MainThread = new Thread(Work);
            server.UserLoggedIn += NewUser;
            server.UserLoggedOut += UserLogOut;
            server.MessageRecieved += RecvMsg;
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

        bool Running = false;
        public void Work()
        {
            Program.Log("已开局");
            Running = true;
            server.BroadCastToAll("SGRD|");
            CreateCards();
            AnnounceNewGame();
            cards[0].Sort();
            cards[1].Sort();
            cards[2].Sort();
            SendCardList(0);
            SendCardList(1);
            SendCardList(2);
            server.BroadCastToAll("SMSG|即将开局...");
            Thread.Sleep(5000);
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
                int Landlord = now;
                Program.Log(String.Format("{0}号玩家是地主", now));
                server.BroadCastToAll("PLDL|" + now);
                cards[now].AddRange(baseCard);
                cards[now].Sort();
                SendBaseCard();
                Selection Last = new Selection();
                int Owner = now;
                int time = 1;
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
                    if(selection.type == Selection.Type.Bomb)
                    {
                        time *= 2;
                    }
                    AnnounceSelection(now, selection);
                    foreach (Card i in selection)
                    {
                        cards[now].Remove(i);
                    }
                    if (cards[now].Count == 0)
                    {
                        AnnounceWinner(now,Landlord,time);
                        break;
                        //server.StopService();
                        //return;
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
                UserNames.Clear();
            }
            Program.Log("本局结束");
            Running = false;
            UserNames.Clear();
            server.BroadCastToAll("GRDY|");
            //server.StopService();
        }

        private void AnnounceNewGame()
        {
            server.BroadCastToAll("PNAM|" + UserNames[0] + '|' + UserNames[1] + '|' + UserNames[2]);
            server.BroadCastToAll("PSCR|" + Score[UserNames[0]] + '|' + Score[UserNames[1]] + '|' + Score[UserNames[2]]);
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

        private void AnnounceWinner(int now,int landlord,int time)
        {
            Program.Log(string.Format("玩家{0}赢了",now));
            server.BroadCastToAll("SLOG|" + UserNames[now] + "赢了");
            server.BroadCastToAll("SMSG|" + UserNames[now] + "赢了");
            if(now == landlord)
            {
                Score[UserNames[0]] -= 1*time;
                Score[UserNames[1]] -= 1*time;
                Score[UserNames[2]] -= 1*time;
                Score[UserNames[now]] += 3*time;
            }
            else
            {
                Score[UserNames[0]] += 1*time;
                Score[UserNames[1]] += 1*time;
                Score[UserNames[2]] += 1*time;
                Score[UserNames[landlord]] -= 3*time;
            }
            server.BroadCastToAll("PSCR|" + Score[UserNames[0]] + '|' + Score[UserNames[1]] + '|' + Score[UserNames[2]]);
        }

        private void AnnounceSelection(int now, Selection selection)
        {
            Program.Log(string.Format("玩家{0}的出牌是:",now) + selection.ToString());
            server.BroadCastToAll("CLST|" + UserNames[now] + ':' + selection.ToString());
        }

        private void TellSelectionFail(int now)
        {
            Program.Log(string.Format("玩家{0}的出牌无效",now));
            server.SendTo(UserNames[now],"SMSG|出牌无效");
        }

        private Selection GetSelection(int now)
        {
            Program.Log(string.Format("正在等待玩家{0}出牌", now));
            //Program.Log("请输入出牌");
            //return new Selection(Console.ReadLine());
            LastSelect = null;
            server.SendTo(UserNames[now], "GCRD|");
            server.SendTo(UserNames[now], "SMSG|现在该你出牌");
            int i = 0;
            while (i++ < MAX_TIME_OUT)
            {
                if (LastSelect != null)
                {
                    return new Selection(LastSelect);
                }
                Thread.Sleep(0);
                Thread.Sleep(100);
            }
            server.SendTo(UserNames[now], "SGCR|");
            return new Selection("");
        }

        private void AnnounceRound(int now)
        {
            Program.Log(string.Format("现在轮到玩家{0}出牌", now));
            server.BroadCastToAll("NPLR|" + now);
        }

        private void SendBaseCard()
        {
            Program.Log("底牌是：" + baseCard.ToString());
            server.BroadCastToAll("CBAS|" + baseCard.ToString());
        }

        private void CalculateScore()
        {
            //Program.Log("应该算分，但功能未开发");
            //server.BroadCastToAll("SLOG|算分功能未开发");
            //server.BroadCastToAll("SMSG|算分功能未开发");
        }

        const int MAX_TIME_OUT = 600;
        private bool AskLandlord(int now)
        {
            Program.Log(string.Format("询问玩家{0}是否成为地主",now));
            //return System.Windows.Forms.MessageBox.Show("是否成为地主？", "提示", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes;
            LastLD = null;
            server.SendTo(UserNames[now], "QLDL|");
            int i = 0;
            while (i++ < MAX_TIME_OUT)
            {
                if (LastLD != null)
                {
                    return (bool)LastLD;
                }
                Thread.Sleep(0);
                Thread.Sleep(100);
            }
            return false;
        }

        private void SendCardList(int v)
        {
            Program.Log(string.Format("向玩家{0}发送手牌列表：", v) + cards[v].ToString());
            server.SendTo(UserNames[v], "COWN|" + cards[v].ToString());
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
            MainThread.Abort();
            MainThread = new Thread(Work);
            server.StartService();
            Program.Log("已启动主线程");
        }

        public void AbortGame()
        {
            MainThread.Abort();
            UserNames.Clear();
            server.BroadCastToAll("GRDY|");
            Running = false;
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

            //Program.Log("测试游戏逻辑");
            //StartGame();

            StartGame();
        }
#endif

        List<string> UserNames = new List<string>();
        List<string> AllUsers = new List<string>();
        Dictionary<string, int> Score = new Dictionary<string, int>();
        private void NewUser(string name)
        {
            AllUsers.Add(name);
            if (!Score.ContainsKey(name))
            {
                Score.Add(name, 0);
            }
            if (!Running)
            {
                server.SendTo(name, "GRDY|");
            }
            server.BroadCastToAll("SLOG|" + name + "进入了房间");
            server.BroadCastToAll("SMSG|" + name + "进入了房间");
        }

        private void UserLogOut(string name)
        {
            if (UserNames.Contains(name))
            {
                AbortGame();
                server.BroadCastToAll("SMSG|由于" + name + "退出了游戏，本局终止");
            }
            server.BroadCastToAll("SLOG|" + name + "已退出房间");
            server.BroadCastToAll("SMSG|" + name + "已退出房间");
        }

        string LastSelect = null;
        bool? LastLD = null;
        private void RecvMsg(string clno,string msg)
        {
            string[] vs = msg.Split('|');
            switch (vs[0])
            {
                case "CHAT":
                    server.BroadCastToAll("CHAT|" + clno + ":" + vs[1]);
                    break;
                case "SLCT":
                    LastSelect = vs[1];
                    break;
                case "LDOK":
                    LastLD = true;
                    break;
                case "LDNO":
                    LastLD = false;
                    break;
                case "REDY":
                    UserNames.Add(clno);
                    server.BroadCastToAll("SLOG|" + clno + "准备好开局了");
                    if (UserNames.Count >= 3)
                    {
                        MainThread.Abort();
                        MainThread = new Thread(Work);
                        MainThread.Start();
                    }
                    break;
            }
        }
        public void StopAll()
        {
            server.StopService();
            if (MainThread.IsAlive)
            {
                MainThread.Abort();
            }
        }
    }
}
