using System.Threading.Tasks;
using hub.Shared.Bases;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace hub.Client.ViewModels.Countdown
{
    public interface ICountdownViewModel : INotifyStateChanged
    {
        ElementReference Audio { get; set; }
        Task ToggleAudio();
        string Icon { get; }
    }

    public class CountdownViewModel : BaseNotifyStateChanged, ICountdownViewModel
    {
        private readonly IJSRuntime _jsRuntime;
        private bool _isPlaying;

        public CountdownViewModel(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
            _isPlaying = false;
        }
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
    }
}