namespace AssociationRegistry.Invitations.Api.Uitnodingen.Responses;

public class UitnodigingenResponse
{
    public Uitnodiging[] Uitnodigingen { get; set; }
}

public class Uitnodiging
{
    public Guid Id { get; set; }
    public string VCode { get; set; }
    public string Boodschap { get; set; }
    public Uitnodiger Uitnodiger { get; set; }
    public Uitgenodigde Uitgenodigde { get; set; }
}

public class Uitnodiger
{
    public int VertegenwoordigerId { get; set; }
}

public class Uitgenodigde
{
    public string Insz { get; set; }
    public string Voornaam { get; set; }
    public string Naam { get; set; }
}
