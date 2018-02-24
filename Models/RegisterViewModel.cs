using System.ComponentModel.DataAnnotations;

namespace BeltExam.Models
{
	public class RegisterViewModel : BaseViewModel
	{
		[Required(ErrorMessage = "* First name is required")]
		[StringLength(64, ErrorMessage="* Must be less than 64 characters.")]
		[Display(Name = "First Name")]
        [RegularExpression("[a-zA-Z]+", ErrorMessage = "First name must contain only letters A-Z")]
        public string FirstName { get; set; }

		[Required(ErrorMessage = "* Last name is required")]
		[StringLength(64, ErrorMessage="* Must be less than 64 characters.")]
		[Display(Name = "Last Name")]
		[RegularExpression("[a-zA-Z]+", ErrorMessage = "Last name must contain only letters A-Z")]
		public string LastName { get; set; }

		[Required(ErrorMessage = "* Email address is required")]
		[EmailAddress]
		[Display(Name = "Email Address")]
		public string EmailAddress { get; set; }

		[Required(ErrorMessage = "* Password is required")]
		[DataType(DataType.Password)]
		[RegularExpression(@"^([a-zA-Z+]+[0-9+]+[&@!#+]+)$", ErrorMessage = "Password must contain at least 1 letter, 1 number, and 1 special character.")]
		public string Password { get; set; }

		[Required(ErrorMessage = "* Confirm password is required")]
		[Compare("Password", ErrorMessage = "Passwords must match.")]
		[DataType(DataType.Password)]
		[Display(Name = "Confirm Password")]
		public string PasswordConfirmation { get; set; }
	}
}