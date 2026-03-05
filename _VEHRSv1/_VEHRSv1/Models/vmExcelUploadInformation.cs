namespace _VEHRSv1.Models
{
    public class vmExcelUploadInformation
    {
        public string Message { get; set; }
        public int UploadedRecordsCount { get; set; }
        public int DuplicateRecordsCount { get; set; }
        public int FailedRecordsCount { get; set; }
        public List<(int RowNumber, string ErrorMessage)> FailedRecordsDetails { get; set; }

    }
}
