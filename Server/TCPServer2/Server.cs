using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.IO;

namespace TCPServer2
{
    internal class Server
    {
        private static int Max_Players = 6;
        private static int PORT = 9999;
        private static TcpListener listener;
        private List<TcpClient> clients_list = new List<TcpClient>();
        private Dictionary<int, TcpClient> clients_Dict = new Dictionary<int, TcpClient>();
        private int attending = 0;
        private int[] lobby_id = { 5, 6, 7, 8, 9 };

        public Server()
        {
            //IPAddress[] localIP = Dns.GetHostAddresses(Dns.GetHostName());
            
        }
        public void Start()
        {
            listener = new TcpListener(IPAddress.Any, PORT);
            listener.Start();
            Console.WriteLine($"Server started on {PORT}");

            Thread listen = new Thread(Listen);
            listen.Start();
        }


        private async Task HandleClient(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    int client_id = clients_Dict.FirstOrDefault(x => x.Value == client).Key;
                    string message = HandlePacket(buffer, bytesRead, client);
                    //Console.WriteLine($"Message from player {client_id.ToString()}: ");
                    // Broadcast lại cho may thằng client khác
                    //foreach (TcpClient otherClient in clients_list)
                    //{
                    //    if (otherClient != client)
                    //    {
                    //        await otherClient.GetStream().WriteAsync(buffer, 0, bytesRead);
                    //    }
                    //}
                    Console.WriteLine(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client: {ex.Message}");
            }
            finally
            {
                clients_list.Remove(client);
                clients_Dict.Remove(clients_Dict.FirstOrDefault(x => x.Value == client).Key);
                client.Close();
            }
        }

        private string HandlePacket(byte[] _packet, int length, TcpClient this_client)
        {
            Packet packet = new Packet(_packet);
            int len = packet.ReadInt();
            

            if(len == 9) //packet of food delivery
            {
                int player_id = packet.ReadInt();
                bool next_or_prev = packet.ReadBool();
                int foodname = packet.ReadInt();
                SendPacket(foodname, next_or_prev, this_client);
                packet.Dispose();
                return $"{player_id} : {Food.getName(foodname)} / {next_or_prev} / data_len = {len}";
            }
            else if (len == 20) //packet of lobby creation
            {
                for (int i = 0; i < 5; i++)
                {
                    lobby_id[i] = packet.ReadInt();
                    Console.WriteLine($"lobby id {i} is : {lobby_id[i]}");
                }
                //Send packet lobby acept
                SendPacket(true, this_client);
                return $"Lobby created, getting this player into lobby";
            }
            else if (len == 21) // packet of lobby authentication
            {

            }



            return $"Packet's length is unknowed {len}, this type of packet doesn't exists"; 
        }

        public async void Listen()
        {
            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();

                //Ket noi xong, gui? goi tin hello den client
                if (client.Connected)
                {
                    Console.WriteLine("Gui packet welcome toi Client");
                    WelcomePacket(client);
                    HelloReceived(client);
                }

                clients_list.Add(client);
                clients_Dict.Add(attending++,client);
                await Task.Run(() => { HandleClient(client); });
            }
        }

        public async void WelcomePacket(TcpClient _client)
        {
            Packet hello_packet = new Packet();
            hello_packet.Write(attending);
            hello_packet.Write("Welcome to server");
            hello_packet.WriteLength();
            await _client.GetStream().WriteAsync(hello_packet.ToArray(),0,hello_packet.Length());
            hello_packet.Dispose();
            return;
        }

        public async void HelloReceived(TcpClient _client)
        {
            byte[] data = new byte[1024];
            await _client.GetStream().ReadAsync(data,0,data.Length);
            Packet packet = new Packet(data);
            int len = packet.ReadInt();
            int id = packet.ReadInt();
            string msg = packet.ReadString();
            Console.WriteLine($"Received hello packet from a client: {id}/ {msg}");
            Console.WriteLine($"Setting this client's id = {attending}\n");
            packet.Dispose();
            return;
        }



        public void SendPacket(bool acept_into_lobby, TcpClient client)
        {
            Packet packet = new Packet();
            packet.Write(acept_into_lobby);
            packet.WriteLength();
            client.GetStream().WriteAsync(packet.ToArray(),0,packet.Length());
            packet.Dispose();
        }


        //<sumary> Transfering food between players
        public void SendPacket(int foodname, bool next, TcpClient this_client)
        {
            int client_key_to_send = clients_Dict.FirstOrDefault(x => x.Value == this_client).Key;

            if (clients_Dict.Count == 1)
            {
                Console.WriteLine("Chi co 1 player trong phong choi");
                return;
            }

            if (next)
            {
                Packet packet = new Packet();
                packet.Write(foodname);
                packet.WriteLength();
                client_key_to_send += 1;
                if (client_key_to_send >= clients_Dict.Count)
                {
                    client_key_to_send = 0;
                }
                Console.WriteLine($"Transfering this food to player {client_key_to_send}");
                clients_Dict[client_key_to_send].GetStream().WriteAsync(packet.ToArray(), 0, packet.Length());
                packet.Dispose();
                return;
            }
            else if (!next)
            {
                Packet packet = new Packet();
                packet.Write(foodname);
                packet.WriteLength();
                client_key_to_send -= 1;
                if (client_key_to_send < 0)
                {
                    Console.WriteLine($"Transfering this food to player {client_key_to_send}");

                    clients_Dict.Values.Last().GetStream().WriteAsync(packet.ToArray(),0, packet.Length());
                    packet.Dispose();
                    return;
                }
                Console.WriteLine($"Transfering this food to player {client_key_to_send}");
                clients_Dict[client_key_to_send].GetStream().WriteAsync(packet.ToArray(), 0, packet.Length());
                packet.Dispose();
                return;
            }
        }

    }
}
