using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TPCmn {
	public static class Cmn {
		static Cmn() {
			Assem       = Assembly.GetExecutingAssembly();
			AppPath     = Assem.Location;
			AppVer      = Assem.GetName().Version;
			CurDir      = Environment.CurrentDirectory;
			PCName      = Environment.MachineName;
			UserName    = Environment.UserName;
			HostName    = Dns.GetHostName();
			Proc        = Process.GetCurrentProcess();
			ProcName    = Proc.ProcessName;
			ProcId      = Proc.Id;
			IPAddress[] ipas = Dns.GetHostAddresses(HostName);
			foreach (IPAddress ipa in ipas) {
				if (ipa.AddressFamily==AddressFamily.InterNetwork) {
					IPv4 = ipa;
				}
				if (ipa.AddressFamily==AddressFamily.InterNetworkV6) {
					IPv6 = ipa;
				}
			}
			NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
			foreach (NetworkInterface adapter in adapters) {
				// ネットワーク接続状態が UP のアダプタのみ
				if (adapter.OperationalStatus   == OperationalStatus.Up
				&& adapter.NetworkInterfaceType != NetworkInterfaceType.Loopback
				&& adapter.NetworkInterfaceType != NetworkInterfaceType.Tunnel
				&& adapter.NetworkInterfaceType != NetworkInterfaceType.Unknown
				) {
					// 物理（MAC）アドレスの取得
					MacAddr = adapter.GetPhysicalAddress();
					break;
				}
			}
			Refresh();
		}
		public static void InitCmn(Assembly assem) {
			Assem       = assem;
			AppPath     = Assem.Location;
			AppVer      = Assem.GetName().Version;
		}
		public static void Refresh() {
			Proc.Refresh();
			PhysMem     = Proc.WorkingSet64;
			using ManagementClass mc = new ManagementClass("Win32_OperatingSystem");
			using ManagementObjectCollection moc = mc.GetInstances();
			foreach (ManagementObject mo in moc) {
				//合計物理メモリ
				TotalPhysMem = mo["TotalVisibleMemorySize"].__ToDecF(0);
				//利用可能な物理メモリ
				FreePhysMem = mo["FreePhysicalMemory"].__ToDecF(0);
				//合計仮想メモリ
				Console.WriteLine("合計仮想メモリ:{0}KB", mo["TotalVirtualMemorySize"]);
				//利用可能な仮想メモリ
				Console.WriteLine("利用可能仮想メモリ:{0}KB", mo["FreeVirtualMemory"]);

				mo.Dispose();
			}
		}
		/// <summary>プロセス情報</summary>
		public  static  Process         Proc;
		/// <summary>プロセス名</summary>
		public  static  string          ProcName;
		/// <summary>プロセス名</summary>
		public  static  int             ProcId;
		/// <summary>アセンブリ情報</summary>
		public  static  Assembly        Assem;
		/// <summary>アプリケーション実行パス</summary>
		public  static  string          AppPath;
		/// <summary>アプリケーションバージョン</summary>
		public  static  Version         AppVer;
		/// <summary>カレントパス</summary>
		public  static  string          CurDir;
		/// <summary>実行PC名</summary>
		public  static  string          PCName;
		/// <summary>実行PCユーザー名</summary>
		public  static  string          UserName;
		/// <summary>実行PC：ホスト名</summary>
		public  static  string          HostName;
		/// <summary>実行PC：IPv6</summary>
		public  static  IPAddress       IPv6;
		/// <summary>実行PC：IPv4</summary>
		public  static  IPAddress       IPv4;
		/// <summary>実行PC：Macアドレス</summary>
		public  static  PhysicalAddress MacAddr;
		/// <summary>実装メモリ</summary>
		public  static  long            PhysMem;
		public  static  decimal         FreePhysMem;
		public  static  decimal         TotalPhysMem;
	}//class Cmn
}//namespace TPCmn
