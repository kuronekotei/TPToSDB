using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TPCmn {
	static partial class W32 {
		/// <summary>ライブラリ読込</summary><returns>hModule</returns>
		[DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern IntPtr LoadLibrary(string lpFileName);
		/// <summary>ライブラリ解放</summary>
		[DllImport("kernel32", SetLastError = true)]
		internal static extern bool FreeLibrary(IntPtr hModule);
		/// <summary>ライブラリから関数を取得</summary>
		[DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = false)]
		internal static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

		/// <summary>前面表示</summary>
		[DllImport("user32.dll")]
		public static extern bool SetForegroundWindow(IntPtr hWnd);
		/// <summary>ウィンドウ状態変更</summary>
		[DllImport("user32.dll")]
		public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
		/// <summary>最小化状態取得</summary>
		[DllImport("user32.dll")]
		public static extern bool IsIconic(IntPtr hWnd);

		/// <summary>画面を元の大きさに戻す</summary>
		public const int SW_RESTORE = 9;
	}
}
