using AssociationRegistry.Invitations.Constants;
using AssociationRegistry.Invitations.Infrastructure.Extentions;
using AssociationRegistry.Invitations.Uitnodingen.Mapping;
using AssociationRegistry.Invitations.Uitnodingen.Querries;
using AssociationRegistry.Invitations.Uitnodingen.Requests;
using AssociationRegistry.Invitations.Uitnodingen.Responses;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Marten;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

const string GlobalPolicyName = "Global";
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

app.MapGet("/", Results.NoContent);
app.MapGet("/uitnodigingen", async ([FromQuery] string vcode, IDocumentStore store) =>
{
    var uitnodigingen = await GetUitnodigingen.MetVCode(vcode).ExecuteAsync(store);
    return Results.Ok(new UitnodigingenResponse()
    {
        Uitnodigingen = uitnodigingen.Select(UitnodigingsMapper.ToResponse).ToArray(),
    });
});
app.MapPost("/uitnodigingen",
    async ([FromBody] UitnodigingsRequest request, IDocumentStore store, CancellationToken cancellationToken) =>
    {
        var lightweightSession = store.LightweightSession();
        var uitnodiging = request.ToModel();
        lightweightSession.Store(uitnodiging);
        await lightweightSession.SaveChangesAsync(cancellationToken);
        return Results.Created("uitnodigingen/0", new RegistratieResponse()
        {
            Id = uitnodiging.Id,
        });
    });

app.Run();
