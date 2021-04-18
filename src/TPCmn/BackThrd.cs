using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using static TPCmn.Stc;

namespace TPCmn {
	public static class BackThrd {
		static bool fLoop   = false;
		static Thread thrdBack;
		static DateTime dtLastBeat = new DateTime();
		static DateTime dtNextBeat = new DateTime();
		public static bool StartBack() {
			//	スレッドが開始してないなら開始する
			if (thrdBack == null) {  //	開始してない
				fLoop   = true;
				thrdBack   = new Thread(new ThreadStart(BackLoop));
				thrdBack.Start();
				return true;
			}
			return false;
		}
		public static bool EndBack() {
			//	スレッドが開始してるなら終了する
			if (thrdBack == null) {  //	開始してない
				return false;
			}
			fLoop   = false;
			if (!thrdBack.Join(1000)) {
#pragma warning disable SYSLIB0006 // 型またはメンバーが旧型式です
				thrdBack.Abort();
#pragma warning restore SYSLIB0006 // 型またはメンバーが旧型式です;
			}
			thrdBack   = null;
			return true;
		}
		private static void BackLoop() {
			Log.SaveLog("▼▼▼BackLoop開始");
			while (fLoop) {
				if (dtNextBeat<=DateTime.Now) {
					dtLastBeat = DateTime.Now._Slice5Min();
					dtNextBeat = dtLastBeat.AddSeconds(300);
					Cmn.Proc.Refresh();

					Log.SaveLog($"〇〇〇定時ログ /ハンドル{Cmn.Proc.HandleCount,6:#,#} /メモリ利用{Cmn.PhysMem/MiB,7:#,#}M /残{Cmn.FreePhysMem/KiB,7:#,#}M /実装{Cmn.TotalPhysMem/KiB,7:#,#}M");
				}
				Thread.Sleep(100);
				continue;
			}
			Log.SaveLog("▲▲▲BackLoop終了");
		}
	}
}
