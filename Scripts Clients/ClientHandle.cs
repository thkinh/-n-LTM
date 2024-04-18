
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet packet)
    {
        string msg = packet.ReadString();
        int myID = packet.ReadInt();
        //Debug.Log(msg);
        Debug.Log($"msg from server: {msg}");
        //Console.WriteLine(msg);
        UI_Manager.instance.username.text = msg;
        Client.instance.ID = myID;
        ClientSend.WelcomeReceived();
    }
}
