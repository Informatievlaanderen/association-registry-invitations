using FluentAssertions;
using NodaTime;

namespace AssociationRegistry.Invitations.Archiver.Tests;

public class ArchivalDateHelperTests
{
    [Fact]
    public void Parse_ValidInput_ReturnsExpectedTimeSpan()
    {
        var date = Instant.FromUtc(2023, 1, 1, 0, 0, 0);
        
        ArchivalDateHelper.CalculateArchivalStartDate("10w", date).Should().Be(Instant.FromUtc(2022, 10, 23, 0, 0, 0));
        ArchivalDateHelper.CalculateArchivalStartDate("10d", date).Should().Be(Instant.FromUtc(2022, 12, 22, 0, 0, 0));
        ArchivalDateHelper.CalculateArchivalStartDate("10h", date).Should().Be(Instant.FromUtc(2022, 12, 31, 14, 0, 0));
        ArchivalDateHelper.CalculateArchivalStartDate("10m", date).Should().Be(Instant.FromUtc(2022, 12, 31, 23, 50, 0));
        ArchivalDateHelper.CalculateArchivalStartDate("10s", date).Should().Be(Instant.FromUtc(2022, 12, 31, 23, 59, 50));
    }
}