#nullable enable
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
	string GetBalanceForCategory(Category category);
	DateTime SelectedDate { get; set; } 
}

public class FinanceViewModel : BaseNotifyStateChanged, IFinanceViewModel
{
	public List<Category> Categories { get; } = [];
	public List<Transaction> Transactions { get; } = [];
	private bool _onlyDisplayMonth = true;
	private bool _onlyDisplayUncategorized = true;
	private Balance? _balance;
	private DateTime _selectedDate;
	private readonly AuthedHttpClient _httpClient;
	private readonly ILoadingService _loadingService;

	public FinanceViewModel(AuthedHttpClient httpClient,
		ILoadingService loadingService,
		INowTimeProvider nowTimeProvider,
		ILogger logger)
	{
		_httpClient = httpClient;
		_loadingService = loadingService;
		_selectedDate = nowTimeProvider.Now;
	}

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

	public DateTime SelectedDate
	{
		get => _selectedDate;
		set
		{
			_selectedDate = value;
			Refresh();
			UpdateBalance();
		}
	}

	public async Task Initialize()
	{
		await _loadingService.WithLoading(async () =>
		{
			await _httpClient.Init();
			var categories = await _httpClient.GetFromJsonAsync<List<Category>>("finance/categories");
			var transactions = await GetTransactions();
			await UpdateBalance();
			
			Categories.AddRange(categories);
			Transactions.AddRange(transactions);
			
			NotifyStateChanged();
		});
	}

	public string Balance => $"{_balance?.Total:C}";
	public string FreeToSpend => $"{_balance?.LeftToSpend:C}";
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

		var result = await _httpClient.PutAsJsonAsync("finance/category", newCategory);
		var responseCategory = await result.Content.ReadFromJsonAsync<Category>();
		
		Categories.Add(responseCategory);
	}

	public async Task DeleteCategory(Category category)
	{

		var result = await _httpClient.DeleteAsync($"finance/category/{category.Id}");

		if (result.IsSuccessStatusCode)
		{
			Categories.Remove(category);
		}

		await Refresh();
	}
	
	public async Task SaveCategory(Category category)
	{
		var result = await _httpClient.PutAsJsonAsync($"finance/category/{category.Id}", category);

		if (result.IsSuccessStatusCode)
		{
			category.Editing = false;
		}
	}
	
	public async Task CancelEditingCategory(Category category)
	{
		var result = await _httpClient.GetFromJsonAsync<Category>($"finance/category/{category.Id}");

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
		await _loadingService.WithLoading(async () =>
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
			
			await _httpClient.PostAsJsonAsync("finance/import", list);
			var transactions = await GetTransactions();
			
			Transactions.Clear();
			Transactions.AddRange(transactions);
			await UpdateBalance();
			
			NotifyStateChanged();
		});
	}
	
	private async Task Refresh()
	{
		await _loadingService.WithLoading(async () =>
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

		await _httpClient.PutAsJsonAsync($"finance/transaction/{transaction.Id}", category);

		transaction.Category = category;

		await UpdateBalance();
	}

	public string GetBalanceForCategory(Category category)
	{
		if (_balance == null || !_balance.CategorySpend.TryGetValue(category.Id, out var spend))
		{
			return $"{category.Budget:C}";
		}
		
		return $"{category.Budget + spend:C}";
	}


	private async Task<List<Transaction>> GetTransactions()
	{

		var queryString = HttpUtility.ParseQueryString(string.Empty);

		if (_onlyDisplayMonth)
		{
			queryString.Add("month", _selectedDate.Month.ToString());
			queryString.Add("year", _selectedDate.Year.ToString());
		}

		if (_onlyDisplayUncategorized)
		{
			queryString.Add("onlyUncategorized", "true");
		}
		
		var transactions = await _httpClient.GetFromJsonAsync<List<Transaction>>($"finance/transactions?{queryString}");

		return transactions;
	}

	private async Task UpdateBalance()
	{
		_balance = await _httpClient.GetFromJsonAsync<Balance>($"finance/balance?year={_selectedDate.Year}&month={_selectedDate.Month}");

		NotifyStateChanged();
	}
}