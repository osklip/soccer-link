using Microsoft.UI.Xaml.Data;
using System;

namespace SoccerLink.Helpers
{
    public class PositionToAbbreviationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var position = value as string;
            if (string.IsNullOrEmpty(position)) return "";

            return position.ToLower().Trim() switch
            {
                "bramkarz" => "BR",
                "obrońca" => "OBR",
                "pomocnik" => "POM",
                "napastnik" => "NAP",
                _ => position.Length >= 2 ? position.Substring(0, 2).ToUpper() : position.ToUpper()
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}