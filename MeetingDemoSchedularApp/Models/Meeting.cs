using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MeetingDemoSchedularApp.Models
{
    public class Meeting
    {
        [Required]
        public string Title { get; set; }

        [Required]
        [EmailAddress]
        public List<string> Emails { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan StartTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan EndTime { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public string Agenda { get; set; }
    }
}
