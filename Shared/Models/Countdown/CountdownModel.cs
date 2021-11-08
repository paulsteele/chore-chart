using System;
using System.Linq;
using hub.Shared.Tools;

namespace hub.Shared.Models.Countdown
{
    public enum DisplayType
    {
        FortNight = 0,
        Weeks = 1,
        Days = 2,
        Hours = 3,
        Minutes = 4,
        Seconds = 5,
        Milliseconds = 6,
        Nanoseconds = 7
    }
    public class CountdownModel
    {
        private readonly INowTimeProvider _nowTimeProvider;

        public CountdownModel(
            string name, 
            DateTime dateTime,
            string image, 
            string imageCredits,
            string audio,
            string audioCredits,
            INowTimeProvider nowTimeProvider)
        {
            _nowTimeProvider = nowTimeProvider;
            Name = name;
            DateTime = dateTime;
            ImageCredits = imageCredits;
            Audio = audio;
            AudioCredits = audioCredits;
            Image = image;
            DisplayType = DisplayType.Days;
        }

        public DisplayType DisplayType { get; set; }
        public string Image { get; }
        public string ImageCredits { get; }
        public string Audio { get; }
        public string AudioCredits { get; }
        private string Name { get; }
        private DateTime DateTime { get; }

        public void CycleDisplayType(int diff)
        {
            var max = (int) Enum.GetValues(typeof(DisplayType)).Cast<DisplayType>().Max();

            var newValue = (int) DisplayType + diff;
            if (newValue < 0)
            {
                newValue = max;
            }

            DisplayType = (DisplayType) (newValue % (max + 1));
        }

        public string TimeLeft
        {
            get
            {
                var timeLeft = DateTime.Subtract(_nowTimeProvider.Now);

                if (timeLeft.Ticks < 0)
                {
                    timeLeft = TimeSpan.Zero;
                }
                
                var time =  DisplayType switch
                {
                    DisplayType.FortNight => $"{timeLeft.TotalDays / 14d:0} fortnights",
                    DisplayType.Weeks => $"{timeLeft.TotalDays / 7d:0} weeks",
                    DisplayType.Days => $"{timeLeft.TotalDays:0} days",
                    DisplayType.Hours => $"{timeLeft.TotalHours:0} hours",
                    DisplayType.Minutes => $"{timeLeft.TotalMinutes:0} minutes",
                    DisplayType.Seconds => $"{timeLeft.TotalSeconds:0} seconds",
                    DisplayType.Milliseconds => $"{timeLeft.TotalMilliseconds:0} milliseconds",
                    DisplayType.Nanoseconds => $"{timeLeft.Ticks * 100d:E2} nanoseconds",
                    _ => throw new ArgumentOutOfRangeException()
                };

                return $"There are {time} until {Name}";
            }
        }
    }
}