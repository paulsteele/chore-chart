using System;
using System.Web;
using hub.Shared.Bases;
using Microsoft.AspNetCore.Components;

namespace hub.Client.ViewModels.HellYeah;

public interface IHellYeahViewModel : INotifyStateChanged
{
	string ImageUrl { get; set; }
	string PendingImageUrl { get; set; }
	void GoToPermalink();
}

public class HellYeahViewModel(NavigationManager navigationManager) : BaseNotifyStateChanged, IHellYeahViewModel
{
	private string _imageUrl = "https://images.unsplash.com/photo-1541701494587-cb58502866ab?q=80&w=3540&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D";
	public string PendingImageUrl { get; set; }

	public string ImageUrl
	{
		get => _imageUrl;
		set
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				return;
			}
			
			_imageUrl = HttpUtility.UrlDecode(value);
		}
	}


	public void GoToPermalink()
	{
		var locationOfQuery = navigationManager.Uri.IndexOf("?", StringComparison.Ordinal);
		var pathWithoutQuery = locationOfQuery != -1
			? navigationManager.Uri[..locationOfQuery]
			: navigationManager.Uri;

		var secondUrl = HttpUtility.UrlEncode(PendingImageUrl);

		navigationManager.NavigateTo($"{pathWithoutQuery}?Image={secondUrl}");
	}
}