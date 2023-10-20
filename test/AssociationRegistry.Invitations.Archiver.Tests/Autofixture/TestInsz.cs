namespace AssociationRegistry.Invitations.Archiver.Tests.Autofixture;

public record TestInsz
{
    private string Value { get; }

    public TestInsz(string value)
    {
        Value = value;
    }

    public override string ToString()
        => Value;

    public static implicit operator string(TestInsz insz)
        => insz.Value;

    public static implicit operator TestInsz(string insz)
        => new(insz);
}