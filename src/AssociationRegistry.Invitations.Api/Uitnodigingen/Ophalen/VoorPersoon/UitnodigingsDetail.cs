﻿using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Ophalen.VoorPersoon;

public class UitnodigingsDetail
{
    public Guid UitnodigingId { get; set; }
    public string VCode { get; set; }
    public string Boodschap { get; set; }
    public string Status { get; set; }
    public string DatumRegistratie { get; set; }
    
    public string DatumLaatsteAanpassing { get; set; }
    public UitnodigerDetail Uitnodiger { get; set; }
    public UitgenodigdeDetail Uitgenodigde { get; set; }


    public class UitnodigerDetail
    {
        public int VertegenwoordigerId { get; set; }
    }

    public class UitgenodigdeDetail
    {
        public string Insz { get; set; }
        public string Voornaam { get; set; }
        public string Achternaam { get; set; }
        
        [JsonProperty("e-mail")]
        [JsonPropertyName("e-mail")]
        public string Email { get; set; }
    }
}