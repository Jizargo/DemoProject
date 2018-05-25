/*
 * 功能：实现客户端socket交互
 * Coder：东方喵
 * 时间：2017/8/15
 */
 using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Net.Sockets;
using UnityEngine.UI;
public class ClientHandler : MonoBehaviour
{
    const int portNo = 500;//端口号
    private TcpClient _client;
    byte[] data;
    public Text nameText;//用户名输入框
    public GameObject btn;//登陆按钮
    public Text hintText;//提示
    public GameObject uiRoot;//UI的根物体
    public GameObject BG;//棋盘
    public static string cruTurn;//当前是谁的回合
    public static string userColor;//用户的颜色
    private ClientLogic cl;
    public static bool isOver=false;//是否游戏结束
    public Text resultText;
    public void Awake()
    {
        cl = BG.GetComponent<ClientLogic>();
    }
    public void login()
    {
        if(nameText.text!=""&&!nameText.text.Contains("+"))
        {
            this._client = new TcpClient();
            this._client.Connect("127.0.0.1", portNo);
            data = new byte[this._client.ReceiveBufferSize];
            SendMessage(nameText.text);
            this._client.GetStream().BeginRead(data, 0, System.Convert.ToInt32(this._client.ReceiveBufferSize), ReceiveMessage, null);
        }
        else
        {
        }
    }
    public void SendMessage(string message)//发送消息
    {
        try
        {
            NetworkStream ns = this._client.GetStream();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
            ns.Write(data, 0, data.Length);
            ns.Flush();
        }
        catch (Exception ex)
        {
        }
    }
    public void ReceiveMessage(IAsyncResult ar)//接收消息
    {
        try
        {
            int bytesRead;
            bytesRead = this._client.GetStream().EndRead(ar);
            if (bytesRead < 1)
            {
                return;
            }
            else
            {
                Debug.Log(System.Text.Encoding.UTF8.GetString(data, 0, bytesRead));
                string message = System.Text.Encoding.UTF8.GetString(data, 0, bytesRead);
                switch (message)
                {
                    case "登陆成功":
                        {
                            btn.SetActive(false);
                            nameText.transform.parent.gameObject.SetActive(false);
                            hintText.text = "等待其他玩家加入。";
                            break;
                        }
                    case "房间已满，登录失败":
                        {
                            btn.SetActive(false);
                            nameText.transform.parent.gameObject.SetActive(false);
                            hintText.text = "房间已满，登录失败。";
                            break;
                        }
                    case "黑棋":
                        {
                            uiRoot.SetActive(false);
                            BG.SetActive(true);
                            userColor = "black";
                            break;
                        }
                    case "白棋":
                        {
                            uiRoot.SetActive(false);
                            BG.SetActive(true);
                            userColor = "white";
                            break;
                        }
                    case "black":
                        {
                            cruTurn = "black";
                            break;
                        }
                    case "white":
                        {
                            cruTurn = "white";
                            break;
                        }
                    case "black胜利":
                        {
                            isOver = true;
                            resultText.gameObject.SetActive(true);
                            if (userColor == "black")
                            {
                                resultText.text = "大获全胜！";
                            }
                            else
                            {
                                resultText.text = "不幸惨败！";
                            }
                            break;
                        }
                    case "white胜利":
                        {
                            isOver = true;
                            resultText.gameObject.SetActive(true);
                            if (userColor == "white")
                            {
                                resultText.text = "大获全胜！";
                            }
                            else
                            {
                                resultText.text = "不幸惨败！";
                            }
                            break;
                        }
                    default:
                        {
                            if (message.Contains("+"))
                            {
                                string[] str = message.Split('+');
                                int x = int.Parse(str[0]);
                                int y = int.Parse(str[1]);
                                cl.setPiece(x,y, cruTurn);
                            }
                            break;
                        }
                }
            }
            this._client.GetStream().BeginRead(data, 0, System.Convert.ToInt32(this._client.ReceiveBufferSize), ReceiveMessage, null);
        }
        catch (Exception ex)
        {
        }
    }
}