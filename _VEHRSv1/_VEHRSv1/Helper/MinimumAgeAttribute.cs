using System;
using System.ComponentModel.DataAnnotations;

namespace _VEHRSv1.Helper
{
    public class MinimumAgeAttribute : ValidationAttribute
    {
        private readonly int _minimumAge;

        public MinimumAgeAttribute(int minimumAge)
        {
            _minimumAge = minimumAge;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateOnly birthDate)
            {
                // Get the current date as a DateOnly
                DateOnly today = DateOnly.FromDateTime(DateTime.Today);
                int age = today.Year - birthDate.Year;

                // Adjust age if the birthday has not occurred this year yet
                if (birthDate > today.AddYears(-age))
                {
                    age--;
                }

                if (age < _minimumAge)
                {
                    return new ValidationResult($"Age must be at least {_minimumAge} years old.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
