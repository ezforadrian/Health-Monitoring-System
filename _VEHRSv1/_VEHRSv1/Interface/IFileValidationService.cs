namespace _VEHRSv1.Interface
{
    public interface IFileValidationService
    {
        Task<(bool IsValid, string ErrorMessage)> ValidateFileAsync(IFormFile file);

        bool CovertToNullableBool(string value);
        DateTime ConvertToDateTime(string value);
        DateOnly ConvertToDateOnly(string dateString);
        int ConvertToInt(string value);
    }
}
