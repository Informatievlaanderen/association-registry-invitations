using System.Net.Http.Headers;
using AssociationRegistry.Invitations.Api.Constants;
using IdentityModel;
using IdentityModel.AspNetCore.OAuth2Introspection;
using IdentityModel.Client;

namespace AssociationRegistry.Invitations.Api.Tests.Fixture;

public class Clients : IDisposable
{
    private readonly Func<HttpClient> _createClientFunc;
    private readonly OAuth2IntrospectionOptions _oAuth2IntrospectionOptions;

    public Clients(OAuth2IntrospectionOptions oAuth2IntrospectionOptions, Func<HttpClient> createClientFunc)
    {
        _oAuth2IntrospectionOptions = oAuth2IntrospectionOptions;
        _createClientFunc = createClientFunc;
    }

    private HttpClient GetAuthenticatedHttpClient()
        => CreateMachine2MachineClientFor(clientId: _oAuth2IntrospectionOptions.ClientId, Security.Scopes.Uitnodigingen, clientSecret: _oAuth2IntrospectionOptions.ClientSecret).GetAwaiter().GetResult();

    public UitnodigingenApiClient Authenticated
        => new(GetAuthenticatedHttpClient());

    public UitnodigingenApiClient Unauthenticated
        => new(_createClientFunc());

    public UitnodigingenApiClient Unauthorized
        => new(CreateMachine2MachineClientFor(clientId: "vloketClient", scope: "vo_info", clientSecret: "secret").GetAwaiter().GetResult());

    public void Dispose()
    {
    }

    private async Task<HttpClient> CreateMachine2MachineClientFor(
        string clientId,
        string scope,
        string clientSecret)
    {
        var tokenClient = new TokenClient(
            client: () => new HttpClient(),
            new TokenClientOptions
            {
                Address = $"http://127.0.0.1:15051/connect/token",
                ClientId = "vloketClient",
                ClientSecret = "secret",
                Parameters = new Parameters(
                    new[]
                    {
                        new KeyValuePair<string, string>(key: "scope", scope),
                    }),
            });

        var acmResponse = await tokenClient.RequestTokenAsync(OidcConstants.GrantTypes.ClientCredentials);

        var token = acmResponse.AccessToken;
        var httpClientFor = _createClientFunc();
        httpClientFor.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClientFor.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Bearer", token);

        return httpClientFor;
    }
}
