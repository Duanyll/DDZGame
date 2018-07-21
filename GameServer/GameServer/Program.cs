using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    class Program
    {
        public static void Log(string a)
        {
            Console.WriteLine(a);
        }

        static GameruleHandler handler;

        static void Main(string[] args)
        {
            Console.WriteLine("DDZ Game Server Copyright (C) 2018  Duanyll");
            Console.WriteLine("This program comes with ABSOLUTELY NO WARRANTY; ");
            Console.WriteLine("This is free software, and you are welcome to redistribute it");
            Console.WriteLine("under certain conditions;");

            Log("服务器程序已启动");
            handler = new GameruleHandler();

            Log("即将终止服务器程序,请按任意键退出");
            Console.ReadKey();
            return;
        }
    }
}
