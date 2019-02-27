using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class UdpServer : MonoBehaviour
{
    Socket aimSocket;
    EndPoint clientEnd;
    IPEndPoint ipEnd;
    string recvStr;
    string sendStr;
    byte[] recvData = new byte[1024];
    byte[] sendData = new byte[1024];
    int recvLen;
    Thread connectThread;

    void SocketQuit()
    {
        if (connectThread != null)
        {
            connectThread.Interrupt();
            connectThread.Abort();
        }

        if (aimSocket != null) aimSocket.Close();
        print("Disconnected");
    }


    void SocketSend(string _sendStr)
    {
        sendData = new byte[1024];
        sendData = Encoding.ASCII.GetBytes(_sendStr);

        aimSocket.SendTo(sendData, sendData.Length, SocketFlags.None, clientEnd);
    }

    void SocketReceive()
    {
        while (true)
        {
            recvData = new byte[1024];
            recvLen = aimSocket.ReceiveFrom(recvData, ref clientEnd);
            print("Message from: " + clientEnd.ToString());

            recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
            print(recvStr);

            sendStr = "From Server: " + recvStr;
            SocketSend(sendStr);
        }
    }

    void InitSocket()
    {
        ipEnd = new IPEndPoint(IPAddress.Any, 8001);
        aimSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        aimSocket.Bind(ipEnd);

        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        clientEnd = (EndPoint)sender;
        print("Waiting for a UDP dgram");

        connectThread = new Thread(new ThreadStart(SocketReceive));
        connectThread.Start();
    }

    // Start is called before the first frame update
    void Start()
    {
        InitSocket();    
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
