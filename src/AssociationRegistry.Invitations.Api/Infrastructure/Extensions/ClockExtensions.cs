using System.Globalization;
using NodaTime;

namespace AssociationRegistry.Invitations.Api.Infrastructure.Extensions;

public static class ClockExtensions
{
    public static string AsFormattedString(this Instant instant) => instant.ToString("g", CultureInfo.InvariantCulture);
}