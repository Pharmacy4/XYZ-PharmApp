using Microsoft.AspNetCore.Authorization.Infrastructure;
using xyzpharmacy.Data.Static;
using xyzpharmacy.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace xyzpharmacy.Data.ViewModels
{
    public class EditUserViewModel
    {
        public EditUserViewModel()
        {
            Claims = new List<string>();
            Roles = new List<string>();
            
        }
        public string Id { get; set; }

        [Display(Name = "Full name")]
        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; }

        [Display(Name = "Email address")]
        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress]
        public string EmailAddress { get; set; }


        [Required(ErrorMessage = "Please enter phone number")]
        [Display(Name = "Phone Number")]
        [Phone]
        public string Contact { get; set; }
        [Required(ErrorMessage = "Age is Required")]
        [Display(Name = "Age")]
        [Ages]
        public int Age { get; set; }
        [Required(ErrorMessage = "Please enter date of birth")]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public System.DateTime DateOfBirth { get; set; }
        [Required(ErrorMessage = "Gender is Required")]
        [Display(Name = "Gender")]
        public Gender Gender { get; set; }

        public List<string> Claims { get; set; }

        public IList<string> Roles { get; set; }

        
    }

    public class Ages : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
        {
            var User = (EditUserViewModel)validationContext.ObjectInstance;

            if (User.DateOfBirth == null)
                return new ValidationResult("Date of Birth is required.");

            var age = DateTime.Today.Year - User.DateOfBirth.Year;

            return (age == User.Age)
                ? ValidationResult.Success
                : new ValidationResult("User Age and DOB does'nt match.");
        }
    }
}
