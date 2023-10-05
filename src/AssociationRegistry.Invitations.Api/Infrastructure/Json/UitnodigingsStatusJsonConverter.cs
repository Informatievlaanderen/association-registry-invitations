using AssociationRegistry.Invitations.Api.Uitnodigingen.Models;
using Newtonsoft.Json;

namespace AssociationRegistry.Invitations.Api.Infrastructure.Json;

public class UitnodigingsStatusJsonConverter : JsonConverter<UitnodigingsStatus>
{
    public override void WriteJson(JsonWriter writer, UitnodigingsStatus? value, JsonSerializer serializer) =>
        writer.WriteValue(value?.Status);

    public override UitnodigingsStatus? ReadJson(JsonReader reader, Type objectType, UitnodigingsStatus? existingValue,
        bool hasExistingValue, JsonSerializer serializer) =>
        UitnodigingsStatus.Parse((string)reader.Value);
}
