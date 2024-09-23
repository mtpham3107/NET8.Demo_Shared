﻿namespace NET8.Demo.GlobalAdmin;

public class AppSettings
{
    public string CorsOrigins { get; set; }
    public string SelfUrl { get; set; }
    public string LoginPath { get; set; }
    public string LogoutPath { get; set; }
    public Dictionary<string, string> ClientUrls { get; set; }
    public JwtOptions JwtOptions { get; set; }
    public string DefaultCulture { get; set; }
    public List<CultureInfo> SupportedCultures { get; set; }
}

public class JwtOptions
{
    public string Authority { get; set; }
    public string Audience { get; set; }
    public string Secret { get; set; }
}

public class CultureInfo
{
    public string Culture { get; set; }
    public string Name { get; set; }
    public string Flag { get; set; }
}