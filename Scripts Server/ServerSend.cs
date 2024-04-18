using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoAn_TCPServer
{
    internal class ServerSend
    {

        private static void SendTcpData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Server.clients[_toClient].tcp.SendData(_packet);
        }


        private static void TCP_Broadcast(Packet _packet)
        {
            _packet.WriteLength();
            foreach (Client client in Server.clients.Values)
            {
                client.tcp.SendData(_packet);
            }
        }

        private static void TcpBroadCast(Client except_client ,Packet _packet)
        {
            _packet.WriteLength();
            foreach (Client client in Server.clients.Values)
            {
                if(client != except_client)
                {
                    client.tcp.SendData(_packet);
                }
            }
        }


        public static void Welcome(int _toClient, string _message)
        {
            using (Packet packet = new Packet((int)ServerPackets.welcome))
            {
                packet.Write(_message);
                packet.Write(_toClient);

                SendTcpData(_toClient, packet);
            }
        }

    }
}
