using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    class GameruleHandler
    {
        private struct Card
        {
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
                value = (Value)(val - 'a');
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
                throw new NotImplementedException();
            }

            public bool CanMatch(Selection Last)
            {
                throw new NotImplementedException();
            }
        }

        List<Card>[] cards;
        List<Card> baseCard;
        Random rand = new Random();

        public GameruleHandler()
        {
            Program.Log("已调用GameruleHandler构造函数");
        }

        public void Work()
        {
            Program.Log("已开局");
            cards = new List<Card>[3];
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
            throw new NotImplementedException();
        }
    }
}
