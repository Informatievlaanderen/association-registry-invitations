using System.Reflection;

namespace AssociationRegistry.Invitations.Api;

public static class AssemblyVersionExtension
{
    public static string GetVersionText(this Assembly assembly)
    {
        var version = assembly.GetName().Version;
        var informationVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

        return string.IsNullOrWhiteSpace(informationVersion)
            ? $"v{version}"
            : $"v{version} ({informationVersion})";
    }
}