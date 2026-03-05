using _VEHRSv1.Helper;
using System.ComponentModel.DataAnnotations;

namespace _VEHRSv1.Models
{
    public class vmEcuHeader
    {

        public string EmployeeIdNumberEnc { get; set; }
        public string EcuHeaderIdEnc { get; set; }
        public int EcuheaderId { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [CustomDateValidation(ErrorMessage = "ECU Date cannot be a future date.")]
        public DateOnly Ecudate { get; set; }
        public DateOnly? RunDate { get; set; }
        public string Branch { get; set; } = null!;
        public string? BranchCodeEnc { get; set; } = null!;
        public string? BranchDesc { get; set; } = null!;
        public string? Remarks { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDateTime { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public List<ECU_vmEcuDetails> Ecudetails { get; set; } = new List<ECU_vmEcuDetails>();


    }

    


}
