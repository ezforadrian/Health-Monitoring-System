using _VEHRSv1.Helper;
using System.ComponentModel.DataAnnotations;

namespace _VEHRSv1.Models
{
    public class ECUDetailEdit
    {
        public string IdEnc { get; set; }
        public string EcuHeaderIdEnc { get; set; }
        public int EcuHeaderId { get; set; }
        public DateOnly EcuDate { get; set; }
        public string BranchCodeEnc { get; set; }
        public string? Remarks { get; set; }
        public List<vmTestResult>? TestResults { get; set; }
        //public List<vmTestDetails>? AllEcuTestDetails { get; set; }
    }
}
