namespace AssociationRegistry.Invitations;

using Newtonsoft.Json;
using System.Collections.Immutable;

public record AanvraagStatus
{
    public static readonly AanvraagStatus WachtOpAntwoord = new("Wacht op antwoord");
    public static readonly AanvraagStatus Aanvaard = new("Aanvaard");
    public static readonly AanvraagStatus Geweigerd = new("Geweigerd");
    public static readonly AanvraagStatus Ingetrokken = new("Ingetrokken");
    public static readonly AanvraagStatus Verlopen = new("Verlopen");

    public static readonly ImmutableArray<AanvraagStatus> All = new[] { WachtOpAntwoord, Aanvaard, Geweigerd, Ingetrokken, Verlopen }
       .ToImmutableArray();

    [JsonConstructor]
    private AanvraagStatus(string status)
    {
        Status = status;
    }

    public string Status { get; }
    public static AanvraagStatus Parse(string status) => All.Single(s => s.Status == status);

    public static bool TryParse(string status, out AanvraagStatus? aanvraagStatus)
    {
        aanvraagStatus = All.SingleOrDefault(u => u.Status == status);

        return aanvraagStatus is not null;
    }

    public static bool CanParse(string status) => All.Any(s => s.Status == status);
    public static implicit operator string(AanvraagStatus status) => status.Status;
}