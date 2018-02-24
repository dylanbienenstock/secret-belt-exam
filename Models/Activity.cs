using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeltExam.Models
{
    public class Activity : BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ActivityId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
        public float Duration { get; set; }
        public string DurationUnits { get; set; }
        public int CreatorId { get; set; }
    }
}