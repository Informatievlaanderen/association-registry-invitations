namespace AssociationRegistry.Invitations.Api.Tests.Fixture.Extensions;

using Newtonsoft.Json.Linq;

public static class AanvraagRequestExtensions
{
    public static async Task<Guid> ParseIdFromAanvraagResponse(this HttpResponseMessage message)
    {
        var content = await message.Content.ReadAsStringAsync();
        return Guid.Parse(JToken.Parse(content)["aanvraagId"]!.Value<string>()!);
    }
}