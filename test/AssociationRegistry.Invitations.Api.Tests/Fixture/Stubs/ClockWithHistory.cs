using NodaTime;
using SystemClock = NodaTime.SystemClock;

namespace AssociationRegistry.Invitations.Api.Tests.Fixture.Stubs;

public class ClockWithHistory : IClock
{
    public Instant PreviousInstant { get; private set; }

    public Instant GetCurrentInstant()
    {
        PreviousInstant = SystemClock.Instance.GetCurrentInstant();
        return PreviousInstant;
    }
}
