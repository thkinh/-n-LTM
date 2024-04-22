using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using System.Threading;
using System;
using UnityEngine.SceneManagement;
using Assets.Scripts.GamePlay;
using Unity.VisualScripting;

namespace Assets.Scripts
{
    public class Client
    {
        TcpClient tcpClient;
        NetworkStream stream;
        string address = "127.0.0.1";
        int port = 9999;
        public int id = 90;

        public Client()
        {

        }

        public async void ConnectToServer()
        {
            try
            {
                tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(IPAddress.Parse(address), port);

                if (tcpClient.Connected)
                {
                    stream = tcpClient.GetStream();
                    SendPacket("Hello server");
                    byte[] first_data = new byte[1024];
                    await stream.ReadAsync(first_data,0,first_data.Length);
                    SceneManager.LoadSceneAsync("GamePlay");
                    WelcomeReceived(first_data, first_data.Length);
                    await Task.Run(() => { ListenToServer(); });
                }

            }
            catch(Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        private void WelcomeReceived(byte[] data, int length)
        {
            try
            {
                Packet packet = new Packet(data);
                int len = packet.ReadInt();
                this.id = packet.ReadInt();
                string msg = packet.ReadString();
                Debug.Log($"This client's ID has been set: {id}/ {msg}");
                return;
            }
            catch
            {
                Debug.Log("Loi khi nhan goi tin welcome");
                return;
                //Debug.Log(ex.Message.ToString());
            }
        }

        private async Task ListenToServer() 
        {
            while (tcpClient.Connected)
            {
                byte[] data = new byte[1024];
                try
                {
                    await stream.ReadAsync(data, 0, data.Length);
                    Handle_Incoming_Packet(data, data.Length);
                }
                catch 
                {
                    Debug.Log("Loi khi nhan packet");
                    //Debug.Log(ex.Message.ToString());
                }
            }

        }

        public void Handle_Incoming_Packet(byte[] data, int data_length)
        {
            try
            {
                Debug.Log("Received a packet from server");
                Packet packet = new Packet(data);
                int lenght = packet.ReadInt();
                int foodname = packet.ReadInt();
                Food food = new Food(foodname);
                EntityManager.instance.Spawn_Food(food);
                
            }
            catch
            {
                Debug.Log("Loi xu ly goi tin, khong the nhan dang food");
            }
        }

        public void SendPacket(string message)
        {
            try
            {
                using (Packet packet = new Packet())
                {
                    packet.Write(ClientManager.client.id);
                    packet.Write(message);
                    packet.WriteLength();
                    stream.WriteAsync(packet.ToArray(), 0, packet.Length());
                }
            }
            catch (Exception e) 
            {
                Debug.Log(e.Message);
                //Debug.Log("Loi khi gui?");
            }
        }

        public void SendPacket(Food food, bool next_or_prev)
        {
            using (Packet packet = new Packet())
            {
                packet.Write(ClientManager.client.id);
                packet.Write(next_or_prev);
                packet.Write((int)food.name);
                packet.WriteLength();
                stream.WriteAsync(packet.ToArray(),0,packet.Length());
            }
        }
    }
}
