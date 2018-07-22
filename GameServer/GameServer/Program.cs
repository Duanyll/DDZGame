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
            Console.WriteLine(DateTime.Now + " " + a);
        }

        static GameruleHandler handler;

        static void Main(string[] args)
        {
#if !DEBUG
            try
            {
#endif
                Console.WriteLine("DDZ Game Server Copyright (C) 2018  Duanyll");
                Console.WriteLine("This program comes with ABSOLUTELY NO WARRANTY; ");
                Console.WriteLine("This is free software, and you are welcome to redistribute it");
                Console.WriteLine("under certain conditions;");

                Log("服务器程序已启动");
                handler = new GameruleHandler();

#if DEBUG
                Log("受编译选项控制，当前处于测试模式");
                handler.DoTest();
#else
                handler.StartGame();
#endif

                Log("即将终止服务器程序,请按任意键退出");
                Console.ReadKey();
                return;
#if !DEBUG
            }
            catch(Exception ex)
            {
                Log("程序遭遇了不可恢复的异常，现将退出");
                Log("异常消息如下：");
                Log(ex.Message);
                Console.ReadKey();
                return;
            }
#endif
        }
    }
}
