namespace _VEHRSv1.Models
{
    public class vmExcelUploadDetails
    {
        public vmExcelUploadInformation ExcelUploadInformation { get; set; }
        public Task<IEnumerable<vmAppReference>> AppReferenceExcelRecord { get; set; }

    }
}
