using Newtonsoft.Json.Linq;

namespace AssociationRegistry.Invitations.Archiver.Tests.Fixture;

public static class UitnodigingsRequestExtensions
{
    public static async Task<Guid> ParseIdFromContentString(this HttpResponseMessage message)
    {
        var content = await message.Content.ReadAsStringAsync();
        return Guid.Parse(JToken.Parse(content)["uitnodigingId"]!.Value<string>()!);
    }
}