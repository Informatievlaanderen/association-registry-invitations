public class ArchiverOptions
{
    public BewaartijdenOptions Bewaartijden { get; set; }
    
    public class BewaartijdenOptions
    {
        public string WachtOpAntwoord { get; set; }
        public string Aanvaard { get; set; }
        public string Geweigerd { get; set; }
        public string Ingetrokken { get; set; }
        public string Verlopen { get; set; }
    }
}