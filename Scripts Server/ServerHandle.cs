using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoAn_TCPServer
{
    internal class ServerHandle
    {
        public static void WelcomeReceived(int _clientID, Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();
            string _username = _packet.ReadString();

            Console.WriteLine($"{Server.clients[_clientID].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_clientID}.");
            if (_clientID != _clientIdCheck)
            {
                Console.WriteLine($"Player \"{_username}\" (ID: {_clientID}) has assumed the wrong client ID ({_clientIdCheck})!");
            }
        }

    }
}
