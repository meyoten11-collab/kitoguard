using System.Net.Sockets;
using SilkroadSecurityAPI;
using SilkroadSecurityAPI.Message;

namespace DuckSoup.Library.Server;

public interface IFakeSession
{
    ISecurity ClientSecurity { get; }
    Socket Socket { get; }
    bool IsConnected { get; }
    void Send(Packet packet, bool transfer = false);
    void Transfer();
    bool Disconnect();
}
