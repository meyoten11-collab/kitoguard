using KitoGuard.WebPanel.Models;

namespace KitoGuard.WebPanel.Services;

public class AuthService
{
    private readonly string _adminUser;
    private readonly string _adminPass;
    private readonly int _sessionTimeout;

    public bool IsAuthenticated { get; private set; }
    public AdminUser? CurrentUser { get; private set; }

    public AuthService(IConfiguration configuration)
    {
        _adminUser = configuration["Auth:AdminUser"] ?? "Admin";
        _adminPass = configuration["Auth:AdminPass"] ?? "1071998k";
        _sessionTimeout = int.Parse(configuration["Auth:SessionTimeoutMinutes"] ?? "480");
    }

    public bool Login(string username, string password)
    {
        if (username == _adminUser && password == _adminPass)
        {
            IsAuthenticated = true;
            CurrentUser = new AdminUser
            {
                Username = username,
                Role = "Admin",
                LoginTime = DateTime.UtcNow
            };
            return true;
        }
        return false;
    }

    public void Logout()
    {
        IsAuthenticated = false;
        CurrentUser = null;
    }

    public bool IsSessionValid()
    {
        if (!IsAuthenticated || CurrentUser == null) return false;
        return (DateTime.UtcNow - CurrentUser.LoginTime).TotalMinutes < _sessionTimeout;
    }
}
