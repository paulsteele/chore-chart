using System.Threading.Tasks;
using hub.Server.NativeBle;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace hub.Server.Controllers {

	[ApiController]
	[Route("scale")]
	public class Scale : ControllerBase{
		private readonly IBluetooth _bluetooth;
		private readonly ILogger _logger;

		public Scale(
			IBluetooth bluetooth,
			ILogger logger
		) {
			_bluetooth = bluetooth;
			_logger = logger;
		}

		[HttpPut]
		public IActionResult StartScanning() {
			_bluetooth.Init();
			_bluetooth.Setup(
				() => _logger.Log( LogLevel.Debug, "Scanning Start"),
				() => _logger.Log(LogLevel.Debug, "Scanning End"),
				(device, id) => _logger.Log(LogLevel.Debug, $"{device} {id} found"),
				() => _logger.Log(LogLevel.Debug, "Device Connected"),
				(device, id) => _logger.Log(LogLevel.Debug, $"{device} {id} disconnected")
			);
			_bluetooth.Scan();

			return Accepted();
		}
	}
}
