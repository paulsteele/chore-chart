using System;
using hub.Shared.Tools;

namespace hub.Shared.Models.Countdown
{
    public enum DisplayType
    {
        FortNight = 1,
        Weeks = 2,
        Days = 3,
        Hours = 4,
        Minutes = 5,
        Seconds = 6,
        Milliseconds = 7,
        Nanoseconds = 8
    }
    public class CountdownModel
    {
        private readonly INowTimeProvider _nowTimeProvider;

        public CountdownModel(string name, DateTime dateTime, string image, string imageCredits, INowTimeProvider nowTimeProvider)
        {
            _nowTimeProvider = nowTimeProvider;
            Name = name;
            DateTime = dateTime;
            ImageCredits = imageCredits;
            Image = image;
            DisplayType = DisplayType.Seconds;
        }

        public DisplayType DisplayType { get; set; }
        public string Image { get; }
        public string ImageCredits { get; }
        public string Name { get; }
        private DateTime DateTime { get; }

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
                    DisplayType.FortNight => $"{timeLeft.TotalDays / 14d:0}fortnights",
                    DisplayType.Weeks => $"{timeLeft.TotalDays / 7d:0} weeks",
                    DisplayType.Days => $"{timeLeft.TotalDays:0} days",
                    DisplayType.Hours => $"{timeLeft.TotalHours:0} hours.",
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