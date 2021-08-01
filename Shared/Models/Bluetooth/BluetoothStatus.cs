namespace hub.Shared.Models.Bluetooth {
	public class BluetoothStatus {
		public BluetoothDevice ConnectedDevice { get; set; }
		public ConnectionStatus ConnectionStatus { get; set; }
	}

	public enum ConnectionStatus {
		Disconnected,
		Connected,
		Unknown
	}
}
