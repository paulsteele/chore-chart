using System;
using System.Threading.Tasks;
using System.Timers;
using hub.Shared.Bases;
using hub.Shared.Models.Countdown;
using hub.Shared.Tools;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace hub.Client.ViewModels.Countdown
{
    public interface ICountdownViewModel : INotifyStateChanged, IDisposable
    {
        ElementReference Audio { get; set; }
        Task ToggleAudio();
        string Icon { get; }
        CountdownModel CurrentModel { get; }
        void StartTimer();
    }

    public class CountdownViewModel : BaseNotifyStateChanged, ICountdownViewModel
    {
        private readonly IJSRuntime _jsRuntime;
        private bool _isPlaying;
        private Timer _timer;

        public CountdownViewModel(IJSRuntime jsRuntime, INowTimeProvider nowTimeProvider)
        {
            _jsRuntime = jsRuntime;
            _isPlaying = false;

            var model = new CountdownModel(
                "Christmas",
                nowTimeProvider.Now.Add(new TimeSpan(1, 0, 0)), 
                "assets/countdown/christmas-tree.svg", 
                "https://www.cutthatdesign.com/free-svg-christmas-tree-with-lights-design/",
                nowTimeProvider
            );
            CurrentModel = model;
        }

        public void StartTimer()
        {
            _timer = new Timer(TimerDelay);
            _timer.Elapsed += (_, _) => NotifyStateChanged();
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }
        
        public CountdownModel CurrentModel { get; }
        public ElementReference Audio { get; set; }

        public async Task ToggleAudio()
        {
            if (_isPlaying)
            {
                await _jsRuntime.InvokeVoidAsync("pauseAudio", Audio);
            }
            else
            {
                await _jsRuntime.InvokeVoidAsync("playAudio", Audio);
            }

            _isPlaying = !_isPlaying;
            NotifyStateChanged();
        }

        public string Icon => "oi " + (_isPlaying ? "oi-media-pause" : "oi-media-play");

        public void Dispose()
        {
            _timer.Dispose();
        }

        private int TimerDelay
        {
            get
            {
                return CurrentModel.DisplayType switch
                {
                    DisplayType.FortNight => 1000 * 60 * 60,
                    DisplayType.Weeks => 1000 * 60 * 60,
                    DisplayType.Days => 1000 * 60 * 60,
                    DisplayType.Hours => 1000 * 60,
                    DisplayType.Minutes => 1000 * 5,
                    DisplayType.Seconds => 1000,
                    DisplayType.Milliseconds => 100,
                    DisplayType.Nanoseconds => 100,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
    }
}