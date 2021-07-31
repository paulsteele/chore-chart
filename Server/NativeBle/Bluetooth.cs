using System;
using Microsoft.AspNetCore.Components;

namespace hub.Server.NativeBle {
	public interface IBluetooth {
		public void Init();
		public void Setup(Action onScanStart, Action onScanEnd, NativeBle.ScanCallback onScanFound, Action onDeviceConnected, NativeBle.DeviceDisconnected onDeviceDisconnected);
		public void Scan();
	}

	public class Bluetooth : IBluetooth {
		private IntPtr BluetoothReference { get; set; } = IntPtr.Zero;

		public void Init() {
			if (IsNotInitialized) {
				BluetoothReference = NativeBle.BleConstruct();
			}
		}

		public void Setup(
			Action onScanStart,
			Action onScanEnd,
			NativeBle.ScanCallback onScanFound,
			Action onDeviceConnected,
			NativeBle.DeviceDisconnected onDeviceDisconnected
		) {
			if (IsNotInitialized) {
				return;
			}

			NativeBle.BleSetup(
				BluetoothReference,
				onScanStart,
				onScanEnd,
				onScanFound,
				onDeviceConnected,
				onDeviceDisconnected
			);
		}

		public void Scan() {
			if (IsNotInitialized) {
				return;
			}

			NativeBle.BleScanStart(BluetoothReference);
		}

		private bool IsNotInitialized => BluetoothReference == IntPtr.Zero;
	}
}
