using UnityEngine;
using System.Collections;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class UdpClient : MonoBehaviour
{
    string editString = "Hello Wolrd";

    Socket aimSocket;
    EndPoint serverEnd;
    IPEndPoint ipEnd;
    string recvStr;
    string sendStr;
    byte[] recvData = new byte[1024]; 
    byte[] sendData = new byte[1024];
    int recvLen;
    Thread connectThread;

    void InitSocket()
    {
        ipEnd = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8001);
        aimSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        serverEnd = (EndPoint)sender;
        print("Waiting for sending UDP dgram");

        SocketSend("Hello");

        connectThread = new Thread(new ThreadStart(SocketReceive));
        connectThread.Start();
    }

    void SocketSend(string _sendStr)
    {
        sendData = new byte[1024];
        sendData = Encoding.ASCII.GetBytes(_sendStr);

        aimSocket.SendTo(sendData, sendData.Length, SocketFlags.None, ipEnd);
    }

    void SocketReceive()
    {
        while (true)
        {
            recvData = new byte[1024];
            recvLen = aimSocket.ReceiveFrom(recvData, ref serverEnd);
            print("Message from: " + serverEnd.ToString()); 

            recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
            print(recvStr);
        }
    }

    void SocketQuit()
    {
        if (connectThread != null)
        {
            connectThread.Interrupt();
            connectThread.Abort();
        }

        if (aimSocket != null) aimSocket.Close();
    }

    // Use this for initialization
    void Start()
    {
        InitSocket();
    }

    void OnGUI()
    {
        editString = GUI.TextField(new Rect(10, 10, 100, 20), editString);
        if (GUI.Button(new Rect(10, 30, 60, 20), "send"))
            SocketSend(editString);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnApplicationQuit()
    {
        SocketQuit();
    }
}