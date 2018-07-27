using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NetClient
{
    public class NetworkClient
    {
        Socket clientSocket = null;
        static Boolean isListen = true;
        Thread thDataFromServer;
        IPAddress ipadr;

        public delegate void MessageRecievedHandler(string msg);
        public delegate void FailureHandler(string msg);
        public event MessageRecievedHandler MessageRecieved;
        public event FailureHandler FailureCaused;

        public void SendMessage(string msg)
        {
            if (String.IsNullOrWhiteSpace(msg.Trim()))
            {
                FailureCaused("发送内容不能为空哦~");
                return;
            }
            if (clientSocket != null && clientSocket.Connected)
            {
                Byte[] bytesSend = Encoding.UTF8.GetBytes(msg + "$");
                clientSocket.Send(bytesSend);
            }
            else
            {
                FailureCaused("未连接服务器或者服务器已停止，请联系管理员~");
                return;
            }
        }


        //每一个连接的客户端必须设置一个唯一的用户名，在服务器端是把用户名和套接字保存在Dictionary<userName,ClientSocket>中
        public bool Connect(string UserName, string ip)
        {
            if (String.IsNullOrWhiteSpace(UserName.Trim()))
            {
                FailureCaused("请设置个用户名哦亲");
                return false;
            }
            if (UserName.Length >= 17 && UserName.ToString().Trim().Substring(0, 17).Equals("Server has closed"))
            {
                FailureCaused("该用户名中包含敏感词，请更换用户名后重试");
                return false;
            }

            if (clientSocket == null || !clientSocket.Connected)
            {
                try
                {
                    clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    //参考网址： https://msdn.microsoft.com/zh-cn/library/6aeby4wt.aspx
                    // Socket.BeginConnect 方法 (String, Int32, AsyncCallback, Object)
                    //开始一个对远程主机连接的异步请求
                    /* string host,     远程主机名
                     * int port,        远程主机的端口
                     * AsyncCallback requestCallback,   一个 AsyncCallback 委托，它引用连接操作完成时要调用的方法，也是一个异步的操作
                     * object state     一个用户定义对象，其中包含连接操作的相关信息。 当操作完成时，此对象会被传递给 requestCallback 委托
                     */
                    //如果txtIP里面有值，就选择填入的IP作为服务器IP，不填的话就默认是本机的
                    if (!String.IsNullOrWhiteSpace(ip.ToString().Trim()))
                    {
                        try
                        {
                            ipadr = IPAddress.Parse(ip.ToString().Trim());
                        }
                        catch
                        {
                            FailureCaused("请输入正确的IP后重试");
                            return false;
                        }
                    }
                    else
                    {
                        ipadr = IPAddress.Loopback;
                    }
                    //IPAddress ipadr = IPAddress.Parse("192.168.1.100");
                    clientSocket.BeginConnect(ipadr, 8080, (args) =>
                    {
                        if (args.IsCompleted)   //判断该异步操作是否执行完毕
                        {
                            Byte[] bytesSend = new Byte[4096];
                            bytesSend = Encoding.UTF8.GetBytes(UserName.Trim() + "$");  //用户名，这里是刚刚连接上时需要传过去
                            if (clientSocket != null && clientSocket.Connected)
                            {
                                clientSocket.Send(bytesSend);
                                thDataFromServer = new Thread(DataFromServer);
                                thDataFromServer.IsBackground = true;
                                thDataFromServer.Start();
                            }
                            else
                            {
                                FailureCaused("服务器已关闭");
                            }
                        }
                    }, null);
                    return true;
                }
                catch (SocketException ex)
                {
                    FailureCaused(ex.ToString());
                    return false;
                }
            }
            else
            {
                FailureCaused("你已经连接上服务器了");
                return true;
            }
        }

        //获取服务器端的消息
        private void DataFromServer()
        {
            MessageRecieved("SLOG|已连接到服务器");
            isListen = true;
            try
            {
                while (isListen)
                {
                    Byte[] bytesFrom = new Byte[4096];
                    Int32 len = clientSocket.Receive(bytesFrom);

                    String dataFromClient = Encoding.UTF8.GetString(bytesFrom, 0, len);
                    if (!String.IsNullOrWhiteSpace(dataFromClient))
                    {
                        //如果收到服务器已经关闭的消息，那么就把客户端接口关了，免得出错，并在客户端界面上显示出来
                        if (dataFromClient.ToString().Length >= 17 && dataFromClient.ToString().Substring(0, 17).Equals("Server has closed"))
                        {
                            clientSocket.Close();
                            clientSocket = null;

                            MessageRecieved( "SLOG|服务器已关闭");
                            MessageRecieved( "SMSG|服务器已关闭");
                            thDataFromServer.Abort();   //这一句必须放在最后，不然这个进程都关了后面的就不会执行了

                            return;
                        }


                        if (dataFromClient.StartsWith("#") && dataFromClient.EndsWith("#"))
                        {
                            String userName = dataFromClient.Substring(1, dataFromClient.Length - 2);
                                FailureCaused("用户名：[" + userName + "]已经存在，请尝试其他用户名并重试");
                            isListen = false;
                                clientSocket.Send(Encoding.UTF8.GetBytes("$"));
                                clientSocket.Close();
                                clientSocket = null;
                        }
                        else
                        {
                            //txtName.Enabled = false;    //当用户名唯一时才禁止再次输入用户名
                            MessageRecieved(dataFromClient);
                        }
                    }
                }
            }
            catch (SocketException ex)
            {
                isListen = false;
                if (clientSocket != null && clientSocket.Connected)
                {
                    //没有在客户端关闭连接，而是给服务器发送一个消息，在服务器端关闭连接
                    //这样可以将异常的处理放到服务器。客户端关闭会让客户端和服务器都抛异常
                    clientSocket.Send(Encoding.UTF8.GetBytes("$"));
                    FailureCaused(ex.ToString());
                }
            }
        }

        public void Stop()
        {
            if (clientSocket != null && clientSocket.Connected)
            {
                thDataFromServer.Abort();
                clientSocket.Send(Encoding.UTF8.GetBytes("$"));

                clientSocket.Close();
                clientSocket = null;
                    MessageRecieved( "已断开与服务器的连接");
            }
        }
    }
}
