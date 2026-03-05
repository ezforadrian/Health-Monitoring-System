using System.ComponentModel.DataAnnotations;
using _VEHRSv1.Helper;
namespace _VEHRSv1.Models
{
    public class vmSavePemeRecord
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z\s.-]{1,100}$", ErrorMessage = "Invalid Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[a-zA-Z\s.-]{1,100}$", ErrorMessage = "Invalid First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[a-zA-Z\s.-]{1,100}$", ErrorMessage = "Invalid Middle Name")]
        public string MiddleName { get; set; } = string.Empty;

        //[Required]
        //public string Department { get; set; } = string.Empty;

        [Required]
        public string Position { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.DateTime)]
        [CustomDateValidation(ErrorMessage = "Medical Exam cannot be a future date.")]
        public DateTime? MedicalExamDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [CustomDateValidation(ErrorMessage = "Birth Date cannot be a future date.")]
        [MinimumAge(20, ErrorMessage = "Age must be at least 20 years old.")]
        public DateOnly? Birthdate { get; set; }  

        [Required]
        public string Status { get; set; } = string.Empty;

        [Required]
        public string MedicalEvaluator { get; set; } = string.Empty;

        [RegularExpression(@"^[a-zA-Z\s.-]{1,300}$", ErrorMessage = "No speacial characters")]
        public string? Remarks { get; set; } = string.Empty;
    }

    
}
