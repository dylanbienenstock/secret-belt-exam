using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeltExam.Models
{
    public class ActivityUserJoin : BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ActivityUserJoinId { get; set; }

        public int ActivityId { get; set; }
        public Activity Activity { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}