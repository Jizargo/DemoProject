/*
 * 功能：实现客户端逻辑
 * Coder：东方喵
 * 时间：2017/8/15
 */
using UnityEngine;
using System.Collections;

public class ClientLogic : MonoBehaviour {
    public Transform c0;//棋盘的2个角
    public Transform c1;
    public GameObject[] piece;
    public Hashtable pieceHash;//棋子
    const int sizeX = 19;//棋盘为19*19格
    const int sizeY = 19;
    private float cellSizeX;//棋盘格尺寸
    private float cellSizeY;
    private bool[,] isSet;//存储此格是否已放棋子
    private ClientHandler ch;
    int indexX;
    int indexY;
    private void Awake()
    {
        pieceHash = new Hashtable();
        foreach(GameObject o in piece)
        {
            pieceHash.Add(o.name,o);
        }
        ClientHandler.cruTurn = "black";
        cellSizeX = (c1.position.x - c0.position.x)/sizeX;
        cellSizeY = (c1.position.y - c0.position.y)/sizeY;
        isSet = new bool[sizeX, sizeY];
        for(int i = 0; i < sizeX; i++)
        {
            for(int j = 0; j < sizeY; j++)
            {
                isSet[i, j] = false;
            }
        }
        ch=GameObject.Find("Main Camera").GetComponent<ClientHandler>();
    }
    private void Update ()
    {
        if (Input.GetMouseButtonDown(0)&& ClientHandler.cruTurn==ClientHandler.userColor&&!ClientHandler.isOver)
        {
            RaycastHit2D hit= Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if(hit)
            {
                Debug.Log(hit.point);
                indexX = (int)System.Math.Round((hit.point.x - c0.transform.position.x) / cellSizeX,System.MidpointRounding.AwayFromZero);//鼠标位置对应的格子按"四舍五入"算
                indexY = (int)System.Math.Round((hit.point.y - c0.transform.position.y) / cellSizeY, System.MidpointRounding.AwayFromZero);
                if (indexX>=0&&indexX <sizeX&& indexY >= 0 && indexY < sizeY)
                {
                    if (!isSet[indexX, indexY])
                    {
                        ch.SendMessage(indexX+"+"+indexY);
                    }
                    else
                    {
                        Debug.Log("此格已经下子儿了");
                    }
                }
            }
        }
	}
    public void setPiece(int x,int y,string c)//落子
    {
        Vector3 v = c0.transform.position + new Vector3(x * cellSizeX, y * cellSizeY, 0);
        Instantiate(pieceHash[c]as GameObject, v, Quaternion.identity);
        isSet[indexX, indexY] = true;
    }
}
