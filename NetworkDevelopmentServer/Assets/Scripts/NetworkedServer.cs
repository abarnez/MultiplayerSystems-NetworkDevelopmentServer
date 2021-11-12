using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class NetworkedServer : MonoBehaviour
{
    int maxConnections = 1000;
    int reliableChannelID;
    int unreliableChannelID;
    int hostID;
    int socketPort = 5491;
    public int playersInRoom;
    bool pos1, pos2, pos3, pos4, pos5, pos6, pos7, pos8, pos9;
    // Start is called before the first frame update
    void Start()
    {
        NetworkTransport.Init();
        ConnectionConfig config = new ConnectionConfig();
        reliableChannelID = config.AddChannel(QosType.Reliable);
        unreliableChannelID = config.AddChannel(QosType.Unreliable);
        HostTopology topology = new HostTopology(config, maxConnections);
        hostID = NetworkTransport.AddHost(topology, socketPort, null);
    }

    // Update is called once per frame
    void Update()
    {
        int recHostID;
        int recConnectionID;
        int recChannelID;
        byte[] recBuffer = new byte[1024];
        int bufferSize = 1024;
        int dataSize;
        byte error = 0;

        NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, recBuffer, bufferSize, out dataSize, out error);

        switch (recNetworkEvent)
        {
            case NetworkEventType.Nothing:
                break;
            case NetworkEventType.ConnectEvent:
                Debug.Log("Connection, " + recConnectionID);
                break;
            case NetworkEventType.DataEvent:
                string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                ProcessRecievedMsg(msg, recConnectionID);
                break;
            case NetworkEventType.DisconnectEvent:
                Debug.Log("Disconnection, " + recConnectionID);
                break;
        }
    }
    public void SendMessageToClient(string msg, int id)
    {
        byte error = 0;
        byte[] buffer = Encoding.Unicode.GetBytes(msg);
        NetworkTransport.Send(hostID, id, reliableChannelID, buffer, msg.Length * sizeof(char), out error);
    }

    private void ProcessRecievedMsg(string msg, int id)
    {
        Debug.Log("msg recieved = " + msg + ".  connection id = " + id);

        string[] csv = msg.Split(',');
        int signifier = int.Parse(csv[0]);
        if (signifier == ClientToServerChatSignifiers.GG)
        {
            if(id==2)
            SendMessageToClient(ServerToClientChatSignifiers.GG + "", id);
            SendMessageToClient(ServerToClientChatSignifiers.GG + "", id-1);
            if(id==1)
            SendMessageToClient(ServerToClientChatSignifiers.GG + "", id+1);
            SendMessageToClient(ServerToClientChatSignifiers.GG + "", id);
        }
        if (signifier == ClientToServerChatSignifiers.Rematch)
        {
            if (id == 2)
                SendMessageToClient(ServerToClientChatSignifiers.Rematch + "", id);
            SendMessageToClient(ServerToClientChatSignifiers.Rematch + "", id - 1);
            if (id == 1)
                SendMessageToClient(ServerToClientChatSignifiers.Rematch + "", id + 1);
            SendMessageToClient(ServerToClientChatSignifiers.Rematch + "", id);
        }
        if (signifier == ClientToServerChatSignifiers.EZCLap)
        {
            if (id == 2)
                SendMessageToClient(ServerToClientChatSignifiers.EZCLap + "", id);
            SendMessageToClient(ServerToClientChatSignifiers.EZCLap + "", id - 1);
            if (id == 1)
                SendMessageToClient(ServerToClientChatSignifiers.EZCLap + "", id + 1);
            SendMessageToClient(ServerToClientChatSignifiers.EZCLap + "", id);
        }
        if (signifier == ClientToServerGameSignifiers.JoinGame)
        {
            if (id == 1)
            {
                SendMessageToClient(ServerToClientGameSignifiers.JoinGame + "", id);
                SendMessageToClient(ServerToClientTurnSignifiers.IsMyTurn + "", id);
            } if(id == 2) {        
                SendMessageToClient(ServerToClientGameSignifiers.JoinGame + "", id);
                SendMessageToClient(ServerToClientTurnSignifiers.NotMyTurn + "", id);
            }
          
        }
        if (signifier == ClientToServerGameSignifiers.JoinAsObserver)
        {
            SendMessageToClient(ServerToClientGameSignifiers.JoinAsObserver + "", id);
        }
        if (signifier == ClientToServerMoveSignifiers.Pos1)
        {
            if (!pos1)
            {
                if (id == 1)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos1 + "", id);
                    Debug.Log("it fired");
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos1 + "", id + 1);
                    pos1 = true;
                    SendMessageToClient(ClientToServerTurnSignifiers.NotMyTurn + "", id);
                    SendMessageToClient(ClientToServerTurnSignifiers.IsMyTurn + "", id+1);

                }
                if (id == 2)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos1 + "", id);
                    Debug.Log("it fired");
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos1 + "", id - 1);
                    pos1 = true;
                    SendMessageToClient(ClientToServerTurnSignifiers.NotMyTurn + "", id);
                    SendMessageToClient(ClientToServerTurnSignifiers.IsMyTurn + "", id - 1);

                }
            }
        }
    }

    public static class ClientToServerChatSignifiers
    {
        public const int GG = 1;
        public const int Rematch = 2;
        public const int EZCLap = 3;
    }

    public static class ServerToClientChatSignifiers
    {
        public const int GG = 1;
        public const int Rematch = 2;
        public const int EZCLap = 3;
    }

    public static class ClientToServerGameSignifiers
    {
        public const int JoinGame = 4;
        public const int JoinAsObserver = 5;
    }


    public static class ServerToClientGameSignifiers
    {
        public const int JoinGame = 4;
        public const int JoinAsObserver = 5;
    }

    public static class ServerToClientMoveSignifiers
    {
        public const int Pos1 = 11;
        public const int Pos2 = 12;
        public const int Pos3 = 13;
        public const int Pos4 = 14;
        public const int Pos5 = 15;
        public const int Pos6 = 16;
        public const int Pos7 = 17;
        public const int Pos8 = 18;
        public const int Pos9 = 19;
    }

    public static class ClientToServerMoveSignifiers
    {
        public const int Pos1 = 11;
        public const int Pos2 = 12;
        public const int Pos3 = 13;
        public const int Pos4 = 14;
        public const int Pos5 = 15;
        public const int Pos6 = 16;
        public const int Pos7 = 17;
        public const int Pos8 = 18;
        public const int Pos9 = 19;
    }
    public static class ServerToClientTurnSignifiers
    {
        public const int IsMyTurn = 20;
        public const int NotMyTurn = 21;
    }
    public static class ClientToServerTurnSignifiers
    {
        public const int IsMyTurn = 20;
        public const int NotMyTurn = 21;
    }
}
