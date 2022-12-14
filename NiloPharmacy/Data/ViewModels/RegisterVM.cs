using xyzpharmacy.Models;
using System.ComponentModel.DataAnnotations;

namespace xyzpharmacy.Data.ViewModels
{
    public class RegisterVM
    {
        [Display(Name = "Full name")]
        [Required(ErrorMessage = "Full name is required")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Full Name contains only alphabets")]
        [StringLength(30,MinimumLength =3,ErrorMessage ="Name should have maximum of 3 letters")]
        public string FullName { get; set; }

        [Display(Name = "Email address")]
        [Required(ErrorMessage = "Email address is required")]
        [RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9]+)*\\.([a-z]{2,4})$", ErrorMessage = "Invalid email format.")]
        [EmailAddress]
        public string EmailAddress { get; set; }
       
        
        [Required(ErrorMessage = "Please enter phone number")]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^[6789]\d{9}$", ErrorMessage = "Please enter 10 digit Mobile No.")]
        [Phone]
        public string Contact { get; set; }
        
        [Display(Name = "Age")]
        [Age]
        public int Age { get; set; }
        [Required(ErrorMessage = "Please enter date of birth")]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public System.DateTime DateOfBirth { get; set; }
        [Required(ErrorMessage = "Gender is Required")]
        [Display(Name = "Gender")]
        public Gender Gender { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{6,}$", ErrorMessage = "Passwords must be at least 6 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]

        public string Password { get; set; }

        [Display(Name = "Confirm password")]
        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }

    public class Age : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
        {
            var User = (RegisterVM)validationContext.ObjectInstance;

            if (User.DateOfBirth == null)
                return new ValidationResult("Date of Birth is required.");

            var age = DateTime.Today.Year - User.DateOfBirth.Year;

            return (age ==User.Age)
                ? ValidationResult.Success
                : new ValidationResult("User Age and DOB does'nt match.");
        }
    }
}
