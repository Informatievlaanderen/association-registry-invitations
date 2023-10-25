using Newtonsoft.Json;

namespace AssociationRegistry.Invitations;

using System.Collections.Immutable;

public record UitnodigingsStatus
{
    public static readonly UitnodigingsStatus WachtOpAntwoord = new("Wacht op antwoord");
    public static readonly UitnodigingsStatus Aanvaard = new("Aanvaard");
    public static readonly UitnodigingsStatus Geweigerd = new("Geweigerd");
    public static readonly UitnodigingsStatus Ingetrokken = new("Ingetrokken");
    public static readonly UitnodigingsStatus Verlopen = new("Verlopen");
    public static readonly ImmutableArray<UitnodigingsStatus> All = new[]{ WachtOpAntwoord, Aanvaard, Geweigerd, Ingetrokken, Verlopen }
       .ToImmutableArray();

    [JsonConstructor]
    private UitnodigingsStatus(string status)
    {
        Status = status;
    }

    public string Status { get; }

    public static UitnodigingsStatus Parse(string status) => All.Single(s => s.Status == status);

    public static bool TryParse(string status, out UitnodigingsStatus? uitnodigingsStatus)
    {
        uitnodigingsStatus = All.SingleOrDefault(u => u.Status == status);
        return uitnodigingsStatus is not null;
    }

    public static bool CanParse(string status) => All.Any(s => s.Status == status);

    public static implicit operator string(UitnodigingsStatus status) => status.Status;
}
