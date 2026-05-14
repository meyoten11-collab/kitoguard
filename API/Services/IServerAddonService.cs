using API.ServerAddon;
using Database.VSRO188.SRO_VT_SHARD;

namespace API.Services;

public interface IServerAddonService
{
    ServerAddonGameServerAction QueueAction(GameServerActionRequest request);

    List<ServerAddonGameServerAction> GetRecentActions(int limit);

    ServerAddonHealth GetHealth();

    bool EnsureActionTable();
}
