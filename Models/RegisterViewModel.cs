using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using OpenTableApp.Models.Validators;

namespace OpenTableApp.Models
{
    public class RegisterViewModel : IValidatableObject
    {
        [Required]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d]+$", ErrorMessage = "UserName must contain at least one upper-case letter, one lower-case letter, and one number.")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^\d{3}[-.]?\d{3}[-.]?\d{4}$", ErrorMessage = "Phone number must be in the format 123-456-7890 or 123.456.7890")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Date of Birth")]
        [ValidDOB(ErrorMessage = "DOB must be a valid date in the past and no more than 100 years old.")]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }

        [Required(ErrorMessage = "Please enter a password.")]
        [DataType(DataType.Password)]
        [MinLength(7, ErrorMessage = "Password must be at least 7 characters long.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm your password.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DOB > DateTime.Today || DOB < DateTime.Today.AddYears(-100))
            {
                yield return new ValidationResult("DOB must be in the past and not more than 100 years ago.", new[] { nameof(DOB) });
            }
        }
    }
}
