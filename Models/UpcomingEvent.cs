using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLink.Models
{
    class UpcomingEvent
    {
        public string EventType { get; set; }
        public string Title { get; set; }
        public DateTime DateTimeStart { get; set; }
        public string TimeEnd { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }

        public string DisplayDate => DateTimeStart.ToString("dd.MM.yyyy");
        public string DisplayTimeStart => DateTimeStart.ToString("HH:mm");
        public string DisplayTimeRange => string.IsNullOrWhiteSpace(TimeEnd)
                                           ? DisplayTimeStart
                                           : $"{DisplayTimeStart} - {TimeEnd}";

        public string EventStyle => GetEventStyle(EventType);

        private static string GetEventStyle(string type)
        {
            return type switch
            {
                "Mecz" => "#C0392B",    // Red
                "Trening" => "#27AE60", // Green
                "Wydarzenie" => "#2980B9", // Blue
                _ => "#7F8C8D"         // Gray
            };
        }
    }
}