using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml; 

namespace SoccerLink.Models
{
    public class UpcomingEvent
    {
        public int Id { get; set; }
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


        public bool IsTraining => EventType == "Trening";


        public Visibility TrainingVisibility => IsTraining ? Visibility.Visible : Visibility.Collapsed;

        private static string GetEventStyle(string type)
        {
            return type switch
            {
                "Mecz" => "#C0392B",    
                "Trening" => "#27AE60", 
                "Wydarzenie" => "#2980B9", 
                _ => "#7F8C8D"         
            };
        }
    }
}