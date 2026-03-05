using _VEHRSv1.Helper;
using System.ComponentModel.DataAnnotations;

namespace _VEHRSv1.Models
{
    public class AmeReportParameters
    {
        [Required(ErrorMessage = "Start Date is required.")]
        [CustomDateOnlyValidation(ErrorMessage = "Start Date should not be in the future.")]
        [DateRangeValidation("EndDate", ErrorMessage = "Start Date should not be greater than end date.")]
        public DateOnly? StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required.")]
        [CustomDateOnlyValidation(ErrorMessage = "End Date should not be in the future.")]
        [DateRangeValidation("StartDate", ErrorMessage = "End Date should not be less than start date.")]
        public DateOnly? EndDate { get; set; }
        [Required(ErrorMessage = "Findings is required.")]
        public string? Findings { get; set; }
    }
}
