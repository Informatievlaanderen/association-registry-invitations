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
           .Must(BeValidInsz)
           .WithName(u => $"{nameof(u.Aanvrager)}.{nameof(u.Aanvrager.Insz)}")
           .WithMessage(u => $"{nameof(u.Aanvrager.Insz)} is ongeldig.")
           .When(u => u.Aanvrager != null);
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

        var inszNumber = long.Parse(trimmedInsz);

        if (97 - inszNumber / 100 % 97 != inszNumber % 100) return false;

        return true;
    }

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