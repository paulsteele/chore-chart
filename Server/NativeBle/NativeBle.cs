using System;
using System.Runtime.InteropServices;

namespace hub.Server.NativeBle {
	public class NativeBle {
		private const string Dll = "/Users/paul/personal/hub/Server/NativeBle/bin/libnativeble_c.dylib";

		[DllImport(Dll, EntryPoint = "ble_construct")]
		public static extern IntPtr BleConstruct();

		[DllImport(Dll, EntryPoint = "ble_destruct")]
		public static extern void BleDestruct(IntPtr bleService);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void ScanCallback(string a, string b);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void DeviceDisconnected(string a, string b);

		[DllImport(Dll, EntryPoint = "ble_setup")]
		public static extern void BleSetup(
			IntPtr bleService,
			Action onScanStart,
			Action onScanEnd,
			ScanCallback onScanFound,
			Action onDeviceConnected,
			DeviceDisconnected onDeviceDisconnected
		);

		[DllImport(Dll, EntryPoint = "ble_scan_start")]
		public static extern void BleScanStart(IntPtr bleService);

		[DllImport(Dll, EntryPoint = "ble_scan_stop")]
		public static extern void BleScanStop(IntPtr bleService);

		[DllImport(Dll, EntryPoint = "ble_scan_is_active")]
		public static extern bool BleIsScanActive(IntPtr bleService);

		[DllImport(Dll, EntryPoint = "ble_scan_timeout")]
		public static extern void BleScanTimeout(IntPtr bleService, int timeout);

		[DllImport(Dll, EntryPoint = "ble_disconnect")]
		public static extern void BleDisconnect(IntPtr bleService);

		[DllImport(Dll, EntryPoint = "ble_dispose")]
		public static extern void BleDispose(IntPtr bleService);
	}
}
