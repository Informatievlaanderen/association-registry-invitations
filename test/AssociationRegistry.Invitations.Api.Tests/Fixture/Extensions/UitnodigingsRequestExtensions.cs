namespace AssociationRegistry.Invitations.Api.Tests.Fixture.Extensions;

using Newtonsoft.Json.Linq;

public static class UitnodigingsRequestExtensions
{
    public static async Task<Guid> ParseIdFromUitnodigingResponse(this HttpResponseMessage message)
    {
        var content = await message.Content.ReadAsStringAsync();
        return Guid.Parse(JToken.Parse(content)["uitnodigingId"]!.Value<string>()!);
    }
}