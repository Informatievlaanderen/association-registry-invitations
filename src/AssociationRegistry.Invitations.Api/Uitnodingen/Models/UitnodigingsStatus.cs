namespace AssociationRegistry.Invitations.Api.Uitnodingen.Models;

public class UitnodigingsStatus : IEquatable<UitnodigingsStatus>
{
    public static UitnodigingsStatus WachtenOpAntwoord = new("Wachten op antwoord.");
    public static UitnodigingsStatus Aanvaard = new("Aanvaard.");
    public static UitnodigingsStatus[] All = { WachtenOpAntwoord, Aanvaard };

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

    public bool Equals(UitnodigingsStatus? other) => Status == other?.Status;

    public static bool operator ==(UitnodigingsStatus status1, UitnodigingsStatus status2) => status1.Equals(status2);

    public static bool operator !=(UitnodigingsStatus status1, UitnodigingsStatus status2) => !(status1 == status2);

    public static implicit operator string(UitnodigingsStatus status) => status.Status;
}
