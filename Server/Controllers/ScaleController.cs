using System.Collections.Generic;
using System.Threading.Tasks;
using hub.Server.NativeBle;
using hub.Shared.Models.Bluetooth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace hub.Server.Controllers;

[ApiController]
[Route("scale")]
public class ScaleController(
	IBluetooth bluetooth,
	ILogger<ScaleController> logger
) : ControllerBase{
	
	[HttpPut]
	public async Task<IActionResult> StartScanning() {
		bluetooth.Init();

		var devices = new List<BluetoothDevice>();

		bluetooth.Setup(
			() => logger.LogInformation( "Scanning Start"),
			() => logger.LogInformation( "Scanning End"),
			(device, id) => {
				logger.LogInformation(device);
				devices.Add(new BluetoothDevice{Id = id, Name = device});
			},
			() => logger.LogInformation( "Device Connected"),
			(device, id) => logger.LogInformation( $"{device} {id} disconnected")
		);

		bluetooth.StartScan();
		await Task.Delay(20000);
		bluetooth.StopScan();
		bluetooth.Dispose();
		bluetooth.Destruct();

		return Ok(new ScanResult { Success = true, FoundDevices = devices.ToArray()});
	}
}