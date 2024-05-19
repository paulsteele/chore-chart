using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Web;
using hub.Client.Services.Loading;
using hub.Client.Services.Web;
using hub.Shared.Bases;
using hub.Shared.Models.Finance;
using hub.Shared.Tools;
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
	Task Import(InputFileChangeEventArgs args);
	List<Category> Categories { get; }
	List<Transaction> Transactions { get; }
	bool OnlyDisplayMonth { get; set; }
	bool OnlyDisplayUncategorized { get; set; }
	Task ChangeTransactionCategory(Transaction transaction, object categoryId);
}

public class FinanceViewModel(
	AuthedHttpClient httpClient,
	ILoadingService loadingService,
	INowTimeProvider nowTimeProvider,
	ILogger logger
) : BaseNotifyStateChanged, IFinanceViewModel
{
	public List<Category> Categories { get; } = [];
	public List<Transaction> Transactions { get; } = [];
	private bool _onlyDisplayMonth = true;
	private bool _onlyDisplayUncategorized = true;

	public bool OnlyDisplayMonth
	{
		get => _onlyDisplayMonth;
		set
		{
			_onlyDisplayMonth = value;
			Refresh();
		}
	}

	public bool OnlyDisplayUncategorized
	{
		get => _onlyDisplayUncategorized;
		set
		{
			_onlyDisplayUncategorized = value;
			Refresh();
		}
	}

	public async Task Initialize()
	{
		await loadingService.WithLoading(async () =>
		{
			await httpClient.Init();
			var categories = await httpClient.GetFromJsonAsync<List<Category>>("finance/categories");
			var transactions = await GetTransactions();
			await SetBalance();
			
			Categories.AddRange(categories);
			Transactions.AddRange(transactions);
			
			NotifyStateChanged();
		});
	}

	public string Balance { get; private set; }
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

		await Refresh();
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
		await loadingService.WithLoading(async () =>
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
			var transactions = await GetTransactions();
			
			Transactions.Clear();
			Transactions.AddRange(transactions);
			await SetBalance();
			
			NotifyStateChanged();
		});
	}
	
	private async Task Refresh()
	{
		await loadingService.WithLoading(async () =>
		{
			var transactions = await GetTransactions();
			
			Transactions.Clear();
			Transactions.AddRange(transactions);
			NotifyStateChanged();
		});
	}

	public async Task ChangeTransactionCategory(Transaction transaction, object categoryIdObject)
	{
		if (categoryIdObject is not string categoryString)
		{
			return;
		}

		if (!int.TryParse(categoryString, out var categoryId))
		{
			return;
		}
		
		var category = Categories.FirstOrDefault(c => c.Id == categoryId);

		await httpClient.PutAsJsonAsync($"finance/transaction/{transaction.Id}", category);

		transaction.Category = category;
	}

	private async Task<List<Transaction>> GetTransactions()
	{

		var queryString = HttpUtility.ParseQueryString(string.Empty);

		if (_onlyDisplayMonth)
		{
			var now = nowTimeProvider.Now;

			queryString.Add("month", now.Month.ToString());
			queryString.Add("year", now.Year.ToString());
		}

		if (_onlyDisplayUncategorized)
		{
			queryString.Add("onlyUncategorized", "true");
		}
		
		var transactions = await httpClient.GetFromJsonAsync<List<Transaction>>($"finance/transactions?{queryString}");

		return transactions;
	}

	private async Task SetBalance()
	{
		var balance = await httpClient.GetFromJsonAsync<decimal>("finance/balance");

		Balance = $"{balance:C}";
	}
}