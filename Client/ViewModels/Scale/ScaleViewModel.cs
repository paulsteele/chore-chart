using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace hub.Client.ViewModels.Scale {
	public interface IScaleViewModel {
		Task StartScanning();
	}

	public class ScaleViewModel : IScaleViewModel {
			private HttpClient _httpClient;

			public ScaleViewModel(HttpClient httpClient) {
				_httpClient = httpClient;
			}

			public async Task StartScanning() {
				var response = await _httpClient.PutAsJsonAsync<object>("scale", this);
			}
	}
}
