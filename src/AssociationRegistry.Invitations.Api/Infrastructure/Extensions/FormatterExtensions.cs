using AssociationRegistry.Invitations.Api.Infrastructure.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace AssociationRegistry.Invitations.Api.Infrastructure.Extensions;

public static class FormatterExtensions
{
    private static readonly DefaultContractResolver SharedContractResolver =
        DefaultApiJsonContractResolver.UsingDefaultNamingStrategy();

    /// <summary>
    /// Sets up and adds additional converters for an API to the JsonSerializerSettings
    /// </summary>
    /// <param name="source"></param>
    /// <returns>the updated JsonSerializerSettings</returns>
    public static JsonSerializerSettings ConfigureDefaultForApi(this JsonSerializerSettings source)
    {
        source.ContractResolver = SharedContractResolver;

        source.DateFormatHandling = DateFormatHandling.IsoDateFormat;
        source.DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;

        var stringEnumConvertor = new StringEnumConverter { CamelCaseText = true };
        stringEnumConvertor.NamingStrategy = new CamelCaseNamingStrategy(true, true);
        source.Converters.Add(stringEnumConvertor);

        // NOT NEEDED FOR NOW
        // source.Converters.Add(new TrimStringConverter());
        // source.Converters.Add(new Rfc3339SerializableDateTimeOffsetConverter());

        //
        // return source
        //     .ConfigureForNodaTime(DateTimeZoneProviders.Tzdb)
        //     .WithIsoIntervalConverter();
        return source;
    }
}