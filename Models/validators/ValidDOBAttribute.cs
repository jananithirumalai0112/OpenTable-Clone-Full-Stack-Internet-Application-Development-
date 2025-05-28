using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace OpenTableApp.Models.Validators
{
    public class ValidDOBAttribute : ValidationAttribute, IClientModelValidator
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime dob)
            {
                var today = DateTime.Today;
                if (dob >= today) return false;

                var age = today.Year - dob.Year;
                if (dob > today.AddYears(-age)) age--;

                return age <= 100;
            }
            return false;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-validdob", ErrorMessage ?? "DOB must be a valid date in the past and no more than 100 years old.");
        }
    }
}
