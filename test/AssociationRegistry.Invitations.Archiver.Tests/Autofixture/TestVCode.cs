namespace AssociationRegistry.Invitations.Archiver.Tests.Autofixture;

public record TestVCode
{
    private string Value { get; }

    public TestVCode(string value)
    {
        Value = value;
    }

    public override string ToString()
        => Value;

    public static implicit operator string(TestVCode vCode)
        => vCode.Value;

    public static implicit operator TestVCode(string vCode)
        => new(vCode);
}