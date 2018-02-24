using System;

namespace BeltExam.Models
{
	public abstract class BaseModel
	{
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
	}
}