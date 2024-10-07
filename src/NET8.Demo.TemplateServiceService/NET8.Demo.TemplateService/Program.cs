using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.OpenApi.Models;
using NET8.Demo.RemoteServices;
using NET8.Demo.Shared;
using NET8.Demo.TemplateService;
using NET8.Demo.TemplateService.Core.DbContexts;

var builder = WebApplication.CreateBuilder(args);
var appSettings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>();

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = appSettings.SupportedCultures.Select(x => x.Culture).ToArray();

    options.AddSupportedCultures(supportedCultures)
           .AddSupportedUICultures(supportedCultures)
           .SetDefaultCulture(appSettings.DefaultCulture)
           .AddInitialRequestCultureProvider(new CookieRequestCultureProvider());
});

builder.Services.AddDbContext<TemplateServiceDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.Authority = appSettings.JwtOptions.Authority;
    options.Audience = appSettings.JwtOptions.Audience;
});

builder.Services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.ConfigureCore();
builder.Services.ConfigureServices();
builder.Services.ConfigureRemoteService();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddControllersWithViews()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization();
builder.Services.AddEndpointsApiExplorer();
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

app.UseRequestLocalization();
app.UseCors();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => "API is working");
app.ApplyMigration();
app.Run();
