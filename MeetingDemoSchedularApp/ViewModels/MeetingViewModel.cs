using MeetingDemoSchedularApp.Models;
using System.Collections.Generic;

namespace MeetingDemoSchedularApp.ViewModels
{
    public class MeetingViewModel
    {
        public List<Meeting> Meetings { get; set; }
        public Meeting NewMeeting { get; set; }
    }
}
