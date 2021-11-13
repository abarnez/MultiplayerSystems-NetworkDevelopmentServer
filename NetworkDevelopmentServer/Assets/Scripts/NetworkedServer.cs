using System.Text;
using UnityEngine;
using UnityEngine.Networking;

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
                SendMessageToClient(ServerToClientXSignifiers.X + "", id);
            } if(id == 2) {        
                SendMessageToClient(ServerToClientGameSignifiers.JoinGame + "", id);
                SendMessageToClient(ServerToClientTurnSignifiers.NotMyTurn + "", id);
                SendMessageToClient(ServerToClientXSignifiers.O + "", id);

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

                    SendMessageToClient(ClientToServerMoveSignifiers.Pos1 + "", id + 1);
                    pos1 = true;
                    SendMessageToClient(ClientToServerTurnSignifiers.NotMyTurn + "", id);
                    SendMessageToClient(ClientToServerTurnSignifiers.IsMyTurn + "", id+1);

                }
                if (id == 2)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers2.Pos1 + "", id);
                    
                    SendMessageToClient(ClientToServerMoveSignifiers2.Pos1 + "", id - 1);
                    pos1 = true;
                    SendMessageToClient(ClientToServerTurnSignifiers.NotMyTurn + "", id);
                    SendMessageToClient(ClientToServerTurnSignifiers.IsMyTurn + "", id - 1);
                    Debug.Log("it messed up");

                }
            }
        }
        if (signifier == ClientToServerMoveSignifiers.Pos2)
        {
            if (!pos2)
            {
                if (id == 1)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos2 + "", id);
                    Debug.Log("it fired");
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos2 + "", id + 1);
                    pos2 = true;
                    SendMessageToClient(ClientToServerTurnSignifiers.NotMyTurn + "", id);
                    SendMessageToClient(ClientToServerTurnSignifiers.IsMyTurn + "", id + 1);

                }
                if (id == 2)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers2.Pos2 + "", id);
                    Debug.Log("it fired");
                    SendMessageToClient(ClientToServerMoveSignifiers2.Pos2 + "", id - 1);
                    pos2 = true;
                    SendMessageToClient(ClientToServerTurnSignifiers.NotMyTurn + "", id);
                    SendMessageToClient(ClientToServerTurnSignifiers.IsMyTurn + "", id - 1);

                }
            }
        }
        if (signifier == ClientToServerMoveSignifiers.Pos3)
        {
            if (!pos3)
            {
                if (id == 1)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos3 + "", id);
                    Debug.Log("it fired");
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos3 + "", id + 1);
                    pos2 = true;
                    SendMessageToClient(ClientToServerTurnSignifiers.NotMyTurn + "", id);
                    SendMessageToClient(ClientToServerTurnSignifiers.IsMyTurn + "", id + 1);

                }
                if (id == 2)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers2.Pos3 + "", id);
                    Debug.Log("it fired");
                    SendMessageToClient(ClientToServerMoveSignifiers2.Pos3 + "", id - 1);
                    pos3 = true;
                    SendMessageToClient(ClientToServerTurnSignifiers.NotMyTurn + "", id);
                    SendMessageToClient(ClientToServerTurnSignifiers.IsMyTurn + "", id - 1);

                }
            }
        }
        if(signifier == ClientToServerMoveSignifiers.Pos4)
        {
            if (!pos4)
            {
                if (id == 1)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos4 + "", id);
                    Debug.Log("it fired");
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos4 + "", id + 1);
                    pos4 = true;
                    SendMessageToClient(ClientToServerTurnSignifiers.NotMyTurn + "", id);
                    SendMessageToClient(ClientToServerTurnSignifiers.IsMyTurn + "", id + 1);

                }
                if (id == 2)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers2.Pos4 + "", id);
                    Debug.Log("it fired");
                    SendMessageToClient(ClientToServerMoveSignifiers2.Pos4 + "", id - 1);
                    pos4 = true;
                    SendMessageToClient(ClientToServerTurnSignifiers.NotMyTurn + "", id);
                    SendMessageToClient(ClientToServerTurnSignifiers.IsMyTurn + "", id - 1);

                }
            }
        }
        if (signifier == ClientToServerMoveSignifiers.Pos5)
        {
            if (!pos5)
            {
                if (id == 1)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos5 + "", id);
                    Debug.Log("it fired");
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos5 + "", id + 1);
                    pos5 = true;
                    SendMessageToClient(ClientToServerTurnSignifiers.NotMyTurn + "", id);
                    SendMessageToClient(ClientToServerTurnSignifiers.IsMyTurn + "", id + 1);

                }
                if (id == 2)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers2.Pos5 + "", id);
                    Debug.Log("it fired");
                    SendMessageToClient(ClientToServerMoveSignifiers2.Pos5 + "", id - 1);
                    pos5 = true;
                    SendMessageToClient(ClientToServerTurnSignifiers.NotMyTurn + "", id);
                    SendMessageToClient(ClientToServerTurnSignifiers.IsMyTurn + "", id - 1);

                }
            }
        }
        if (signifier == ClientToServerMoveSignifiers.Pos6)
        {
            if (!pos6)
            {
                if (id == 1)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos6 + "", id);
                    Debug.Log("it fired");
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos6 + "", id + 1);
                    pos6 = true;
                    SendMessageToClient(ClientToServerTurnSignifiers.NotMyTurn + "", id);
                    SendMessageToClient(ClientToServerTurnSignifiers.IsMyTurn + "", id + 1);

                }
                if (id == 2)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers2.Pos6 + "", id);
                    Debug.Log("it fired");
                    SendMessageToClient(ClientToServerMoveSignifiers2.Pos6 + "", id - 1);
                    pos6 = true;
                    SendMessageToClient(ClientToServerTurnSignifiers.NotMyTurn + "", id);
                    SendMessageToClient(ClientToServerTurnSignifiers.IsMyTurn + "", id - 1);

                }
            }
        }
        if (signifier == ClientToServerMoveSignifiers.Pos7)
        {
            if (!pos7)
            {
                if (id == 1)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos7 + "", id);
                    Debug.Log("it fired");
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos7 + "", id + 1);
                    pos7 = true;
                    SendMessageToClient(ClientToServerTurnSignifiers.NotMyTurn + "", id);
                    SendMessageToClient(ClientToServerTurnSignifiers.IsMyTurn + "", id + 1);

                }
                if (id == 2)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers2.Pos7 + "", id);
                    Debug.Log("it fired");
                    SendMessageToClient(ClientToServerMoveSignifiers2.Pos7 + "", id - 1);
                    pos7 = true;
                    SendMessageToClient(ClientToServerTurnSignifiers.NotMyTurn + "", id);
                    SendMessageToClient(ClientToServerTurnSignifiers.IsMyTurn + "", id - 1);

                }
            }
        }
        if (signifier == ClientToServerMoveSignifiers.Pos8)
        {
            if (!pos8)
            {
                if (id == 1)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos8 + "", id);
                    Debug.Log("it fired");
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos8 + "", id + 1);
                    pos8 = true;
                    SendMessageToClient(ClientToServerTurnSignifiers.NotMyTurn + "", id);
                    SendMessageToClient(ClientToServerTurnSignifiers.IsMyTurn + "", id + 1);

                }
                if (id == 2)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers2.Pos8 + "", id);
                    Debug.Log("it fired");
                    SendMessageToClient(ClientToServerMoveSignifiers2.Pos8 + "", id - 1);
                    pos8 = true;
                    SendMessageToClient(ClientToServerTurnSignifiers.NotMyTurn + "", id);
                    SendMessageToClient(ClientToServerTurnSignifiers.IsMyTurn + "", id - 1);

                }
            }
        }
        if (signifier == ClientToServerMoveSignifiers.Pos9)
        {
            if (!pos9)
            {
                if (id == 1)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos9 + "", id);
                    Debug.Log("it fired");
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos9 + "", id + 1);
                    pos9 = true;
                    SendMessageToClient(ClientToServerTurnSignifiers.NotMyTurn + "", id);
                    SendMessageToClient(ClientToServerTurnSignifiers.IsMyTurn + "", id + 1);

                }
                if (id == 2)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers2.Pos9 + "", id);
                    Debug.Log("it fired");
                    SendMessageToClient(ClientToServerMoveSignifiers2.Pos9 + "", id - 1);
                    pos9 = true;
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

    public static class ServerToClientMoveSignifiers2
    {
        public const int Pos1 = 22;
        public const int Pos2 = 23;
        public const int Pos3 = 24;
        public const int Pos4 = 25;
        public const int Pos5 = 26;
        public const int Pos6 = 27;
        public const int Pos7 = 28;
        public const int Pos8 = 29;
        public const int Pos9 = 30;
    }

    public static class ClientToServerMoveSignifiers2
    {
        public const int Pos1 = 22;
        public const int Pos2 = 23;
        public const int Pos3 = 24;
        public const int Pos4 = 25;
        public const int Pos5 = 26;
        public const int Pos6 = 27;
        public const int Pos7 = 28;
        public const int Pos8 = 29;
        public const int Pos9 = 30;
    }

    public static class ClientToServerXSignifiers
    {
        public const int X = 31;
        public const int O = 32;
    }
    public static class ServerToClientXSignifiers
    {
        public const int X = 31;
        public const int O = 32;
    }
}
