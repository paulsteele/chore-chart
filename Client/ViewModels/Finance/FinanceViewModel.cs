using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using hub.Client.Services.Loading;
using hub.Client.Services.Web;
using hub.Shared.Bases;
using hub.Shared.Models.Finance;
using Microsoft.Extensions.Logging;

namespace hub.Client.ViewModels.Finance;

public interface IFinanceViewModel : INotifyStateChanged
{
	Task Initialize();
}

public class FinanceViewModel(
	AuthedHttpClient httpClient,
	ILoadingService loadingService,
	ILogger logger
) : BaseNotifyStateChanged, IFinanceViewModel
{
	public async Task Initialize()
	{
		await loadingService.WithLoading(async () =>
		{
			await httpClient.Init();
			var categories = await httpClient.GetFromJsonAsync<List<Category>>("finance/categories");
			
			logger.LogInformation($"{categories.Count}");
		});
	}

}