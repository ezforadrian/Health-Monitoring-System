using _VEHRSv1.Helper;
using System.ComponentModel.DataAnnotations;

namespace _VEHRSv1.Models
{
    public class ECUEditRecord
    {
        [Required]
        public string IdEnc { get; set; }
        [Required]
        public string EcuHeaderIdEnc { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [CustomDateOnlyValidation(ErrorMessage = "ECU Date cannot be a future date.")]
        public DateOnly EcuDate { get; set; }
        [Required]
        public string BranchCodeEnc { get; set; }
        [RegularExpression(@"^[a-zA-Z\s.-]{1,300}$", ErrorMessage = "No speacial characters")]
        public string? Remarks { get; set; }
        public List<string> TestIdEnc { get; set; }
    }
}
