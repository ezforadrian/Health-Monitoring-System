namespace _VEHRSv1.Helper
{
    public class DatabaseConnectionException : Exception
    {
        public DatabaseConnectionException(string message, Exception innerException)
        : base(message, innerException)
        {
        }
    }
}
