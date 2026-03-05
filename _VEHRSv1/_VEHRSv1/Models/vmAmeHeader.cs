using _VEHRSv1.Helper;
using System.ComponentModel.DataAnnotations;

namespace _VEHRSv1.Models
{
    public class vmAmeHeader
    {
        public string EmployeeIdNumberEnc { get; set; }
        public string AmeHeaderIdEnc { get; set; }
        public int AmeheaderId { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [CustomDateValidation(ErrorMessage = "AME Date cannot be a future date.")]
        public DateOnly Amedate { get; set; }
        public DateOnly? RunDate { get; set; }
        public string Branch { get; set; } = null!;
        public string? BranchCodeEnc { get; set; } = null!;
        public string? BranchDesc { get; set; } = null!;
        public string? Remarks { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDateTime { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public List<AME_vmAmeDetails> Amedetails { get; set; } = new List<AME_vmAmeDetails>();

    }
}
