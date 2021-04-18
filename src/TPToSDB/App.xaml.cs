using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using TPCmn;

namespace TPToS {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application {
		/// <summary>メインウィンドウ　クラスは適宜書き換える</summary>
		Window window;
		//Window window;

		/// <summary>
		/// アプリケーション開始
		/// </summary>
		protected override void OnStartup(StartupEventArgs e) {
			try {
				Cmn.InitCmn(Assembly.GetExecutingAssembly());
				Log.InitLog(15);

				if (SameProc.SameProcWakeup()) {
					Log.SaveLog("★★★同名プロセスがあるため終了");
					Environment.Exit(-3);
				}
			} catch (Exception ex) {
				Log.SaveLog(ex);
				//MsgBox.ShowMsg(null, "起動時に問題が発生しました。");  //	メッセージ取得前の可能性があるため、String版
				Environment.Exit(-1);
			}
			Log.SaveLog($"▼▼▼App.OnStartup()");
			BackThrd.StartBack();
			// メイン ウィンドウ表示
			window  = new MainWindow();

			window.Show();
			Log.SaveLog($"▲▲▲App.OnStartup()");
		}

		/// <summary>
		/// アプリケーション終了
		/// </summary>
		protected override void OnExit(ExitEventArgs e) {
			Log.SaveLog($"▼▼▼App.OnExit({e.ApplicationExitCode})");
			BackThrd.EndBack();
			Log.SaveLog($"▲▲▲App.OnExit({e.ApplicationExitCode})");
		}

		/// <summary>
		/// 未処理例外
		/// </summary>
		protected void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
			Log.SaveLog("★★★App.Application_DispatcherUnhandledExceptionが発生しました。", e.Exception);
			//MsgBox.ShowMsg(null, message_id.S0000001);
			e.Handled = true;
			Environment.Exit(-2);
		}

		/// <summary>
		/// 非アクティブ化
		/// </summary>
		private void Application_Deactivated(object sender, EventArgs e) {
		}

		/// <summary>
		/// アクティブ化
		/// </summary>
		private void Application_Activated(object sender, EventArgs e) {
		}
	}
}
