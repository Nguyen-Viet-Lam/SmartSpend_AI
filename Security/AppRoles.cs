namespace SmartSpendAI.Security
{
    public static class AppRoles
    {
        public const string Guest = "Guest";
        public const string User = "User";
        public const string Admin = "Admin";

        // Keep legacy names so existing database records and tokens stay valid.
        public const string StandardUser = "StandardUser";
        public const string SystemAdmin = "SystemAdmin";

        public static bool IsAdmin(string? roleName)
        {
            return string.Equals(roleName, Admin, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(roleName, SystemAdmin, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsUser(string? roleName)
        {
            return string.Equals(roleName, User, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(roleName, StandardUser, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsKnownRole(string? roleName)
        {
            return IsAdmin(roleName) || IsUser(roleName) || string.Equals(roleName, Guest, StringComparison.OrdinalIgnoreCase);
        }

        public static string ToDisplayRole(string? roleName)
        {
            if (IsAdmin(roleName))
            {
                return Admin;
            }

            if (IsUser(roleName))
            {
                return User;
            }

            return Guest;
        }
    }
}
