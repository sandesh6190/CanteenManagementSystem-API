namespace Mart.Data.Configurations;

public static class CanteenConfig
{
    public static JwtSettings? JwtSettings { get; set; } = new();
    public static ConnectionStrings? ConnectionStrings { get; set; } = new();
    public static SmtpSettings? SmtpSettings { get; set; } = new();
}

public class JwtSettings
{
    public string? Secret { get; init; }
    public string? Issuer { get; init; }
    public string? Audience { get; init; }
    public int ExpiryMinutes { get; init; }
}

public class SmtpSettings
{
    public string? Host { get; init; }
    public int Port { get; init; }
    public string? Name { get; init; }
    public string? Email { get; init; }
    public string? Password { get; init; }
}

public class ConnectionStrings
{
    public string? DefaultConnection { get; init; }
}