using System;
using Microsoft.AspNetCore.Components;

namespace hub.Server.NativeBle {
	public interface IBluetooth {
		public void Init();
		public void Setup(Action onScanStart, Action onScanEnd, NativeBle.ScanCallback onScanFound, Action onDeviceConnected, NativeBle.DeviceDisconnected onDeviceDisconnected);
		public void StartScan();
		public void StopScan();
		public void Dispose();
		public void Destruct();
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

		public void StartScan() {
			if (IsNotInitialized) {
				return;
			}

			NativeBle.BleScanStart(BluetoothReference);
		}

		public void StopScan() {
			if (IsNotInitialized) {
				return;
			}

			NativeBle.BleScanStop(BluetoothReference);
		}

		public void Dispose() {
			if (IsNotInitialized) {
				return;
			}

			NativeBle.BleDispose(BluetoothReference);
		}

		public void Destruct() {
			if (IsNotInitialized) {
				return;
			}

			NativeBle.BleDestruct(BluetoothReference);
			BluetoothReference = IntPtr.Zero;
		}

		private bool IsNotInitialized => BluetoothReference == IntPtr.Zero;
	}
}
