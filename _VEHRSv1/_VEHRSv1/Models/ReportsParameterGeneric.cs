using _VEHRSv1.Helper;
using System.ComponentModel.DataAnnotations;

public class ReportsParameterGeneric
{
    [Required(ErrorMessage = "Start Date is required.")]
    [CustomDateOnlyValidation(ErrorMessage = "Encoded Start Date should not be in the future.")]
    [DateRangeValidation("EncodedEndDate", ErrorMessage = "Encoded Start Date should not be greater than end date.")]
    public DateOnly? EncodedStartDate { get; set; }

    [Required(ErrorMessage = "End Date is required.")]
    [CustomDateOnlyValidation(ErrorMessage = "Encoded End Date should not be in the future.")]
    [DateRangeValidation("EncodedStartDate", ErrorMessage = "Encoded End Date should not be less than start date.")]
    public DateOnly? EncodedEndDate { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    public string Status { get; set; }
}
