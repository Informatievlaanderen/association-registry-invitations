namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Models;

public record UitnodigingsStatus
{
    public static UitnodigingsStatus WachtenOpAntwoord = new("Wachten op antwoord");
    public static UitnodigingsStatus Aanvaard = new("Aanvaard");
    public static UitnodigingsStatus Geweigerd = new("Geweigerd");
    public static UitnodigingsStatus[] All = { WachtenOpAntwoord, Aanvaard, Geweigerd };

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
