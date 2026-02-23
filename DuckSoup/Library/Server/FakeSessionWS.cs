using System;
using System.Collections.Generic;
using System.Net.Sockets;
using API.Database.DuckSoup;
using API.EventFactory;
using API.Session;
using DuckSoup.Library.Session;
using NetCoreServer;
using PacketLibrary.Handler;
using Serilog;
using SilkroadSecurityAPI;
using SilkroadSecurityAPI.Exceptions;
using SilkroadSecurityAPI.Message;

namespace DuckSoup.Library.Server;

/// <summary>
///     Basically the connection to the Client, the proxy spawns a server <see cref="FakeServerWS" /> that accepts
///     WebSocket connections.
///     After a successfully established connection it will spawn a Session, this.
/// </summary>
public class FakeSessionWS : WsSession, IFakeSession
{
    public FakeSessionWS(FakeServerWS server, Service service) : base(server)
    {
        FakeServerWS = server;
        try
        {
            ClientSecurity = Utility.GetSecurity(service.SecurityType);
            ClientSecurity.GenerateSecurity(true, true, true);

            FakeClient fakeRemoteClient = new FakeClient(server, service);
            fakeRemoteClient.ConnectAsync();

            Session = new DuckSession(this, fakeRemoteClient);
            fakeRemoteClient.Session = Session;
        }
        catch (Exception exception)
        {
            Log.Error("FakeSessionWS | Could not establish remote client");
            Log.Error("FakeSessionWS | {0}", exception.Message);
            Log.Error("FakeSessionWS | {0}", exception.StackTrace);
            Log.Error("FakeSessionWS | {0}", exception.InnerException);
            Log.Error("FakeSessionWS | {0}", exception.Data);
            base.Disconnect();
        }

    }

    public ISession? Session { get; }
    public ISecurity ClientSecurity { get; }
    private FakeServerWS FakeServerWS { get; }

    public override void OnWsConnected(HttpRequest request)
    {
        if (Session == null)
        {
            return;
        }

        Log.Debug($"FakeSessionWS connected with Id {Id} connected!");
        FakeServerWS.AddSession(Session);
    }

    public override void OnWsDisconnected()
    {
        if (Session == null)
        {
            return;
        }

        Session.Disconnect();
        FakeServerWS.RemoveSession(Session);
        Log.Debug($"FakeSessionWS disconnected with Id {Id} disconnected!");
    }

    protected override void OnError(SocketError error)
    {
        if (Session == null)
        {
            return;
        }

        Session.Disconnect();
        Console.WriteLine($"FakeSessionWS caught an error with code {error}");
    }

