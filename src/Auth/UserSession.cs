using MongoDB.Bson;

namespace CourseSharesApp.Auth
{
    public static class UserSession
    {
        public static ObjectId CurrentUserId { get; private set; }
        public static string CurrentUserRole { get; private set; } = string.Empty;
        public static string CurrentUserName { get; private set; } = string.Empty;

        public static bool IsLoggedIn => CurrentUserId != ObjectId.Empty;

        public static void Login(ObjectId id, string role, string name)
        {
            CurrentUserId = id;
            CurrentUserRole = role ?? string.Empty;
            CurrentUserName = name ?? string.Empty;
        }

        public static void Logout()
        {
            CurrentUserId = ObjectId.Empty;
            CurrentUserRole = string.Empty;
            CurrentUserName = string.Empty;
        }
    }
}
