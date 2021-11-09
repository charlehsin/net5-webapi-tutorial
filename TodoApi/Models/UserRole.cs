namespace TodoApi.Models
{
    public static class UserRole
    {
        public enum Roles
        {
            Admin = 0,
            User = 1
        }

        public const string RoleAdmin = "Admin";
        public const string RoleUser = "User";
        public const string RoleAdminOrUser = "Admin, User";
    }
}