    // Receive from Client
    // C -> P -> S
    public override void OnWsReceived(byte[] buffer, long offset, long size)
    {
        if (Session == null)
        {
            return;
        }

        Session.GetData(Data.CrcFailure, out int crc, 0);
        if (crc > 5)
        {
            Session.Disconnect();
            return;
        }

        string message = string.Empty;
        try
        {
            ClientSecurity.Recv(buffer, (int)offset, (int)size);

            List<Packet>? receivedPackets = ClientSecurity.TransferIncoming();

            if (receivedPackets == null || receivedPackets.Count == 0) return;

            foreach (Packet packet in receivedPackets)
            {
                string packetType = packet.Encrypted ? "[E]" : packet.Massive ? "[M]" : "";
                message = $"[C -> P] {packetType} Packet: 0x{packet.MsgId:X} - {Id}";
                Log.Verbose(message);

                if (EventFactory.HasSubscriptions(EventFactoryNames.OnClientReceivePacket))
                {
                    EventFactory.Publish(EventFactoryNames.OnClientReceivePacket, DateTime.Now, FakeServerWS.Service.ServerType, Session, new Packet(packet));
                }

                if (packet.MsgId == 0x5000 || packet.MsgId == 0x9000 || packet.MsgId == 0x2001) continue;

                Packet packetResult = FakeServerWS.PacketHandler.HandleClient(packet, Session).Result;

                switch (packetResult.ResultType)
                {
                    case PacketResultType.Block:
                        // TODO :: Temporary for testing purp.
                        // Session.SendToServer(packetResult);
                        Log.Debug($"Client Packet: 0x{packet.MsgId:X} is perhaps not on whitelist!");
                        break;
                    case PacketResultType.Disconnect:
                        // Console.WriteLine($"Packet: 0x{packet.MsgId:X} is on blacklist!");
                        Session.Disconnect();
                        return;
                    case PacketResultType.Nothing:
                        Session.SendToServer(packetResult);
                        break;
                    default:
                        Log.Error("FakeSessionWS - Unknown ResultType");
                        break;
                }
            }

            Session.SetData(Data.CrcFailure, 0);
            Session.TransferToServer();
        }
        catch (RecvException)
        {
            Session.SetData(Data.CrcFailure, crc + 1);
        }
        catch (Exception exception)
        {
            Session.GetData(Data.CharInfo, out ICharInfo? charInfo, null);
            Session.GetData(Data.CharId, out int charId, -1);
            Log.Error("FakeSessionWS Recv | 0x{0:X} | Name: {1} | Id: {2} | ServerType: {3} ", message, charInfo != null ? charInfo.CharName : "null", charId, FakeServerWS.Service.ServerType);
            Log.Error("FakeSessionWS Recv | SSAClientId: {1} | Current: {2} | Last: {3} ", Session.GetServerSecurity().GetId(), Session.GetServerSecurity().GetCurrentLockState(), Session.GetServerSecurity().GetLastLockState());
            Log.Error("FakeSessionWS Recv | SSAServerId: {1} | Current: {2} | Last: {3} ", Session.GetClientSecurity().GetId(), Session.GetClientSecurity().GetCurrentLockState(), Session.GetClientSecurity().GetLastLockState());
            Log.Error("FakeSessionWS Recv | {0}", exception.Message);
            Log.Error("FakeSessionWS Recv | {0}", exception.StackTrace);
            Log.Error("FakeSessionWS Recv | {0}", exception.InnerException);
            Log.Error("FakeSessionWS Recv | {0}", exception.Data);
            Session.Disconnect();
        }
    }

    public void Send(Packet packet, bool transfer = false)
    {
        try
        {
            ClientSecurity.Send(packet);

            if (EventFactory.HasSubscriptions(EventFactoryNames.OnClientTransferPacket))
            {
                EventFactory.Publish(EventFactoryNames.OnClientTransferPacket, DateTime.Now, FakeServerWS.Service.ServerType, Session, new Packet(packet));
            }

            if (transfer) Transfer();
        }
        catch (Exception exception)
        {
            Log.Error("FakeSessionWS:166 | ID: {0} ", Id);
            Log.Error("FakeSessionWS:166 | {0}", exception.Message);
            Log.Error("FakeSessionWS:166 | {0}", exception.StackTrace);
            Log.Error("FakeSessionWS:166 | {0}", exception.InnerException);
            Log.Error("FakeSessionWS:166 | {0}", exception.Data);
            Disconnect();
        }
    }

    public void Transfer()
    {
        try
        {
            // It expects TcpSession - wait, WsSession inherits from TcpSession so 'this' works.
            ClientSecurity.TransferOutgoing(this);
        }
        catch (Exception exception)
        {
            Log.Error("FakeSessionWS:181 | ID: {0} ", Id);
            Log.Error("FakeSessionWS:181 | {0}", exception.Message);
            Log.Error("FakeSessionWS:181 | {0}", exception.StackTrace);
            Log.Error("FakeSessionWS:181 | {0}", exception.InnerException);
            Log.Error("FakeSessionWS:181 | {0}", exception.Data);
            Disconnect();
        }
    }
}
