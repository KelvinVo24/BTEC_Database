namespace MyStore
{
    public static class Session
    {
        public static string UserRole;
        public static string EmployeeID;

        public static void ClearToLogin()
        {
            UserRole = null;
            EmployeeID = null;
        }
    }
 }