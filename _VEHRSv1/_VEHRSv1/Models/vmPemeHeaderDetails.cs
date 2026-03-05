namespace _VEHRSv1.Models
{
    public class vmPemeHeaderDetails
    {
        public int DetailId { get; set; }
        public string DetailIdString { get; set; }

        public int Pemeid { get; set; }
        public string PemeIdString { get; set; }

        public DateTime ExamDate { get; set; }

        public string? Remarks { get; set; }

        public int? MedicalEvaluatorId { get; set; } 
        public string? MedicalEvaluatorIdString { get; set; }
        public string? MedicalEvaluatorName { get; set; }

    }
}
