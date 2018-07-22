﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace GameServer
{
    class GameruleHandler
    {
        private struct Card
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
            public static bool operator ==(Card A,Card B)
            {
                return A.value == B.value;
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
        }

        class Selection : List<Card>
        {
            public enum Type
            {
                None,
                Single,
                Double,
                ThreeWithOne,
                Chair,
                Line,
                Plane,
                Bomb
            };
            public Type type;

            public Selection()
            {
                type = Type.None;
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
                throw new NotImplementedException();
            }
        }

        Selection[] cards;
        Selection baseCard;
        Random rand = new Random();

        public GameruleHandler()
        {
            Program.Log("已调用GameruleHandler构造函数");
            MainThread = new Thread(Work);
        }

        public void Work()
        {
            Program.Log("已开局");
            cards = new Selection[3];
            CreateCards();
            SendCardList(0);
            SendCardList(1);
            SendCardList(2);
            int now = rand.Next(2);
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
                SendBaseCard();
                Selection Last = new Selection();
                int Owner = now;
                while (true)
                {
                    AnnounceRound(now);
                    SendCardList(now);
                    Selection selection = GetSelection(now);
                    AnnounceSelection(now, selection);
                    while ((!((now != Owner) && selection.type == Selection.Type.None)) && !selection.CanMatch(Last))
                    {
                        TellSelectionFail(now);
                        selection = GetSelection(now);
                    }
                    if (selection.type != Selection.Type.None)
                    {
                        Last = selection;
                    }
                    AnnounceSelection(now, selection);
                    for(int i = 0; i < cards[now].Count; i++)
                    {
                        if (selection.Contains(cards[now][i]))
                        {
                            cards[i].RemoveAt(i);
                            i--;
                        }
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
        }

        private void AnnounceWinner(int now)
        {
            throw new NotImplementedException();
        }

        private void AnnounceSelection(int now, Selection selection)
        {
            throw new NotImplementedException();
        }

        private void TellSelectionFail(int now)
        {
            throw new NotImplementedException();
        }

        private Selection GetSelection(int now)
        {
            throw new NotImplementedException();
        }

        private void AnnounceRound(int now)
        {
            throw new NotImplementedException();
        }

        private void SendBaseCard()
        {
            throw new NotImplementedException();
        }

        private void CalculateScore()
        {
            throw new NotImplementedException();
        }

        private bool AskLandlord(int now)
        {
            throw new NotImplementedException();
        }

        private void SendCardList(int v)
        {
            throw new NotImplementedException();
        }

        private void CreateCards()
        {
            Selection Full = new Selection();
            for(int i = 0; i < Card.COLOR_CNT - 1; i++)
            {
                for(int j = 1;j<Card.VALUE_CNT - 2; j++)
                {

                }
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
        }

#endif
    }
}
