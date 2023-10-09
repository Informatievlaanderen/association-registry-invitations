using Microsoft.AspNetCore.Mvc;

namespace AssociationRegistry.Invitations.Api;

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