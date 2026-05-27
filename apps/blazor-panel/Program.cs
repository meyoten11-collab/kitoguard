using KitoGuard.WebPanel.Services;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
});

builder.Host.UseSerilog();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddSingleton<DatabaseService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<SettingsService>();
builder.Services.AddScoped<ProxyService>();
builder.Services.AddScoped<PlayerService>();
builder.Services.AddScoped<GuildService>();
builder.Services.AddScoped<AchievementService>();
builder.Services.AddScoped<AttendanceService>();
builder.Services.AddScoped<EventService>();
builder.Services.AddScoped<ItemMallService>();
builder.Services.AddScoped<LuckySpinService>();
builder.Services.AddScoped<ItemChestService>();
builder.Services.AddScoped<RankService>();
builder.Services.AddScoped<SchedulerService>();
builder.Services.AddScoped<NoticeService>();
builder.Services.AddScoped<GmCommandService>();
builder.Services.AddScoped<DbBrowserService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

var port = builder.Configuration.GetValue<int>("KitoGuard:WebPanelPort", 8889);
app.Urls.Clear();
app.Urls.Add($"http://0.0.0.0:{port}");

Log.Information("KitoGuard-S500 WebPanel starting on port {Port}", port);
app.Run();
