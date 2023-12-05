using Swashbuckle.AspNetCore.Filters;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Ophalen.VoorVereniging;

internal class UitnodigingenResponseExamples : IExamplesProvider<UitnodigingenResponse>
{
    public UitnodigingenResponse GetExamples()
        => new()
        {
            Uitnodigingen = new[]
            {
                new Uitnodiging
                {
                    UitnodigingId = Guid.NewGuid(),
                    VCode = "V0000001",
                    Boodschap = "Boodschap voor uitgenodigde",
                    Uitgenodigde = new Uitgenodigde
                    {
                        Voornaam = "John",
                        Achternaam = "Doe",
                        Email = "john.doe@example.com",
                        Insz = "00000000001",
                    },
                    Validator = new Validator
                        { VertegenwoordigerId = 1 },
                    Uitnodiger = new Uitnodiger
                    {
                        VertegenwoordigerId = 12345,
                    },
                    Status = UitnodigingsStatus.All[Random.Shared.Next(0, UitnodigingsStatus.All.Length - 1)],
                    DatumRegistratie = DateTime.Today.AddDays(-1).ToLongDateString(),
                    DatumLaatsteAanpassing = DateTime.Today.ToLongDateString(),
                },
                new Uitnodiging
                {
                    UitnodigingId = Guid.NewGuid(),
                    VCode = "V0000001",
                    Boodschap = "Boodschap voor uitgenodigde",
                    Uitgenodigde = new Uitgenodigde
                    {
                        Voornaam = "Jane",
                        Achternaam = "Smith",
                        Email = "jane.smith@example.com",
                        Insz = "00000000002",
                    },
                    Validator = new Validator
                        { VertegenwoordigerId = 5 },
                    Uitnodiger = new Uitnodiger
                    {
                        VertegenwoordigerId = 12345,
                    },
                    Status = UitnodigingsStatus.All[Random.Shared.Next(0, UitnodigingsStatus.All.Length - 1)],
                    DatumRegistratie = DateTime.Today.AddDays(-1).ToLongDateString(),
                    DatumLaatsteAanpassing = DateTime.Today.ToLongDateString(),
                },
            },
        };
}
