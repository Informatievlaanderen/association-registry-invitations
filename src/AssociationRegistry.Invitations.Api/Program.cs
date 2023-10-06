using System.Net;
using AssociationRegistry.Invitations.Api.Constants;
using AssociationRegistry.Invitations.Api.Infrastructure.Extentions;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Models;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using NodaTime;

var builder = WebApplication.CreateBuilder(args);

var postgreSqlOptions = builder.Configuration.GetPostgreSqlOptionsSection();


// Add services to the container.
builder.Services
    .AddSingleton<IClock>(SystemClock.Instance)
    .AddTransient<UitnodigingsStatusHandler>();

ConfigureKestrel(builder);

builder.Services.AddMarten(postgreSqlOptions);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddOAuth2Introspection(
        JwtBearerDefaults.AuthenticationScheme,
        configureOptions: options =>
        {
            var configOptions = builder.Configuration.GetSection(nameof(OAuth2IntrospectionOptions))
                .Get<OAuth2IntrospectionOptions>()!;

            options.ClientId = configOptions.ClientId;
            options.ClientSecret = configOptions.ClientSecret;
            options.Authority = configOptions.Authority;
            options.IntrospectionEndpoint = configOptions.IntrospectionEndpoint;
        }
    );

// builder.Services.AddAuthorization(
//     options =>
//         options.FallbackPolicy = new AuthorizationPolicyBuilder()
//             .RequireClaim(Security.ClaimTypes.Scope, Security.Scopes.Uitnodigingen)
//             .Build());

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static void ConfigureKestrel(WebApplicationBuilder builder)
{
    builder.WebHost.ConfigureKestrel(
        options =>
        {
            options.AddServerHeader = false;

            options.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(value: 120);

            options.Listen(
                new IPEndPoint(IPAddress.Any, port: 11009),
                configure: listenOptions =>
                {
                    listenOptions.UseConnectionLogging();
                    listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                });
        });
}


namespace AssociationRegistry.Invitations.Api
{
    public partial class Program
    {
    }
}
