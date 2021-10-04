using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using hub.Client.Services.Web;
using hub.Shared.Bases;
using hub.Shared.Models.Bluetooth;

namespace hub.Client.ViewModels.Scale {
	public interface IScaleSetupViewModel : INotifyStateChanged {
		List<BluetoothDevice> FoundDevices { get; set; }
		Task ScanForDevices();
		bool Scanning { get; set; }
		int ScanningProgress { get; set; }
		string ScanningProgressWidthStyle { get; }
		int ScanningMax { get; set; }
	}

	public class ScaleSetupViewModel : BaseNotifyStateChanged, IScaleSetupViewModel  {
			private AuthedHttpClient _httpClient;

			public ScaleSetupViewModel(AuthedHttpClient httpClient) {
				_httpClient = httpClient;
			}

			public List<BluetoothDevice> FoundDevices { get; set; }

			public async Task ScanForDevices()
			{
				await _httpClient.Init();
				Scanning = true;
				Task<HttpResponseMessage> task = _httpClient.PutAsJsonAsync<object>("scale", this);
				for (ScanningProgress = 0; ScanningProgress < ScanningMax; ScanningProgress++) {
					if (task.IsCompleted) {
						break;
					}

					await Task.Delay(1000);
				}

				var response = await task;
				var scanResult = await response.Content.ReadFromJsonAsync<ScanResult>();

				if (response.IsSuccessStatusCode && scanResult != null) {
					FoundDevices = scanResult.FoundDevices.ToList();
				}
				Scanning = false;
			}

			private bool _scanning;

			public bool Scanning {
				get => _scanning;
				set => SetAndNotify(ref _scanning, value);
			}

			private int _scanningProgress;

			public int ScanningProgress {
				get => _scanningProgress;
				set => SetAndNotify(ref _scanningProgress, value);
			}

			private int _scanningMax = 20;

			public int ScanningMax {
				get => _scanningMax;
				set => SetAndNotify(ref _scanningMax, value);
			}

			public string ScanningProgressWidthStyle => $"width : {_scanningProgress * 5}%;";
	}
}
