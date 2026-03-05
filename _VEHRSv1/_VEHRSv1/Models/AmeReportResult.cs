namespace _VEHRSv1.Models
{
    public class AmeReportResult
    {
        public DateOnly? AmeDate { get; set; }
        public string? Branch { get; set; }
        public string? IDNumber { get; set; }
        public string? Name { get; set; }
        public string? Position { get; set; }
        public int? BirthMonth { get; set; }
        public string? CombinedFindings { get; set; }
        public int? TestId { get; set; }
        public string? TestName { get; set; }
        public bool? TestResult { get; set; }

    }
}
