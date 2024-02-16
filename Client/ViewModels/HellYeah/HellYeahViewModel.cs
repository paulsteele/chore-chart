using System;
using System.Net.Http;
using System.Threading.Tasks;
using hub.Shared.Bases;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;

namespace hub.Client.ViewModels.HellYeah;

public interface IHellYeahViewModel : INotifyStateChanged
{
	string ImageUrl { get; set; }
	Task Initialize();
	string ImageHex { get; }
}

public class HellYeahViewModel(ILogger logger) : BaseNotifyStateChanged, IHellYeahViewModel
{
	private string _imageUrl = "https://images.unsplash.com/photo-1541701494587-cb58502866ab?q=80&w=3540&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D";
	// private string _imageUrl = "https://upload.wikimedia.org/wikipedia/commons/4/4a/100x100_logo.png";

	public string ImageUrl
	{
		get => _imageUrl;
		set
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				return;
			}

			_imageUrl = value;
		}
	}

	private string _imageHex;
	public string ImageHex
	{
		get => _imageHex;
		private set => SetAndNotify(ref _imageHex, value);
	}

	public async Task Initialize()
	{
		try
		{
			logger.LogInformation("AAAAAA");
			using var httpClient = new HttpClient();
			logger.LogInformation("BBBBB");
			var stream = await httpClient.GetStreamAsync(ImageUrl);
			logger.LogInformation("CCCCC");
			var image = await Image.LoadAsync(stream);
			logger.LogInformation("DDDDD");

			// var settings = new MagickReadSettings
			// {
			// 	Font = "Calibri",
			// 	TextGravity = Gravity.Center,
			// 	BackgroundColor = MagickColors.Transparent,
			// 	Height = image.Height, 
			// 	Width = image.Width 
			// };

			// using var caption = new MagickImage($"caption:Hell Yeah", settings);

			// image.Composite(caption, image.Height / 2, image.Width / 2, CompositeOperator.Over);
			
			ImageHex = image.ToBase64String(PngFormat.Instance);
			logger.LogInformation("EEEEEE");
		}
		catch(Exception e)
		{
			logger.LogError($"{e.Message} - {e.StackTrace}");
		}
	}
}