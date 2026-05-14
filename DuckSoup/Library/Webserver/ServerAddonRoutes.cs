using System;
using System.Threading.Tasks;
using API.Enums;
using API.ServerAddon;
using API.ServiceFactory;
using API.Services;
using API.Webserver;
using Database.VSRO188.SRO_VT_SHARD;
using Newtonsoft.Json;
using WatsonWebserver.Core;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace DuckSoup.Library.Webserver;

public class ServerAddonRoutes
{
    private readonly IServerAddonService _serverAddonService;

    public ServerAddonRoutes(IWebserverManager webserverManager)
    {
        _serverAddonService = ServiceFactory.Load<IServerAddonService>(typeof(IServerAddonService));

        webserverManager.addStaticRoute(HttpMethod.GET, "/api/v1/serveraddon/health", HealthRoute);
        webserverManager.addStaticRoute(HttpMethod.GET, "/api/v1/serveraddon/actions", ActionsRoute);
        webserverManager.addStaticRoute(HttpMethod.GET, "/api/v1/serveraddon/actions/recent", RecentRoute);
        webserverManager.addStaticRoute(HttpMethod.POST, "/api/v1/serveraddon/actions", QueueRoute);
        webserverManager.addStaticRoute(HttpMethod.POST, "/api/v1/serveraddon/job-equipment", JobEquipmentRoute);
        webserverManager.addStaticRoute(HttpMethod.GET, "/api/v1/serveraddon/servers", ServersRoute);

        webserverManager.addProtectedPrefix("/api/v1/serveraddon", new[]
        {
            UserRole.SuperAdmin,
            UserRole.Owner,
            UserRole.Admin
        });
    }

    private async Task HealthRoute(HttpContextBase ctx)
    {
        ctx.Response.ContentType = "application/json";

        ServerAddonHealth health = _serverAddonService.GetHealth();
        ctx.Response.StatusCode = health.TableAvailable ? 200 : 503;
        await ctx.Response.Send(JsonSerializer.Serialize(health));
    }

    private async Task RecentRoute(HttpContextBase ctx)
    {
        ctx.Response.ContentType = "application/json";

        int limit = 20;
        string? limitQuery = ctx.Request.Query.Elements["limit"];
        if (limitQuery != null)
            int.TryParse(limitQuery, out limit);

        ctx.Response.StatusCode = 200;
        await ctx.Response.Send(JsonSerializer.Serialize(_serverAddonService.GetRecentActions(limit)));
    }

    private async Task ActionsRoute(HttpContextBase ctx)
    {
        ctx.Response.ContentType = "application/json";
        ctx.Response.StatusCode = 200;
        await ctx.Response.Send(JsonSerializer.Serialize(_serverAddonService.GetSupportedActions()));
    }

    private async Task ServersRoute(HttpContextBase ctx)
    {
        ctx.Response.ContentType = "application/json";
        ctx.Response.StatusCode = 200;
        await ctx.Response.Send(JsonSerializer.Serialize(_serverAddonService.GetConfiguredServers()));
    }

    private async Task QueueRoute(HttpContextBase ctx)
    {
        ctx.Response.ContentType = "application/json";

        GameServerActionRequest? request;
        try
        {
            request = JsonConvert.DeserializeObject<GameServerActionRequest>(ctx.Request.DataAsString);
        }
        catch (Exception)
        {
            ctx.Response.StatusCode = 400;
            await ctx.Response.Send();
            return;
        }

        if (request == null)
        {
            ctx.Response.StatusCode = 400;
            await ctx.Response.Send();
            return;
        }

        try
        {
            ServerAddonGameServerAction action = _serverAddonService.QueueAction(request);
            ctx.Response.StatusCode = 201;
            await ctx.Response.Send(JsonSerializer.Serialize(action));
        }
        catch (ArgumentException exception)
        {
            ctx.Response.StatusCode = 400;
            await ctx.Response.Send(JsonSerializer.Serialize(new
            {
                error = exception.Message
            }));
        }
    }

    private async Task JobEquipmentRoute(HttpContextBase ctx)
    {
        ctx.Response.ContentType = "application/json";

        JobEquipmentGrantRequest? request;
        try
        {
            request = JsonConvert.DeserializeObject<JobEquipmentGrantRequest>(ctx.Request.DataAsString);
        }
        catch (Exception)
        {
            ctx.Response.StatusCode = 400;
            await ctx.Response.Send();
            return;
        }

        if (request == null)
        {
            ctx.Response.StatusCode = 400;
            await ctx.Response.Send();
            return;
        }

        try
        {
            ServerAddonGameServerAction action = _serverAddonService.QueueJobEquipmentGrant(request);
            ctx.Response.StatusCode = 201;
            await ctx.Response.Send(JsonSerializer.Serialize(action));
        }
        catch (ArgumentException exception)
        {
            ctx.Response.StatusCode = 400;
            await ctx.Response.Send(JsonSerializer.Serialize(new
            {
                error = exception.Message
            }));
        }
    }
}
