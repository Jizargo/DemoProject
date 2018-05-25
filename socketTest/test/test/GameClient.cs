/*
 * 功能：实现服务器端逻辑判断
 * Coder：东方喵
 * 时间：2017/8/15
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Net.Sockets;
namespace SoketTest
{
    class GameClient
    {
        public static Hashtable ALLClients = new Hashtable(); // 客户端列表 
        public static List<string> ipList=new List<string>();//客户端IP列表
        private TcpClient _client; // 客户端实体
        public string _clientIP; // 客户端IP,作为主键值
        private string _clientNick; // 客户端昵称
        private string _color;//黑棋还是白棋
        private byte[] data; // 消息数据
        private string[,] chess=new string[sizeX,sizeY];//记录棋盘上的布子
        private const int sizeX = 19;
        private const int sizeY = 19;
        public GameClient(TcpClient client)
        {
            this._client = client;
            this._clientIP = client.Client.RemoteEndPoint.ToString();//获取远程终结点
            if (ALLClients.Count<=2)//俩人下棋
            {
                ALLClients.Add(this._clientIP, this);// 把当前客户端实例添加到客户列表当中
                ipList.Add(this._clientIP);// 把当前客户端ip添加到ip列表当中
                data = new byte[this._client.ReceiveBufferSize];//包含以字节为单位的接收缓冲区的大小，默认值为 8192。
                client.GetStream().BeginRead(data, 0, System.Convert.ToInt32(this._client.ReceiveBufferSize), ReceiveMessage, null);// 从服务端获取消息
                if (ALLClients.Count ==1)
                {
                    this.sendMessage("登陆成功");
                }
                else
                {
                    ((GameClient)(ALLClients[ipList[0]])).sendMessage("黑棋");
                    ((GameClient)(ALLClients[ipList[0]]))._color = "black";
                    ((GameClient)(ALLClients[ipList[1]])).sendMessage("白棋");
                    ((GameClient)(ALLClients[ipList[1]]))._color = "white";
                }
            }
            else
            {
                
                this.sendMessage("房间已满，登录失败");
            }
        }

        public void ReceiveMessage(IAsyncResult ar)// 从客戶端获取消息
        {
            int bytesRead;
            try
            {
                lock (this._client.GetStream())//对用于接收和发送数据的NetworkStream加锁
                {
                    bytesRead = this._client.GetStream().EndRead(ar);
                }
                if (bytesRead < 1)
                {
                    ALLClients.Remove(this._clientIP);
                    Broadcast("服务器异常！");
                    return;
                }
                else
                {
                    string messageReceived = System.Text.Encoding.UTF8.GetString(data, 0, bytesRead);
                    Console.WriteLine("服务器接收到:" + messageReceived);
                        if (!messageReceived.Contains("+"))
                        {
                            this._clientNick = messageReceived;
                            Console.WriteLine(this._clientIP+this._clientNick);
                        }
                        else
                        {
                            string[] strVect = new string[2];
                            strVect = messageReceived.Split('+');
                            int indexX = int.Parse(strVect[0]);
                            int indexY = int.Parse(strVect[1]);
                            chess[indexX, indexY] = this._color;
                            Broadcast(messageReceived);
                            if (checkChess(indexX, indexY, this._color))
                            {
                                Broadcast(this._color + "胜利");
                            }
                            else
                            {
                                string str;
                                Console.WriteLine(this._clientNick +"+"+this._color );
                                if (this._color == "black")
                                {
                                    str = "white";
                                }
                                else
                                {
                                    str = "black";
                                }
                                Console.WriteLine(str);
                                Broadcast(str);
                            }
                        }
                        lock (this._client.GetStream())
                        {
                            this._client.GetStream().BeginRead(data, 0, System.Convert.ToInt32(this._client.ReceiveBufferSize), ReceiveMessage, null);
                        }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("异常：" + ex);
                ALLClients.Remove(this._clientIP);
                Broadcast(this._clientNick + " 离开了游戏。");
            }
        }

        private bool checkChess(int x,int y,string c)//遍历检查棋盘是否有胜负出现
        {
            int counta = 0;
            int countb = 0;
            int countc = 0;
            int countd = 0;
            for (int i=1;i<5;i++)//横，正方向
            {
                if (x + i >=sizeX)
                {
                    break;
                }
                if (chess[i+x, y] == c)
                {
                    counta++;
                }
                else
                {
                    break;
                }
            }
            for (int i = 1; i < 5; i++)//横，负方向
            {
                if (x -i <0)
                {
                    break;
                }
                if (chess[x-i,y] == c)
                {
                    counta++;
                }
                else
                {
                    break;
                }
            }
            Console.WriteLine("counta="+counta);
            if (counta>=4)
            {
                return true;
            }

            for (int i = 1; i < 5; i++)//竖，正方向
            {
                if (y + i >=sizeY)
                {
                    break;
                }
                if (chess[x, i+y] == c)
                {
                    countb++;
                }
                else
                {
                    break;
                }
            }
            for (int i = 1; i < 5; i++)//竖，负方向
            {
                if (y-i <0)
                {
                    break;
                }
                if (chess[x,y-i] == c)
                {
                    countb++;
                }
                else
                {
                    break;
                }
            }
            Console.WriteLine("countb=" + countb);
            if (countb >= 4)
            {
                return true;
            }

            for (int i = 1; i < 5; i++)//正斜，正方向
            {
                if (x + i >=sizeX||y+i>=sizeY)
                {
                    break;
                }
                if (chess[x+i, y+i] == c)
                {
                    countc++;
                }
                else
                {
                    break;
                }
            }
            for (int i = 1; i < 5; i++)//正斜，负方向
            {
                if (x -i<0 || y -i<0)
                {
                    break;
                }
                if (chess[x-i, y-i] == c)
                {
                    countc++;
                }
                else
                {
                    break;
                }
            }
            Console.WriteLine("countc=" + countc);
            if (countc >= 4)
            {
                return true;
            }

            for (int i = 1; i < 5; i++)//负斜，正方向
            {
                if (x + i >=sizeX || y - i < 0)
                {
                    break;
                }
                if (chess[x + i, y - i] == c)
                {
                    countd++;
                }
                else
                {
                    break;
                }
            }
            for (int i = 1; i < 5; i++)//负斜，负方向
            {
                if (x - i < 0 || y + i >=sizeY)
                {
                    break;
                }
                if (chess[x - i, y + i] == c)
                {
                    countd++;
                }
                else
                {
                    break;
                }
            }
            Console.WriteLine("countd=" + countd);
            if (countd >= 4)
            {
                return true;
            }

            return false;
        }

        // 向客戶端发送消息
        public void sendMessage(string message)
        {
            Console.WriteLine("服务器对"+this._clientNick+"发送了："+message);
            try
            {
                System.Net.Sockets.NetworkStream ns;
                lock (this._client.GetStream())
                {
                    ns = this._client.GetStream();
                }
                // 对信息进行编码
                byte[] bytesToSend = System.Text.Encoding.UTF8.GetBytes(message);
                ns.Write(bytesToSend, 0, bytesToSend.Length);
                ns.Flush();
            }
            catch (Exception ex)
            {
            }
        }
        // 向客户端广播消息
        public void Broadcast(string message)
        {
            foreach (DictionaryEntry c in ALLClients)
            {
                ((GameClient)(c.Value)).sendMessage(message);
            }
        }
    }
}