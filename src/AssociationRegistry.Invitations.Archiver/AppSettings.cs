namespace AssociationRegistry.Invitations.Archiver;

public class AppSettings
{
    public BewaartijdenOptions Bewaartijden { get; set; } = new();
    
    public class BewaartijdenOptions
    {
        public BewaartijdenCategorieOptions Uitnodigingen { get; set; }
        public BewaartijdenCategorieOptions Aanvragen { get; set; }
    }

    public class BewaartijdenCategorieOptions
    {
        public string WachtOpAntwoord { get; set; } = ""; 
        public string Aanvaard { get; set; } = "";
        public string Geweigerd { get; set; } = "";
        public string Ingetrokken { get; set; } = "";
        public string Verlopen { get; set; } = "";
    }
}