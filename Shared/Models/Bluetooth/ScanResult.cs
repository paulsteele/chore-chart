namespace hub.Shared.Models.Bluetooth
{
	public class ScanResult {
		public bool Success { get; set; }
		public BluetoothDevice[] FoundDevices { get; set; }
	}
}
