namespace AssociationRegistry.Invitations.Tests.Fixture;

public class UitnodigingenApiClient : IDisposable
{
    private readonly HttpClient _httpClient;

    public UitnodigingenApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> GetRoot() => await _httpClient.GetAsync("/");

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
