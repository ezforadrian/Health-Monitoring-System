using _VEHRSv1.Helper;
using System.ComponentModel.DataAnnotations;

namespace _VEHRSv1.Models
{
    public class vmSavePemeDetail
    {
        public string? PemeDetailIdString { get; set; } 
        [Required]
        public string PemeIdString { get; set; }
        

        [Required]
        [DataType(DataType.DateTime)]
        [CustomDateValidation(ErrorMessage = "Medical Exam cannot be a future date.")]
        public DateTime ExamDate { get; set; }

        [Required]
        public string MedicalEvaluatorIdString { get; set; }

        [Required]
        public string Remarks { get; set; }

        

    }
}
