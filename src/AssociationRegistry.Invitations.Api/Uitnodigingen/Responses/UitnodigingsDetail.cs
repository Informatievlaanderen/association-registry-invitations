﻿namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Responses;

public class UitnodigingsDetail
{
    public Guid Id { get; set; }
    public string VCode { get; set; }
    public string Boodschap { get; set; }
    public string Status { get; set; }
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
        public string Naam { get; set; }
        public string Email { get; set; }
    }
}