using Microsoft.Extensions.Localization;

namespace AssociationRegistry.Invitations.Api.Infrastructure.Localization;

public class GlobalStringLocalizer
{
    private readonly object _lock = new object();
    private readonly IServiceProvider _serviceProvider;
    private readonly IDictionary<Type, IStringLocalizer> _localizers = new Dictionary<Type, IStringLocalizer>();

    public static GlobalStringLocalizer Instance;

    public GlobalStringLocalizer(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public IStringLocalizer<T> GetLocalizer<T>()
    {
        var type = typeof(T);

        lock (_lock)
        {
            if (!_localizers.ContainsKey(type))
                _localizers.Add(type, _serviceProvider.GetService<IStringLocalizer<T>>());
        }

        return (IStringLocalizer<T>)_localizers[type];
    }
}