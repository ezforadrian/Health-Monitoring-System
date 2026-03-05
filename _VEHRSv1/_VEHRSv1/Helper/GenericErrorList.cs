using _VEHRSv1.Models;

namespace _VEHRSv1.Helper
{
    public class GenericErrorList
    {
        public static List<vmError> Errors = new List<vmError>
        {
            new vmError { Id = 1, IdString = "1", Code = "200", Message = "Null or Invalid Parameter", Type = ErrorType.ParameterNull },
            new vmError { Id = 2, IdString = "2", Code = "500", Message = "Database Error", Type = ErrorType.DatabaseError },
            new vmError { Id = 3, IdString = "3", Code = "404", Message = "Resource Not Found", Type = ErrorType.NotFound },
            new vmError { Id = 4, IdString = "4", Code = "401", Message = "Unauthorized Access", Type = ErrorType.Unauthorized },
            new vmError { Id = 5, IdString = "5", Code = "503", Message = "Web Server Error", Type = ErrorType.WebServerError },
            new vmError { Id = 6, IdString = "6", Code = "520", Message = "Unknown Error", Type = ErrorType.Unknown },
            // Add more errors as needed
        };


        public static vmError GetError(ErrorType type)
        {
            return Errors.Find(e => e.Type == type);
        }
    }
}
