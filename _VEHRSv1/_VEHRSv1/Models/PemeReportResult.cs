namespace _VEHRSv1.Models
{
    public class PemeReportResult
    {
        public string? EmployeeId { get; set; }
        public string? FullName { get; set; }
        public DateOnly? BirthDate { get; set; }
        public DateOnly? ExamDate { get; set; }
        public int? Status { get; set; }
        public string? StatusDescription { get; set; }
        public string? MedicalEvaluator { get; set; }
    }
}
