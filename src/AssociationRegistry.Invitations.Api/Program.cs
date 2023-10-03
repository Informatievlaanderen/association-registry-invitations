using AssociationRegistry.Invitations.Api.Constants;
using AssociationRegistry.Invitations.Api.Infrastructure.Extentions;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

var postgreSqlOptions = builder.Configuration.GetPostgreSqlOptionsSection();


// Add services to the container.

builder.Services.AddMarten(postgreSqlOptions);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
builder.Services.AddAuthorization(
    options =>
        options.FallbackPolicy = new AuthorizationPolicyBuilder()
            .RequireClaim(Security.ClaimTypes.Scope, Security.Scopes.Uitnodigingen)
            .Build());

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

public partial class Program
{
}
