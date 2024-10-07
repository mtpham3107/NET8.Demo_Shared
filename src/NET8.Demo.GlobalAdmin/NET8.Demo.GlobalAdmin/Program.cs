using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.OpenApi.Models;
using NET8.Demo.GlobalAdmin;
using NET8.Demo.GlobalAdmin.Application.Services;
using NET8.Demo.GlobalAdmin.Application.SignalRHubs;
using NET8.Demo.GlobalAdmin.Core.DbContexts;
using NET8.Demo.GlobalAdmin.Domain.Entities;
using NET8.Demo.Shared;

var builder = WebApplication.CreateBuilder(args);
var appSettings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = appSettings.SupportedCultures.Select(x => x.Culture).ToArray();

    options.AddSupportedCultures(supportedCultures)
           .AddSupportedUICultures(supportedCultures)
           .SetDefaultCulture(appSettings.DefaultCulture)
           .AddInitialRequestCultureProvider(new CookieRequestCultureProvider());
});

builder.Services.AddDbContext<GlobalAdminDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddIdentity<User, Role>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;

    options.Password.RequiredLength = 9;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireDigit = true;

    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;

}).AddEntityFrameworkStores<GlobalAdminDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication()
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = appSettings.LoginPath;
        options.LogoutPath = appSettings.LogoutPath;
    })
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.Authority = appSettings.JwtOptions.Authority;
        options.Audience = appSettings.JwtOptions.Audience;
    });

//var signingCredentials = () =>
//{
//    using var rsa = RSA.Create(2048);
//    var rsaKey = new RsaSecurityKey(rsa);
//    return new SigningCredentials(rsaKey, SecurityAlgorithms.RsaSha256);
//};

builder.Services.AddIdentityServer()
    .AddInMemoryIdentityResources(IdentityServerConfig.IdentityResources)
    .AddInMemoryApiResources(IdentityServerConfig.ApiResources)
    .AddInMemoryApiScopes(IdentityServerConfig.ApiScopes)
    .AddInMemoryClients(IdentityServerConfig.Clients(appSettings.ClientUrls))
    .AddAspNetIdentity<User>()
    .AddProfileService<ProfileService>()
    .AddDeveloperSigningCredential();
//.AddSigningCredential(signingCredentials());


builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(1);
});

builder.Services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.ConfigureCore();
builder.Services.ConfigureServices();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddControllersWithViews()
                .AddDataAnnotationsLocalization()
                .AddViewLocalization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();
builder.Services.AddSwaggerGen(options =>
{
    options.DocInclusionPredicate((docName, apiDesc) =>
    {
        // Only include controllers that are API controllers
        return apiDesc.ActionDescriptor.EndpointMetadata.OfType<ApiControllerAttribute>().Any();
    });
    options.SwaggerDoc("v1", new OpenApiInfo { Title = $"API - {builder.Environment.EnvironmentName}", Version = "v1" });

    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(b =>
    {
        var allowedOrigins = appSettings.CorsOrigins
            .Split(",", StringSplitOptions.RemoveEmptyEntries)
            .Select(o => o.TrimEnd('/'))
            .ToArray();

        b.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowedToAllowWildcardSubdomains();
    });
});

builder.Services.Configure<MvcOptions>(o => o.Filters.Add<ErrorHandlingMiddleware>());

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStatusCodePagesWithReExecute("/Error/{0}");
app.UseRequestLocalization();
app.UseCors();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseIdentityServer();
app.MapHub<NotificationHub>("/notificationHub");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
await app.ApplyMigration();
app.Run();

