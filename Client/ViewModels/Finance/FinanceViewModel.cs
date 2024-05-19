using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using hub.Client.Services.Loading;
using hub.Client.Services.Web;
using hub.Shared.Bases;
using hub.Shared.Models.Finance;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;

namespace hub.Client.ViewModels.Finance;

public interface IFinanceViewModel : INotifyStateChanged
{
	Task Initialize();
	string Balance { get; }
	string FreeToSpend { get; }
	Task AddCategory();
	Task SaveCategory(Category category);
	Task CancelEditingCategory(Category category);
	Task DeleteCategory(Category category);
	List<Category> Categories { get; }
	Task Import(InputFileChangeEventArgs args);
}

public class FinanceViewModel(
	AuthedHttpClient httpClient,
	ILoadingService loadingService,
	ILogger logger
) : BaseNotifyStateChanged, IFinanceViewModel
{
	public List<Category> Categories { get; } = [];

	public async Task Initialize()
	{
		await loadingService.WithLoading(async () =>
		{
			await httpClient.Init();
			var categories = (await httpClient.GetFromJsonAsync<List<Category>>("finance/categories")).OrderBy(c => c.Order);
			
			Categories.AddRange(categories);
			NotifyStateChanged();
		});
	}

	public string Balance => "$xxxx.xx";
	public string FreeToSpend => "$xxx.xx";
	public async Task AddCategory()
	{
		var newCategory = new Category
		{
			Budget = 0,
			Color = Color.Black,
			Emoji = '0',
			Name = "New Category",
			Order = Categories.Count
		};

		var result = await httpClient.PutAsJsonAsync("finance/category", newCategory);
		var responseCategory = await result.Content.ReadFromJsonAsync<Category>();
		
		Categories.Add(responseCategory);
	}

	public async Task DeleteCategory(Category category)
	{

		var result = await httpClient.DeleteAsync($"finance/category/{category.Id}");

		if (result.IsSuccessStatusCode)
		{
			Categories.Remove(category);
		}
	}
	
	public async Task SaveCategory(Category category)
	{
		var result = await httpClient.PutAsJsonAsync($"finance/category/{category.Id}", category);

		if (result.IsSuccessStatusCode)
		{
			category.Editing = false;
		}
	}
	
	public async Task CancelEditingCategory(Category category)
	{
		var result = await httpClient.GetFromJsonAsync<Category>($"finance/category/{category.Id}");

		if (result == null)
		{
			return;
		}

		Categories.Remove(category);
		Categories.Add(result);
		Categories.Sort((a,b) => a.Order - b.Order);
	}
	
	public async Task Import(InputFileChangeEventArgs args)
	{
		using var stream = new StreamReader(args.File.OpenReadStream());

		var list = new List<string>();
		var keepReading = true;

		while (keepReading)
		{
			var value = await stream.ReadLineAsync();
			if (value != null)
			{
				list.Add(value);
			}
			else
			{
				keepReading = false;
			}
		}
		
		await httpClient.PostAsJsonAsync("finance/import", list);
	}
}