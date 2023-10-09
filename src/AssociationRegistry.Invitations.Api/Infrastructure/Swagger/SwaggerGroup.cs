using Microsoft.AspNetCore.Mvc;

namespace AssociationRegistry.Invitations.Api.Infrastructure.Swagger;

public static class SwaggerGroup
{
    public class Beheer : ApiExplorerSettingsAttribute
    {
        public Beheer()
        {
            GroupName = "Beheer";
        }
    }
    
    public class Personen : ApiExplorerSettingsAttribute
    {
        public Personen()
        {
            GroupName = "Personen";
        }
    }
    
    public class Verenigingen : ApiExplorerSettingsAttribute
    {
        public Verenigingen()
        {
            GroupName = "Verenigingen";
        }
    }

}