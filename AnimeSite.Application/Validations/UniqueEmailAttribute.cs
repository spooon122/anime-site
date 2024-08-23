using anime_site.Dto;
using AnimeSite.Core.Models;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimeSite.Application.Validations
{
    public class UniqueEmailAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var userManager = (UserManager<User>)validationContext.GetService(typeof(UserManager<User>));
            var email = value?.ToString();

            if (email != null)
            {
                var user = userManager.FindByEmailAsync(email).Result;
                if (user != null)
                {
                    return new ValidationResult("A user with this email already exists.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
