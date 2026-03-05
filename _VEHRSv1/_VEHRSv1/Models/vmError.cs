namespace _VEHRSv1.Models
{
    public enum ErrorType
    {
        ParameterNull,
        DatabaseError,
        NotFound,
        Unauthorized,
        WebServerError,
        Unknown
    }
    public class vmError
    {
        public int Id { get; set; }
        public string IdString { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public ErrorType Type { get; set; }
    }
}
