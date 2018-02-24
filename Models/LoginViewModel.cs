using System.ComponentModel.DataAnnotations;

namespace BeltExam.Models
{
	public class LoginViewModel : BaseViewModel
	{
		[Required(ErrorMessage = "* Required")]
		[EmailAddress]
		[Display(Name = "Email Address")]
		public string LoginEmailAddress { get; set; } // Prefixed name to avoid id reuse

		[Required(ErrorMessage = "* Required")]
		[DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string LoginPassword { get; set; }  // Prefixed name to avoid id reuse
    }
}