using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml; // WAŻNE: Potrzebne do obsługi typu Visibility

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

        // --- NOWE WŁAŚCIWOŚCI (To naprawi błąd) ---
        public bool IsTraining => EventType == "Trening";

        // Właściwość sterująca widocznością przycisku "Obecność"
        public Visibility TrainingVisibility => IsTraining ? Visibility.Visible : Visibility.Collapsed;
        // -------------------------------------------

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