using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using hub.Server.NativeBle;
using hub.Shared.Models.Bluetooth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace hub.Server.Controllers {

	[ApiController]
	[Route("scale")]
	public class ScaleController : ControllerBase{
		private readonly IBluetooth _bluetooth;
		private readonly ILogger<ScaleController> _logger;

		public ScaleController(
			IBluetooth bluetooth,
			ILogger<ScaleController> logger
		) {
			_bluetooth = bluetooth;
			_logger = logger;
		}

		[HttpPut]
		public async Task<IActionResult> StartScanning() {
			_bluetooth.Init();

			var devices = new List<BluetoothDevice>();

			_bluetooth.Setup(
				() => _logger.LogInformation( "Scanning Start"),
				() => _logger.LogInformation( "Scanning End"),
				(device, id) => {
					_logger.LogInformation(device);
					devices.Add(new BluetoothDevice{Id = id, Name = device});
				},
				() => _logger.LogInformation( "Device Connected"),
				(device, id) => _logger.LogInformation( $"{device} {id} disconnected")
			);

			_bluetooth.StartScan();
			await Task.Delay(20000);
			_bluetooth.StopScan();
			_bluetooth.Dispose();
			_bluetooth.Destruct();

			return Ok(new ScanResult { Success = true, FoundDevices = devices.ToArray()});
		}
	}
}
