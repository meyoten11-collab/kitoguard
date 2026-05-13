using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using API;
using API.Database.DuckSoup;
using API.Enums;
using API.Server;
using API.ServiceFactory;
using API.Webserver;
using WatsonWebserver.Core;

namespace DuckSoup.Library.Webserver;

public class MonitoringRoutes
{
    private readonly IServerManager _serverManager;
    private readonly ISharedObjects _sharedObjects;

    public MonitoringRoutes(IWebserverManager webserverManager)
    {
        _serverManager = ServiceFactory.Load<IServerManager>(typeof(IServerManager));
        _sharedObjects = ServiceFactory.Load<ISharedObjects>(typeof(ISharedObjects));

        webserverManager.addStaticRoute(HttpMethod.GET, "/api/v1/monitoring/health", HealthRoute);
        webserverManager.addStaticRoute(HttpMethod.GET, "/api/v1/monitoring/summary", SummaryRoute);
        webserverManager.addStaticRoute(HttpMethod.GET, "/api/v1/monitoring/services", ServicesRoute);

        webserverManager.addProtectedPrefix("/api/v1/monitoring/health", new[]
        {
            UserRole.Anyone
        });
        webserverManager.addProtectedPrefix("/api/v1/monitoring/summary", new[]
        {
            UserRole.Authenticated
        });
        webserverManager.addProtectedPrefix("/api/v1/monitoring/services", new[]
        {
            UserRole.Authenticated
        });
    }

    private async Task HealthRoute(HttpContextBase ctx)
    {
        await SendJson(ctx, new
        {
            status = "ok",
            timestampUtc = DateTime.UtcNow
        });
    }

    private async Task SummaryRoute(HttpContextBase ctx)
    {
        await SendJson(ctx, new
        {
            timestampUtc = DateTime.UtcNow,
            services = _serverManager.Servers.Count,
            sessions = new
            {
                agent = _sharedObjects.AgentSessions.Count,
                gateway = _sharedObjects.GatewaySessions.Count,
                download = _sharedObjects.DownloadSessions.Count,
                total = _sharedObjects.AgentSessions.Count + _sharedObjects.GatewaySessions.Count + _sharedObjects.DownloadSessions.Count
            }
        });
    }

    private async Task ServicesRoute(HttpContextBase ctx)
    {
        List<ServiceStatus> services = _serverManager.Servers
            .Select(server => new ServiceStatus(server.Service))
            .ToList();

        await SendJson(ctx, new
        {
            timestampUtc = DateTime.UtcNow,
            services
        });
    }

    private static async Task SendJson(HttpContextBase ctx, object payload)
    {
        ctx.Response.ContentType = "application/json";
        ctx.Response.StatusCode = 200;
        await ctx.Response.Send(JsonSerializer.Serialize(payload));
    }

    private sealed class ServiceStatus
    {
        public ServiceStatus(Service service)
        {
            serviceId = service.ServiceId;
            name = service.Name;
            serverType = service.ServerType.ToString();
            securityType = service.SecurityType.ToString();
            bindPort = service.BindPort;
            remotePort = service.RemotePort;
            autoStart = service.AutoStart;
            localAddress = service.LocalMachine_Machine.Address;
            remoteAddress = service.RemoteMachine_Machine.Address;
            spoofAddress = service.SpoofMachine_Machine?.Address;
        }

        public int serviceId { get; }
        public string name { get; }
        public string serverType { get; }
        public string securityType { get; }
        public int bindPort { get; }
        public int remotePort { get; }
        public bool autoStart { get; }
        public string localAddress { get; }
        public string remoteAddress { get; }
        public string? spoofAddress { get; }
    }
}
