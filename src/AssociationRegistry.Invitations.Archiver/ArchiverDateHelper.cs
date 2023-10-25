namespace AssociationRegistry.Invitations.Archiver;

using System.Text.RegularExpressions;
using NodaTime;

public static class ArchiverDateHelper
{
    public static Instant CalculateArchivalStartDate(string archivalPeriod, Instant currentTime)
    {
        var duration = ParseArchivalPeriod(archivalPeriod);
        return currentTime - duration;
    }

    private static Duration ParseArchivalPeriod(string archivalPeriod)
    {
        if (string.IsNullOrEmpty(archivalPeriod))
            throw new ArgumentException("Input cannot be null or empty.", nameof(archivalPeriod));

        var match = Regex.Match(archivalPeriod, @"^(\d+)([smhdw])$");
        if (!match.Success)
            throw new FormatException("Invalid format.");

        var value = int.Parse(match.Groups[1].Value);
        var unit = match.Groups[2].Value[0];

        return unit switch {
            's' => Duration.FromSeconds(value),
            'm' => Duration.FromMinutes(value),
            'h' => Duration.FromHours(value),
            'd' => Duration.FromDays(value),
            'w' => Duration.FromDays(value * 7),
            _ => throw new FormatException("Invalid format."),
        };
    }
}