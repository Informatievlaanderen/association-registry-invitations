namespace AssociationRegistry.Invitations.Api.Aanvragen.Registreer;

using FluentValidation;

public class AanvraagValidator : AbstractValidator<AanvraagRequest>
{
    public AanvraagValidator()
    {
        RuleFor(u => u.VCode)
           .NotNull()
           .WithMessage(u => $"{nameof(u.VCode)} is verplicht.");

        RuleFor(u => u.VCode)
           .Must(BeValidFormat)
           .WithMessage(u => $"{nameof(u.VCode)} heeft een ongeldig formaat. (V#######)")
           .When(u => !string.IsNullOrEmpty(u.VCode));

        RuleFor(u => u.Aanvrager)
           .NotNull()
           .WithMessage(u => $"{nameof(u.Aanvrager)} is verplicht.");

        RuleFor(u => u.Aanvrager.Insz)
           .NotNull()
           .WithName(u => $"{nameof(u.Aanvrager)}.{nameof(u.Aanvrager.Insz)}")
           .WithMessage(u=> $"{nameof(u.Aanvrager.Insz)} is verplicht.")
           .When(u => u.Aanvrager != null);
        RuleFor(u => u.Aanvrager.Insz)
           .Must(BeValidInsz)
           .WithName(u => $"{nameof(u.Aanvrager)}.{nameof(u.Aanvrager.Insz)}")
           .WithMessage(u=> $"{nameof(u.Aanvrager.Insz)} is ongeldig. (##.##.##-###.## of ###########)")
           .When(u => u.Aanvrager?.Insz != null);
        
        RuleFor(u => u.Aanvrager.Achternaam)
           .NotNull()
           .WithName(u => $"{nameof(u.Aanvrager)}.{nameof(u.Aanvrager.Achternaam)}")
           .WithMessage(u=> $"{nameof(u.Aanvrager.Achternaam)} is verplicht.")
           .When(u => u.Aanvrager != null);
        RuleFor(u => u.Aanvrager.Voornaam)
           .NotNull()
           .WithName(u => $"{nameof(u.Aanvrager)}.{nameof(u.Aanvrager.Voornaam)}")
           .WithMessage(u=> $"{nameof(u.Aanvrager.Voornaam)} is verplicht.")
           .When(u => u.Aanvrager != null);

        RuleFor(u => u.Aanvrager.Email)
           .NotNull()
           .WithName(u => $"{nameof(u.Aanvrager)}.{nameof(u.Aanvrager.Email)}")
           .WithMessage(u=> $"{nameof(u.Aanvrager.Email)} is verplicht.")
           .When(u => u.Aanvrager != null);
        RuleFor(u => u.Aanvrager.Email)
           .EmailAddress()
           .WithName(u => $"{nameof(u.Aanvrager)}.{nameof(u.Aanvrager.Email)}")
           .WithMessage(u=> $"{nameof(u.Aanvrager.Email)} is ongeldig.")
           .When(u => u.Aanvrager?.Email != null);
    }

    private static bool BeValidInsz(string insz)
    {
        if (insz.Length == 15 &&
            (insz[2] != '.' || insz[5] != '.' || insz[8] != '-' || insz[12] != '.')) return false;

        var trimmedInsz = insz.Length < 13
            ? insz
            : insz.Remove(startIndex: 12, count: 1).Remove(startIndex: 8, count: 1).Remove(startIndex: 5, count: 1)
                  .Remove(startIndex: 2, count: 1);

        if (trimmedInsz.Length != 11) return false;
        if (!trimmedInsz.All(char.IsDigit)) return false;
        
        return BeValidInszBefore2000(trimmedInsz) || BeValidInszAfter2000(trimmedInsz);
    }

    private static bool BeValidInszBefore2000(string trimmedInsz)
    {
        var inszNumber = long.Parse(trimmedInsz);
        return 97 - ((inszNumber / 100) % 97) == inszNumber % 100;
    }

    private static bool BeValidInszAfter2000(string trimmedInsz) => BeValidInszBefore2000("2" + trimmedInsz);

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