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
            Type type;
        }

        public GameruleHandler()
        {
            Program.Log("已调用GameruleHandler构造函数");
        }

        void Work()
        {

        }
    }
}
