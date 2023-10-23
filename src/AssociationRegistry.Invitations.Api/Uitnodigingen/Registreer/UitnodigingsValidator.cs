using FluentValidation;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Registreer;

public class UitnodigingsValidator : AbstractValidator<UitnodigingsRequest>
{
    public UitnodigingsValidator()
    {
        RuleFor(u => u.VCode)
            .NotNull()
            .WithMessage(u=> $"{nameof(u.VCode)} is verplicht.");
        RuleFor(u => u.VCode)
            .Must(BeValidFormat)
            .WithMessage(u=> $"{nameof(u.VCode)} heeft een ongeldig formaat. (V#######)")
            .When(u => !string.IsNullOrEmpty(u.VCode));

        RuleFor(u => u.Uitnodiger)
            .NotNull()
            .WithMessage(u=> $"{nameof(u.Uitnodiger)} is verplicht.");
        RuleFor(u => u.Uitnodiger.VertegenwoordigerId)
            .Must(BeValidVertegenwoordigerid)
            .WithName(u => $"{nameof(u.Uitnodiger)}.{nameof(u.Uitnodiger.VertegenwoordigerId)}")
            .WithMessage(u=> $"{nameof(u.Uitnodiger.VertegenwoordigerId)} is ongeldig.")
            .When(u => u.Uitnodiger != null);

        RuleFor(u => u.Uitgenodigde)
            .NotNull()
            .WithMessage(u=> $"{nameof(u.Uitgenodigde)} is verplicht.");

        RuleFor(u => u.Uitgenodigde.Insz)
            .NotNull()
            .WithName(u => $"{nameof(u.Uitgenodigde)}.{nameof(u.Uitgenodigde.Insz)}")
            .WithMessage(u=> $"{nameof(u.Uitgenodigde.Insz)} is verplicht.")
            .When(u => u.Uitgenodigde != null);
        RuleFor(u => u.Uitgenodigde.Insz)
            .Must(BeValidInsz)
            .WithName(u => $"{nameof(u.Uitgenodigde)}.{nameof(u.Uitgenodigde.Insz)}")
            .WithMessage(u=> $"{nameof(u.Uitgenodigde.Insz)} is ongeldig. (##.##.##-###.## of ###########)")
            .When(u => u.Uitgenodigde?.Insz != null);
        RuleFor(u => u.Uitgenodigde.Achternaam)
            .NotNull()
            .WithName(u => $"{nameof(u.Uitgenodigde)}.{nameof(u.Uitgenodigde.Achternaam)}")
            .WithMessage(u=> $"{nameof(u.Uitgenodigde.Achternaam)} is verplicht.")
            .When(u => u.Uitgenodigde != null);
        RuleFor(u => u.Uitgenodigde.Voornaam)
            .NotNull()
            .WithName(u => $"{nameof(u.Uitgenodigde)}.{nameof(u.Uitgenodigde.Voornaam)}")
            .WithMessage(u=> $"{nameof(u.Uitgenodigde.Voornaam)} is verplicht.")
            .When(u => u.Uitgenodigde != null);
        RuleFor(u => u.Uitgenodigde.Email)
            .NotNull()
            .WithName(u => $"{nameof(u.Uitgenodigde)}.{nameof(u.Uitgenodigde.Email)}")
            .WithMessage(u=> $"{nameof(u.Uitgenodigde.Email)} is verplicht.")
            .When(u => u.Uitgenodigde != null);
        RuleFor(u => u.Uitgenodigde.Email)
            .EmailAddress()
            .WithName(u => $"{nameof(u.Uitgenodigde)}.{nameof(u.Uitgenodigde.Email)}")
            .WithMessage(u=> $"{nameof(u.Uitgenodigde.Email)} is ongeldig.")
            .When(u => u.Uitgenodigde?.Email != null);
    }


    private static bool BeValidInsz(string insz)
    {
        if (insz.Length == 15 &&
            (insz[2] != '.' || insz[5] != '.' || insz[8] != '-' || insz[12] != '.')) return false;

        var trimmedInsz = insz.Length < 13 ? insz : insz.Remove(12, 1).Remove(8, 1).Remove(5, 1).Remove(2, 1);
        if (trimmedInsz.Length != 11) return false;
        if (!trimmedInsz.All(char.IsDigit)) return false;

        var inszNumber = long.Parse(trimmedInsz);
        if (97 - ((inszNumber / 100) % 97) != inszNumber % 100) return false;

        return true;
    }

    private static bool BeValidVertegenwoordigerid(int vertegenwoordigerId) =>
        vertegenwoordigerId > 0;

    private bool BeValidFormat(string vCode)
    {
        if (vCode.Length != 8)
            return false;
        if (!vCode.ToUpper().StartsWith("V"))
            return false;
        if (!int.TryParse(vCode.ToUpper()[1..], out _))
            return false;

        return true;
    }
}
