using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeltExam.Models
{
	public class User : BaseModel
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

		[Required]
		[StringLength(64)]
		public string FirstName { get; set; }

		[Required]
		[StringLength(64)]
		public string LastName { get; set; }

		[Required]
		[EmailAddress]
		public string EmailAddress { get; set; }

		[Required]
		public string PasswordHash { get; set; }

		public string LoginToken { get; set; }
	}
}