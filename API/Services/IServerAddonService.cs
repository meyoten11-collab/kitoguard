using API.ServerAddon;
using Database.VSRO188.SRO_VT_SHARD;

namespace API.Services;

public interface IServerAddonService
{
    ServerAddonGameServerAction QueueAction(GameServerActionRequest request);

    ServerAddonGameServerAction QueueJobEquipmentGrant(JobEquipmentGrantRequest request);

    List<ServerAddonGameServerAction> GetRecentActions(int limit);

    List<ServerAddonActionSummary> GetSupportedActions();

    List<ServerAddonServerSummary> GetConfiguredServers();

    ServerAddonHealth GetHealth();

    bool EnsureActionTable();
}
