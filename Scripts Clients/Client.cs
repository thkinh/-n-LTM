using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using UnityEditor.Experimental.GraphView;
using System;
using UnityEngine.UI;
using System.Text;

public class Client : MonoBehaviour
{
    public static Client instance;
    public static int databuffersize = 4096;
    public string ip = "127.0.0.1";
    public int port = 9999;
    public int ID;
    public TCP tcp;
   
    private delegate void PacketHandler(Packet packet);
    private static Dictionary<int, PacketHandler> packet_manager = new Dictionary<int, PacketHandler>();


    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Instance already exists");
            Destroy(instance);
            return;
        }
        instance = this;
    }



    private void Start()
    {
        tcp = new TCP();  
    }

    public void ConnectToServer()
    {
        InitializeClientData();
        tcp.Connect();
    }

    public class TCP
    {
        public TcpClient socket;
        private NetworkStream stream;
        private byte[] receivedbuffer;
        private Packet receivedData;

        public void Connect()
        {
            socket = new TcpClient();
            socket.ReceiveBufferSize = databuffersize;
            socket.SendBufferSize = databuffersize;

            receivedbuffer = new byte[databuffersize];
            socket.BeginConnect(instance.ip,instance.port,ConnectCallBack,socket);
        }

        private void ConnectCallBack(IAsyncResult result)
        {
            socket.EndConnect(result);
            if(!socket.Connected)
            {
                return;
            }

            stream = socket.GetStream();

            receivedData = new Packet();

            stream.BeginRead(receivedbuffer, 0, databuffersize, ReceiveCallBack, null);

        }

        public void SendData(Packet _packet)
        {
            try
            {
                if(socket != null)
                {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
            }
            catch  { 
                Debug.Log("Loi~ luc gui?");
            }
        }

        private void ReceiveCallBack(IAsyncResult result)
        {
            try
            {
                int bytelength = stream.EndRead(result);
                if (bytelength > 0)
                {
                    byte[] data = new byte[bytelength];
                    Array.Copy(receivedbuffer, data, bytelength);
                    //Debug.Log(data);
                    //UI_Manager.instance.username.text = Encoding.UTF8.GetString(data);
                    receivedData.Reset(HandleData(data));
                    stream.BeginRead(receivedbuffer, 0,databuffersize, ReceiveCallBack, null);   
                }
            }
            catch 
            {
                Debug.Log("Loi~ luc nha^n");
            }
        }

        private bool HandleData(byte[] _data)
        {
            int _packetLength = 0;

            receivedData.SetBytes(_data);

            if (receivedData.UnreadLength() >= 4)
            {
                _packetLength = receivedData.ReadInt();
                if (_packetLength <= 0)
                {
                    return true;
                }
            }

            while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
            {
                byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        packet_manager[_packetId](_packet);
                    }
                });

                _packetLength = 0;
                if (receivedData.UnreadLength() >= 4)
                {
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }
            }

            if (_packetLength <= 1)
            {
                return true;
            }

            return false;
        }

    }

    private void InitializeClientData()
    {
        packet_manager = new Dictionary<int, PacketHandler>()
        {
            {(int)ServerPackets.welcome, ClientHandle.Welcome}
        };
        //Debug.Log("Initializing packets..");
    }
}
