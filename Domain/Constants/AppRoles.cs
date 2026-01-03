using System.Collections.ObjectModel;


namespace Domain.Constants
{
    public static class AppRoles
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string Admin = "Admin";
        public const string User = "User";
        public const string Provider = "Provider";

        public static readonly ReadOnlyCollection<string> RegistrationRoles =
            new ReadOnlyCollection<string>(new List<string> { User, Provider });
    }
}
