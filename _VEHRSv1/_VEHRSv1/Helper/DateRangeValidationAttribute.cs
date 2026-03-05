using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace _VEHRSv1.Helper
{
    public class DateRangeValidationAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DateRangeValidationAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Ensure the value being validated is of type DateOnly?
            var currentValue = value as DateOnly?;

            // Get the comparison property from the validation context
            var comparisonProperty = validationContext.ObjectType.GetProperty(_comparisonProperty);

            // If the comparison property is not found, return an error
            if (comparisonProperty == null)
            {
                return new ValidationResult($"Unknown property: {_comparisonProperty}");
            }

            // Retrieve the value of the comparison property (the other date)
            var comparisonValue = comparisonProperty.GetValue(validationContext.ObjectInstance) as DateOnly?;

            // If both currentValue and comparisonValue are not null, perform validation
            if (currentValue.HasValue && comparisonValue.HasValue)
            {
                // Dynamically check the property being validated
                if (validationContext.MemberName == "StartDate" && currentValue > comparisonValue)
                {
                    return new ValidationResult("Start Date should not be greater than end date.");
                }
                else if (validationContext.MemberName == "EndDate" && currentValue < comparisonValue)
                {
                    return new ValidationResult("End Date should not be less than start date.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
