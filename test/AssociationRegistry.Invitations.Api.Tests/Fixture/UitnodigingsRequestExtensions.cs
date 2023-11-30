using Newtonsoft.Json.Linq;

namespace AssociationRegistry.Invitations.Api.Tests.Fixture;

public static class UitnodigingsRequestExtensions
{
    public static async Task<Guid> ParseIdFromUitnodigingResponse(this HttpResponseMessage message)
    {
        var content = await message.Content.ReadAsStringAsync();
        return Guid.Parse(JToken.Parse(content)["uitnodigingId"]!.Value<string>()!);
    }

    public static async Task<Guid> ParseIdFromAanvraagResponse(this HttpResponseMessage message)
    {
        var content = await message.Content.ReadAsStringAsync();
        return Guid.Parse(JToken.Parse(content)["aanvraagId"]!.Value<string>()!);
    }
}
