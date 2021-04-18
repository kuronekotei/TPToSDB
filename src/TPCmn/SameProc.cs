using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static TPCmn.Stc;
using static TPCmn.W32;

namespace TPCmn {
	public static class SameProc {

		/// <summary>自身と同名のプロセスがあればそれをアクティブ化し、Trueを返す</summary>
		/// <returns>True：同名プロセスあり　False：同名プロセス無し</returns>
		public static bool SameProcWakeup() {
			Process prevProcess = GetPreviousProcess();
			if (prevProcess != null) {
				WakeupWindow(prevProcess.MainWindowHandle);
				return true;
			}
			return false;
		}

		// 外部プロセスのウィンドウを起動する
		public static void WakeupWindow(IntPtr hWnd) {
			// メイン・ウィンドウが最小化されていれば元に戻す
			if (IsIconic(hWnd)) {
				ShowWindowAsync(hWnd, SW_RESTORE);
			}

			// メイン・ウィンドウを最前面に表示する
			SetForegroundWindow(hWnd);
		}

		// 実行中の同じアプリケーションのプロセスを取得する
		public static Process GetPreviousProcess() {
			Process curProcess = Process.GetCurrentProcess();
			Process[] allProcesses = Process.GetProcessesByName (curProcess.ProcessName);

			foreach (Process checkProcess in allProcesses) {
				// 自分自身のプロセスIDは無視する
				if (checkProcess.Id != curProcess.Id) {
					// プロセスのフルパス名を比較して同じアプリケーションか検証
					if (string.Compare(checkProcess.MainModule.FileName, curProcess.MainModule.FileName, true) == 0) {
						// 同じフルパス名のプロセスを取得
						return checkProcess;
					}
				}
			}

			// 同じアプリケーションのプロセスが見つからない！
			return null;
		}
	}
}
