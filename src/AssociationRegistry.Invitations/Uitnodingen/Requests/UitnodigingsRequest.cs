namespace AssociationRegistry.Invitations.Uitnodingen.Requests;

public class UitnodigingsRequest
{
    public string VCode { get; set; }
    public string Boodschap { get; set; }
    public Uitnodiger Uitnodiger { get; set; }
    public Uitgenodigde Uitgenodigde { get; set; }
}

public class Uitnodiger
{
    public string Insz { get; set; }
}


public class Uitgenodigde
{
    public string Insz { get; set; }
    public string Voornaam { get; set; }
    public string Naam { get; set; }
}