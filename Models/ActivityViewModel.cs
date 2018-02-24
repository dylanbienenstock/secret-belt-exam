using System;
using System.ComponentModel.DataAnnotations;

namespace BeltExam.Models
{
    public class ActivityViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "Title is required.")]
        [MinLength(2)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Please enter a valid date.")]
        public DateTime? Date { get; set; }

        [Required(ErrorMessage = "Time is required.")]
        [DataType(DataType.Time, ErrorMessage = "Please enter a valid time.")]
        public DateTime? Time { get; set; }

        [Required(ErrorMessage = "Duration is required.")]
        public float? Duration { get; set; }

        [Required(ErrorMessage = "Duration Units is required.")]
        public string DurationUnits { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [MinLength(10)]
        public string Description { get; set; }
    }
}