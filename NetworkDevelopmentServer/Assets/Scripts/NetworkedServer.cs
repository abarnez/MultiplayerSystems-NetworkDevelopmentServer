using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NetworkedServer : MonoBehaviour
{
    int maxConnections = 1000;
    int reliableChannelID;
    int unreliableChannelID;
    int hostID;
    int socketPort = 5491;
    public int playersInRoom;
    public bool p1Won, p2Won;
    bool MsgSent;
    public bool pos1, pos2, pos3, pos4, pos5, pos6, pos7, pos8, pos9;
    bool xpos1, xpos2, xpos3, xpos4, xpos5, xpos6, xpos7, xpos8, xpos9;
    public int MoveCounter, Move1 = 0, Move2 = 0, Move3 = 0, Move4 = 0, Move5 = 0, Move6 = 0, Move7 = 0, Move8 = 0, Move9 = 0;
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

        if(pos1 && pos2 && pos3)
        {
            p1Won = true;
        }
        if (pos4 && pos5 && pos6)
        {
            p1Won = true;
        }
        if (pos4 && pos5 && pos6)
        {
            p1Won = true;
        }
        if (pos7 && pos8 && pos9)
        {
            p1Won = true;
        }
        if (pos1 && pos4 && pos7)
        {
            p1Won = true;
        }
        if (pos2 && pos5 && pos8)
        {
            p1Won = true;
        }
        if (pos3 && pos6 && pos9)
        {
            p1Won = true;
        }
        if (pos1 && pos5 && pos9)
        {
            p1Won = true;
        }
        if (pos3 && pos5 && pos7)
        {
            p1Won = true;
        }
        if (xpos1 && xpos2 && xpos3)
        {
            p2Won = true;
        }
        if (xpos4 && xpos5 && xpos6)
        {
            p2Won = true;
        }
        if (xpos4 && xpos5 && xpos6)
        {
            p2Won = true;
        }
        if (xpos7 && xpos8 && xpos9)
        {
            p2Won = true;
        }
        if (xpos1 && xpos4 && xpos7)
        {
            p2Won = true;
        }
        if (xpos2 && xpos5 && xpos8)
        {
            p2Won = true;
        }
        if (xpos3 && xpos6 && xpos9)
        {
            p2Won = true;
        }
        if (xpos1 && xpos5 && xpos9)
        {
            p2Won = true;
        }
        if (xpos3 && xpos5 && xpos7)
        {
            p2Won = true;
        }
        if (!MsgSent)
        {
            if (p2Won)
            {
                SendMessageToClient(ServerToClientGOSignifiers.p2Won + "", 1);
                SendMessageToClient(ServerToClientGOSignifiers.p2Won + "", 2);
                MsgSent = true;
            }
            if (p1Won)
            {
                SendMessageToClient(ServerToClientGOSignifiers.p1Won + "", 1);
                SendMessageToClient(ServerToClientGOSignifiers.p1Won + "", 2);
                MsgSent = true;
            }
        }


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
            if (!pos1 && !xpos1)
            {
                if (id == 1)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos1 + "", id);
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos1 + "", id + 1);
                    pos1 = true;
                    SendMessageToClient(ClientToServerTurnSignifiers.NotMyTurn + "", id);
                    SendMessageToClient(ClientToServerTurnSignifiers.IsMyTurn + "", id+1);
                    if(MoveCounter == 0)
                    {
                        Move1 = 1;
                        MoveCounter++;
                    }
                    if (MoveCounter == 2)
                    {
                        Move3 = 1;
                        MoveCounter++;
                    }
                    if (MoveCounter == 4)
                    {
                        Move5 = 1;
                        MoveCounter++;
                    }
                    if (MoveCounter == 6)
                    {
                        Move7 = 1;
                        MoveCounter++;
                    }
                    if (MoveCounter == 8)
                    {
                        Move9 = 1;
                        MoveCounter++;
                    }

                }
                if (id == 2)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers2.Pos1 + "", id);                   
                    SendMessageToClient(ClientToServerMoveSignifiers2.Pos1 + "", id - 1);
                    xpos1 = true;
                    SendMessageToClient(ClientToServerTurnSignifiers.NotMyTurn + "", id);
                    SendMessageToClient(ClientToServerTurnSignifiers.IsMyTurn + "", id - 1);
                    if (MoveCounter == 1)
                    {
                        Move2 = 1;
                        MoveCounter++;
                    }
                    if (MoveCounter == 3)
                    {
                        Move4 = 1;
                        MoveCounter++;
                    }
                    if (MoveCounter == 5)
                    {
                        Move6 = 1;
                        MoveCounter++;
                    }
                    if (MoveCounter == 7)
                    {
                        Move8 = 1;
                        MoveCounter++;
                    }               
                }
            }
        }
        if (signifier == ClientToServerMoveSignifiers.Pos2)
        {
            if (!pos2 && !xpos2)
            {
                if (id == 1)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos2 + "", id);
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos2 + "", id + 1);
                    pos2 = true;
                    SendMessageToClient(ClientToServerTurnSignifiers.NotMyTurn + "", id);
                    SendMessageToClient(ClientToServerTurnSignifiers.IsMyTurn + "", id + 1);
                    if (MoveCounter == 0)
                    {
                        Move1 = 2;
                        MoveCounter++;
                    }
                    if (MoveCounter == 2)
                    {
                        Move3 = 2;
                        MoveCounter++;
                    }
                    if (MoveCounter == 4)
                    {
                        Move5 = 2;
                        MoveCounter++;
                    }
                    if (MoveCounter == 6)
                    {
                        Move7 = 2;
                        MoveCounter++;
                    }
                    if (MoveCounter == 8)
                    {
                        Move9 = 2;
                        MoveCounter++;
                    }
                }
                if (id == 2)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers2.Pos2 + "", id);
                    SendMessageToClient(ClientToServerMoveSignifiers2.Pos2 + "", id - 1);
                    xpos2 = true;
                    SendMessageToClient(ClientToServerTurnSignifiers.NotMyTurn + "", id);
                    SendMessageToClient(ClientToServerTurnSignifiers.IsMyTurn + "", id - 1);
                    if (MoveCounter == 1)
                    {
                        Move2 = 2;
                        MoveCounter++;
                    }
                    if (MoveCounter == 3)
                    {
                        Move4 = 2;
                        MoveCounter++;
                    }
                    if (MoveCounter == 5)
                    {
                        Move6 = 2;
                        MoveCounter++;
                    }
                    if (MoveCounter == 7)
                    {
                        Move8 = 2;
                        MoveCounter++;
                    }
                }
            }
        }
        if (signifier == ClientToServerMoveSignifiers.Pos3)
        {
            if (!pos3 && !xpos3)
            {
                if (id == 1)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos3 + "", id);
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos3 + "", id + 1);
                    pos3 = true;
                    SendMessageToClient(ClientToServerTurnSignifiers.NotMyTurn + "", id);
                    SendMessageToClient(ClientToServerTurnSignifiers.IsMyTurn + "", id + 1);
                    if (MoveCounter == 0)
                    {
                        Move1 = 3;
                        MoveCounter++;
                    }
                    if (MoveCounter == 2)
                    {
                        Move3 = 3;
                        MoveCounter++;
                    }
                    if (MoveCounter == 4)
                    {
                        Move5 = 3;
                        MoveCounter++;
                    }
                    if (MoveCounter == 6)
                    {
                        Move7 = 3;
                        MoveCounter++;
                    }
                    if (MoveCounter == 8)
                    {
                        Move9 = 3;
                        MoveCounter++;
                    }
                }
                if (id == 2)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers2.Pos3 + "", id);
                    SendMessageToClient(ClientToServerMoveSignifiers2.Pos3 + "", id - 1);
                    xpos3 = true;
                    SendMessageToClient(ClientToServerTurnSignifiers.NotMyTurn + "", id);
                    SendMessageToClient(ClientToServerTurnSignifiers.IsMyTurn + "", id - 1);
                    if (MoveCounter == 1)
                    {
                        Move2 = 3;
                        MoveCounter++;
                    }
                    if (MoveCounter == 3)
                    {
                        Move4 = 3;
                        MoveCounter++;
                    }
                    if (MoveCounter == 5)
                    {
                        Move6 = 3;
                        MoveCounter++;
                    }
                    if (MoveCounter == 7)
                    {
                        Move8 = 3;
                        MoveCounter++;
                    }
                }
            }
        }
        if (signifier == ClientToServerMoveSignifiers.Pos4)
        {
            if (!pos4 && !xpos4)
            {
                if (id == 1)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos4 + "", id);
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos4 + "", id + 1);
                    pos4 = true;
                    SendMessageToClient(ClientToServerTurnSignifiers.NotMyTurn + "", id);
                    SendMessageToClient(ClientToServerTurnSignifiers.IsMyTurn + "", id + 1);
                    if (MoveCounter == 0)
                    {
                        Move1 = 4;
                        MoveCounter++;
                    }
                    if (MoveCounter == 2)
                    {
                        Move3 = 4;
                        MoveCounter++;
                    }
                    if (MoveCounter == 4)
                    {
                        Move5 = 4;
                        MoveCounter++;
                    }
                    if (MoveCounter == 6)
                    {
                        Move7 = 4;
                        MoveCounter++;
                    }
                    if (MoveCounter == 8)
                    {
                        Move9 = 4;
                        MoveCounter++;
                    }
                }
                if (id == 2)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers2.Pos4 + "", id);
                    SendMessageToClient(ClientToServerMoveSignifiers2.Pos4 + "", id - 1);
                    xpos4 = true;
                    SendMessageToClient(ClientToServerTurnSignifiers.NotMyTurn + "", id);
                    SendMessageToClient(ClientToServerTurnSignifiers.IsMyTurn + "", id - 1);
                    if (MoveCounter == 1)
                    {
                        Move2 = 4;
                        MoveCounter++;
                    }
                    if (MoveCounter == 3)
                    {
                        Move4 = 4;
                        MoveCounter++;
                    }
                    if (MoveCounter == 5)
                    {
                        Move6 = 4;
                        MoveCounter++;
                    }
                    if (MoveCounter == 7)
                    {
                        Move8 = 4;
                        MoveCounter++;
                    }
                }
            }
        }
        if (signifier == ClientToServerMoveSignifiers.Pos5)
        {
            if (!pos5 && !xpos5)
            {
                if (id == 1)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos5 + "", id);
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos5 + "", id + 1);
                    pos5 = true;
                    SendMessageToClient(ClientToServerTurnSignifiers.NotMyTurn + "", id);
                    SendMessageToClient(ClientToServerTurnSignifiers.IsMyTurn + "", id + 1);
                    if (MoveCounter == 0)
                    {
                        Move1 = 5;
                        MoveCounter++;
                    }
                    if (MoveCounter == 2)
                    {
                        Move3 = 5;
                        MoveCounter++;
                    }
                    if (MoveCounter == 4)
                    {
                        Move5 = 5;
                        MoveCounter++;
                    }
                    if (MoveCounter == 6)
                    {
                        Move7 = 5;
                        MoveCounter++;
                    }
                    if (MoveCounter == 8)
                    {
                        Move9 = 5;
                        MoveCounter++;
                    }
                }
                if (id == 2)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers2.Pos5 + "", id);
                    SendMessageToClient(ClientToServerMoveSignifiers2.Pos5 + "", id - 1);
                    xpos5 = true;
                    SendMessageToClient(ClientToServerTurnSignifiers.NotMyTurn + "", id);
                    SendMessageToClient(ClientToServerTurnSignifiers.IsMyTurn + "", id - 1);
                    if (MoveCounter == 1)
                    {
                        Move2 = 5;
                        MoveCounter++;
                    }
                    if (MoveCounter == 3)
                    {
                        Move4 = 5;
                        MoveCounter++;
                    }
                    if (MoveCounter == 5)
                    {
                        Move6 = 5;
                        MoveCounter++;
                    }
                    if (MoveCounter == 7)
                    {
                        Move8 = 5;
                        MoveCounter++;
                    }
                }
            }
        }
        if (signifier == ClientToServerMoveSignifiers.Pos6)
        {
            if (!pos6 && !xpos6)
            {
                if (id == 1)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos6 + "", id);
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos6 + "", id + 1);
                    pos6 = true;
                    SendMessageToClient(ClientToServerTurnSignifiers.NotMyTurn + "", id);
                    SendMessageToClient(ClientToServerTurnSignifiers.IsMyTurn + "", id + 1);
                    if (MoveCounter == 0)
                    {
                        Move1 = 6;
                        MoveCounter++;
                    }
                    if (MoveCounter == 2)
                    {
                        Move3 = 6;
                        MoveCounter++;
                    }
                    if (MoveCounter == 4)
                    {
                        Move5 = 6;
                        MoveCounter++;
                    }
                    if (MoveCounter == 6)
                    {
                        Move7 = 6;
                        MoveCounter++;
                    }
                    if (MoveCounter == 8)
                    {
                        Move9 = 6;
                        MoveCounter++;
                    }
                }
                if (id == 2)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers2.Pos6 + "", id);
                    SendMessageToClient(ClientToServerMoveSignifiers2.Pos6 + "", id - 1);
                    xpos6 = true;
                    SendMessageToClient(ClientToServerTurnSignifiers.NotMyTurn + "", id);
                    SendMessageToClient(ClientToServerTurnSignifiers.IsMyTurn + "", id - 1);
                    if (MoveCounter == 1)
                    {
                        Move2 = ClientToServerMoveSignifiers.Pos6;
                        MoveCounter++;
                    }
                    if (MoveCounter == 3)
                    {
                        Move4 = ClientToServerMoveSignifiers.Pos6;
                        MoveCounter++;
                    }
                    if (MoveCounter == 5)
                    {
                        Move6 = ClientToServerMoveSignifiers.Pos6;
                        MoveCounter++;
                    }
                    if (MoveCounter == 7)
                    {
                        Move8 = ClientToServerMoveSignifiers.Pos6;
                        MoveCounter++;
                    }
                }
            }
        }
        if (signifier == ClientToServerMoveSignifiers.Pos7)
        {
            if (!pos7 && !xpos7)
            {
                if (id == 1)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos7 + "", id);
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos7 + "", id + 1);
                    pos7 = true;
                    SendMessageToClient(ClientToServerTurnSignifiers.NotMyTurn + "", id);
                    SendMessageToClient(ClientToServerTurnSignifiers.IsMyTurn + "", id + 1);
                    if (MoveCounter == 0)
                    {
                        Move1 = 7;
                        MoveCounter++;
                    }
                    if (MoveCounter == 2)
                    {
                        Move3 = 7;
                        MoveCounter++;
                    }
                    if (MoveCounter == 4)
                    {
                        Move5 = 7;
                        MoveCounter++;
                    }
                    if (MoveCounter == 6)
                    {
                        Move7 = 7;
                        MoveCounter++;
                    }
                    if (MoveCounter == 8)
                    {
                        Move9 = 7;
                        MoveCounter++;
                    }
                }
                if (id == 2)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers2.Pos7 + "", id);
                    SendMessageToClient(ClientToServerMoveSignifiers2.Pos7 + "", id - 1);
                    xpos7 = true;
                    SendMessageToClient(ClientToServerTurnSignifiers.NotMyTurn + "", id);
                    SendMessageToClient(ClientToServerTurnSignifiers.IsMyTurn + "", id - 1);
                    if (MoveCounter == 1)
                    {
                        Move2 = 7;
                        MoveCounter++;
                    }
                    if (MoveCounter == 3)
                    {
                        Move4 = 7;
                        MoveCounter++;
                    }
                    if (MoveCounter == 5)
                    {
                        Move6 = 7;
                        MoveCounter++;
                    }
                    if (MoveCounter == 7)
                    {
                        Move8 = 7;
                        MoveCounter++;
                    }
                }
            }
        }
        if (signifier == ClientToServerMoveSignifiers.Pos8)
        {
            if (!pos8 && !xpos8)
            {
                if (id == 1)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos8 + "", id);
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos8 + "", id + 1);
                    pos8 = true;
                    SendMessageToClient(ClientToServerTurnSignifiers.NotMyTurn + "", id);
                    SendMessageToClient(ClientToServerTurnSignifiers.IsMyTurn + "", id + 1);
                    if (MoveCounter == 0)
                    {
                        Move1 = 8;
                        MoveCounter++;
                    }
                    if (MoveCounter == 2)
                    {
                        Move3 = 8;
                        MoveCounter++;
                    }
                    if (MoveCounter == 4)
                    {
                        Move5 = 8;
                        MoveCounter++;
                    }
                    if (MoveCounter == 6)
                    {
                        Move7 = 8;
                        MoveCounter++;
                    }
                    if (MoveCounter == 8)
                    {
                        Move9 = 8;
                        MoveCounter++;
                    }
                }
                if (id == 2)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers2.Pos8 + "", id);
                    SendMessageToClient(ClientToServerMoveSignifiers2.Pos8 + "", id - 1);
                    xpos8 = true;
                    SendMessageToClient(ClientToServerTurnSignifiers.NotMyTurn + "", id);
                    SendMessageToClient(ClientToServerTurnSignifiers.IsMyTurn + "", id - 1);
                    if (MoveCounter == 1)
                    {
                        Move2 = 8;
                        MoveCounter++;
                    }
                    if (MoveCounter == 3)
                    {
                        Move4 = 8;
                        MoveCounter++;
                    }
                    if (MoveCounter == 5)
                    {
                        Move6 = 8;
                        MoveCounter++;
                    }
                    if (MoveCounter == 7)
                    {
                        Move8 = 8;
                        MoveCounter++;
                    }
                }
            }
        }
        if (signifier == ClientToServerMoveSignifiers.Pos9)
        {
            if (!pos9 && !xpos9)
            {
                if (id == 1)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos9 + "", id);
                    SendMessageToClient(ClientToServerMoveSignifiers.Pos9 + "", id + 1);
                    pos9 = true;
                    SendMessageToClient(ClientToServerTurnSignifiers.NotMyTurn + "", id);
                    SendMessageToClient(ClientToServerTurnSignifiers.IsMyTurn + "", id + 1);
                    if (MoveCounter == 0)
                    {
                        Move1 = 9;
                        MoveCounter++;
                    }
                    if (MoveCounter == 2)
                    {
                        Move3 = 9;
                        MoveCounter++;
                    }
                    if (MoveCounter == 4)
                    {
                        Move5 = 9;
                        MoveCounter++;
                    }
                    if (MoveCounter == 6)
                    {
                        Move7 = 9;
                        MoveCounter++;
                    }
                    if (MoveCounter == 8)
                    {
                        Move9 = 9;
                        MoveCounter++;
                    }
                }
                if (id == 2)
                {
                    SendMessageToClient(ClientToServerMoveSignifiers2.Pos9 + "", id);
                    SendMessageToClient(ClientToServerMoveSignifiers2.Pos9 + "", id - 1);
                    xpos9 = true;
                    SendMessageToClient(ClientToServerTurnSignifiers.NotMyTurn + "", id);
                    SendMessageToClient(ClientToServerTurnSignifiers.IsMyTurn + "", id - 1);
                    if (MoveCounter == 1)
                    {
                        Move2 = ClientToServerMoveSignifiers.Pos9;
                        MoveCounter++;
                    }
                    if (MoveCounter == 3)
                    {
                        Move4 = ClientToServerMoveSignifiers.Pos9;
                        MoveCounter++;
                    }
                    if (MoveCounter == 5)
                    {
                        Move6 = ClientToServerMoveSignifiers.Pos9;
                        MoveCounter++;
                    }
                    if (MoveCounter == 7)
                    {
                        Move8 = ClientToServerMoveSignifiers.Pos9;
                        MoveCounter++;
                    }
                }
            }
        }
        if (signifier == ClientToServerRPSignifiers.Replay)
        {
            SendMessageToClient(ServerToClientRPSignifiers.Replay + "", id);
            SendMessageToClient(ServerToClientRPSignifiers.Replay + "", id+1);
            Replay();
        }
    
    }
    IEnumerator ExampleCoroutine()
    {
        if (Move1 != 0)
        {
            if (Move1 == 1)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos1 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos1 + "", 2);
            }
            if (Move1 == 2)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos2 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos2 + "", 2);
            }
            if (Move1 == 3)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos3 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos3 + "", 2);
            }
            if (Move1 == 4)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos4 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos4 + "", 2);
            }
            if (Move1 == 5)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos5 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos5 + "", 2);
            }
            if (Move1 == 6)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos6 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos6 + "", 2);
            }
            if (Move1 == 7)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos7 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos7 + "", 2);
            }
            if (Move1 == 8)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos8 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos9 + "", 2);
            }
            if (Move1 == 9)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos9 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos9 + "", 2);
            }
        }

        yield return new WaitForSeconds(2);

        if (Move2 != 0)
        {
            if (Move2 == 1)
            {
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos1 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos1 + "", 2);
            }
            if (Move2 == 2)
            {
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos2 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos2 + "", 2);

            }
            if (Move2 == 3)
            {
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos3 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos3 + "", 2);
            }
            if (Move2 == 4)
            {
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos4 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos4 + "", 2);
            }
            if (Move2 == 5)
            {
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos5 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos5 + "", 2);
            }
            if (Move2 == 6)
            {
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos6 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos6 + "", 2);
            }
            if (Move2 == 7)
            {
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos7 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos7 + "", 2);
            }
            if (Move2 == 8)
            {
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos8 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos9 + "", 2);
            }
            if (Move2 == 9)
            {
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos9 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos9 + "", 2);
            }
        }

        yield return new WaitForSeconds(2);
        if (Move3 != 0)
        {
            if (Move3 == 1)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos1 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos1 + "", 2);
            }
            if (Move3 == 2)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos2 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos2 + "", 2);
            }
            if (Move3 == 3)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos3 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos3 + "", 2);
            }
            if (Move3 == 4)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos4 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos4 + "", 2);
            }
            if (Move3 == 5)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos5 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos5 + "", 2);
            }
            if (Move3 == 6)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos6 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos6 + "", 2);
            }
            if (Move3 == 7)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos7 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos7 + "", 2);
            }
            if (Move3 == 8)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos8 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos9 + "", 2);
            }
            if (Move3 == 9)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos9 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos9 + "", 2);
            }
        }
        yield return new WaitForSeconds(2);
        if (Move4 != 0)
        {
            if (Move4 == 1)
            {
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos1 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos1 + "", 2);
            }
            if (Move4 == 2)
            {
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos2 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos2 + "", 2);
            }
            if (Move4 == 3)
            {
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos3 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos3 + "", 2);
            }
            if (Move4 == 4)
            {
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos4 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos4 + "", 2);
            }
            if (Move4 == 5)
            {
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos5 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos5 + "", 2);
            }
            if (Move4 == 6)
            {
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos6 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos6 + "", 2);
            }
            if (Move4 == 7)
            {
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos7 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos7 + "", 2);
            }
            if (Move4 == 8)
            {
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos8 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos9 + "", 2);
            }
            if (Move4 == 9)
            {
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos9 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos9 + "", 2);
            }
        }
        yield return new WaitForSeconds(2);
        if (Move5 != 0)
        {
            if (Move5 == 1)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos1 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos1 + "", 2);
            }
            if (Move5 == 2)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos2 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos2 + "", 2);
            }
            if (Move5 == 3)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos3 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos3 + "", 2);
            }
            if (Move5 == 4)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos4 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos4 + "", 2);
            }
            if (Move5 == 5)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos5 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos5 + "", 2);
            }
            if (Move5 == 6)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos6 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos6 + "", 2);
            }
            if (Move5 == 7)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos7 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos7 + "", 2);
            }
            if (Move5 == 8)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos8 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos9 + "", 2);
            }
            if (Move5 == 9)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos9 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos9 + "", 2);
            }
        }
        yield return new WaitForSeconds(2);
        if (Move6 != 0)
        {
            if (Move6 == 1)
            {
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos1 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos1 + "", 2);
            }
            if (Move6 == 2)
            {
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos2 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos2 + "", 2);
            }
            if (Move6 == 3)
            {
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos3 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos3 + "", 2);
            }
            if (Move6 == 4)
            {
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos4 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos4 + "", 2);
            }
            if (Move6 == 5)
            {
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos5 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos5 + "", 2);
            }
            if (Move6 == 6)
            {
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos6 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos6 + "", 2);
            }
            if (Move6 == 7)
            {
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos7 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos7 + "", 2);
            }
            if (Move6 == 8)
            {
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos8 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos9 + "", 2);
            }
            if (Move6 == 9)
            {
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos9 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos9 + "", 2);
            }
        }
        yield return new WaitForSeconds(2);
        if (Move7 != 0)
        {
            if (Move7 == 1)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos1 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos1 + "", 2);
            }
            if (Move7 == 2)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos2 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos2 + "", 2);
            }
            if (Move7 == 3)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos3 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos3 + "", 2);
            }
            if (Move7 == 4)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos4 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos4 + "", 2);
            }
            if (Move7 == 5)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos5 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos5 + "", 2);
            }
            if (Move7 == 6)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos6 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos6 + "", 2);
            }
            if (Move7 == 7)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos7 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos7 + "", 2);
            }
            if (Move7 == 8)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos8 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos9 + "", 2);
            }
            if (Move7 == 9)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos9 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos9 + "", 2);
            }
        }
        yield return new WaitForSeconds(2);
        if (Move8 != 0)
        {
            if (Move8 == 1)
            {
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos1 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos1 + "", 2);
            }
            if (Move8 == 2)
            {
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos2 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos2 + "", 2);
            }
            if (Move8 == 3)
            {
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos3 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos3 + "", 2);
            }
            if (Move8 == 4)
            {
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos4 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos4 + "", 2);
            }
            if (Move8 == 5)
            {
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos5 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos5 + "", 2);
            }
            if (Move8 == 6)
            {
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos6 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos6 + "", 2);
            }
            if (Move8 == 7)
            {
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos7 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos7 + "", 2);
            }
            if (Move8 == 8)
            {
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos8 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos9 + "", 2);
            }
            if (Move8 == 9)
            {
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos9 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers2.Pos9 + "", 2);
            }
        }
        yield return new WaitForSeconds(2);
        if (Move9 != 0)
        {
            if (Move9 == 1)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos1 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos1 + "", 2);
            }
            if (Move9 == 2)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos2 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos2 + "", 2);
            }
            if (Move9 == 3)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos3 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos3 + "", 2);
            }
            if (Move9 == 4)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos4 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos4 + "", 2);
            }
            if (Move9 == 5)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos5 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos5 + "", 2);
            }
            if (Move9 == 6)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos6 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos6 + "", 2);
            }
            if (Move9 == 7)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos7 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos7 + "", 2);
            }
            if (Move9 == 8)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos8 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos9 + "", 2);
            }
            if (Move9 == 9)
            {
                SendMessageToClient(ClientToServerMoveSignifiers.Pos9 + "", 1);
                SendMessageToClient(ClientToServerMoveSignifiers.Pos9 + "", 2);
            }
        }
    }
 
    public void Replay()
    {
        StartCoroutine(ExampleCoroutine());
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

    public static class ClientToServerGOSignifiers
    {
        public const int p1Won = 33;
        public const int p2Won = 34;
    }
    public static class ServerToClientGOSignifiers
    {
        public const int p1Won = 33;
        public const int p2Won = 34;
    }

    public static class ClientToServerRPSignifiers
    {
        public const int Replay = 35;
    }
    public static class ServerToClientRPSignifiers
    {
        public const int Replay = 35;
    }
}
