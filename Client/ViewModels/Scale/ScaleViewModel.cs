using System;
using System.ComponentModel;
using hub.Shared.Bases;
using hub.Shared.Models.Bluetooth;

namespace hub.Client.ViewModels.Scale {
	public interface IScaleViewModel : INotifyStateChanged
	{
		string ConnectedDeviceName { get; }
		string ConnectedDeviceId { get; }
		string CircleClass { get; }
	}

	public class ScaleViewModel : BaseNotifyStateChanged, IScaleViewModel {

		private BluetoothStatus _bluetoothStatus;
		public BluetoothStatus BluetoothStatus {
			get => _bluetoothStatus;
			set => SetAndNotify(ref _bluetoothStatus, value);
		}

		public string ConnectedDeviceName => BluetoothStatus?.ConnectedDevice?.Name ?? "Unknown Name";
		public string ConnectedDeviceId => BluetoothStatus?.ConnectedDevice?.Id ?? "Unknown Id";

		public string CircleClass {
			get {
				var connectionStatus = _bluetoothStatus?.ConnectionStatus ?? ConnectionStatus.Unknown;
				var type = connectionStatus switch {
					ConnectionStatus.Disconnected => "oi-circle-x text-danger",
					ConnectionStatus.Connected => "oi-circle-check text-success",
					ConnectionStatus.Unknown => "oi-clock text-warning",
					_ => ""
				};

				return $"oi circle-size {type}";
			}
		}
	}
}
