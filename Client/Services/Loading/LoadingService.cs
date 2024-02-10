using System;
using System.Threading.Tasks;
using hub.Shared.Bases;

namespace hub.Client.Services.Loading;

public interface ILoadingService : INotifyStateChanged
{
	bool IsLoading { get; }
	void WithLoading(Action action);
	public Task WithLoading(Func<Task> action);
}

public class LoadingService : BaseNotifyStateChanged, ILoadingService
{
	private bool _isLoading;
	public bool IsLoading { get => _isLoading; private set => SetAndNotify(ref _isLoading, value); }

	public void WithLoading(Action action)
	{
		IsLoading = true;
		try
		{
			action.Invoke();
		}
		finally
		{
			IsLoading = false;
		}
	}
	
	public async Task WithLoading(Func<Task> action)
	{
		IsLoading = true;
		try
		{
			await action.Invoke();
		}
		finally
		{
			IsLoading = false;
		}
	}
}