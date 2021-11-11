﻿using System.Collections;
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
    int playersInRoom;
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
        if (signifier == ServerToClientGameSignifiers.JoinGame)
        {
            if (playersInRoom<2) {
                SendMessageToClient(ServerToClientGameSignifiers.JoinGame + "", id);
                playersInRoom += 1;
            }
        }
        if (signifier == ServerToClientGameSignifiers.JoinAsObserver)
        {
            SendMessageToClient(ServerToClientGameSignifiers.JoinAsObserver + "", id);
        }
        if (signifier == ServerToClientMoveSignifiers.Pos1)
        {
            if (!pos1)
            {
                if (id == 1)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos1 + "", id);
                    pos1 = true;
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

    public static class ServerToClientGameSignifiers
    {
        public const int JoinGame = 1;
        public const int JoinAsObserver = 2;
    }

    public static class ServerToClientTurnSignifiers
    {
        public const int IsMyTurn = 1;
        public const int NotMyTurn = 2;
    }
    public static class ClientToServerTurnSignifiers
    {
        public const int IsMyTurn = 1;
        public const int NotMyTurn = 2;
    }
    public static class ServerToClientMoveSignifiers
    {
        public const int Pos1 = 1;
        public const int Pos2 = 2;
        public const int Pos3 = 3;
        public const int Pos4 = 4;
        public const int Pos5 = 5;
        public const int Pos6 = 6;
        public const int Pos7 = 7;
        public const int Pos8 = 8;
        public const int Pos9 = 9;
    }

    public static class ClientToServerMoveSignifiers
    {
        public const int Pos1 = 1;
        public const int Pos2 = 2;
        public const int Pos3 = 3;
        public const int Pos4 = 4;
        public const int Pos5 = 5;
        public const int Pos6 = 6;
        public const int Pos7 = 7;
        public const int Pos8 = 8;
        public const int Pos9 = 9;
    }
}
