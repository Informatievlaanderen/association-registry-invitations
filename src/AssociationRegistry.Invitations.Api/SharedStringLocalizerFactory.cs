using Microsoft.Extensions.Localization;

namespace AssociationRegistry.Invitations.Api;

public class SharedStringLocalizerFactory<T> : IStringLocalizerFactory
{
    private readonly ResourceManagerStringLocalizerFactory _resourceManagerStringLocalizerFactory;

    public SharedStringLocalizerFactory(ResourceManagerStringLocalizerFactory resourceManagerStringLocalizerFactory)
        => _resourceManagerStringLocalizerFactory = resourceManagerStringLocalizerFactory;

    public IStringLocalizer Create(Type resourceSource)
        => resourceSource == typeof(T)
            ? _resourceManagerStringLocalizerFactory.Create(resourceSource)
            : new SharedStringLocalizer<T>(_resourceManagerStringLocalizerFactory.Create(resourceSource));

    public IStringLocalizer Create(string baseName, string location) => throw new NotSupportedException();
}