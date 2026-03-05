using _VEHRSv1.Helper;
using System.ComponentModel.DataAnnotations;

namespace _VEHRSv1.Models
{
    public class vmPemeEmployeeRecord
    {
        public string PemeidString { get; set; }
        public int Pemeid { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z\s.-]{1,100}$", ErrorMessage = "Invalid Last Name")]
        public string LastName { get; set; } = null!;

        [Required]
        [RegularExpression(@"^[a-zA-Z\s.-]{1,100}$", ErrorMessage = "Invalid First Name")]
        public string FirstName { get; set; } = null!;

        [RegularExpression(@"^[a-zA-Z\s.-]{1,100}$", ErrorMessage = "Invalid Middle Name")]
        public string? MiddleName { get; set; }


        [Required(ErrorMessage = "Birthdate is required.")]
        [DataType(DataType.Date)] // Change to Date
        [CustomDateValidation(ErrorMessage = "Birth Date cannot be a future date.")]
        [MinimumAge(20, ErrorMessage = "Age must be at least 20 years old.")]
        public DateOnly? Birthdate { get; set; }


        [Required]
        public string Position { get; set; }


        public string? Status { get; set; }
        public string? Idnumber { get; set; }
        public string? MedicalEvaluator { get; set; }

        public ICollection<vmPemeHeaderDetails>? PemeDetails { get; set; }

    }



    
}
