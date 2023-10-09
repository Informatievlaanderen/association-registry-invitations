namespace AssociationRegistry.Invitations.Api.Infrastructure;

public static class DevelopmentExtensions
{
    public static IApplicationBuilder ConfigureDevelopmentEnvironment(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment()) return app;
        return app
            .UseDeveloperExceptionPage();
    }
}