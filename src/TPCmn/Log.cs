using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using static TPCmn.Stc;

namespace TPCmn {
	/// <summary>
	/// ログ処理を担うクラス
	/// </summary>
	public static partial class Log {
		public	static	bool fLog;
		public	static	FileStream fs;
		public	static	StreamWriter sw;
		public	static	string LogDir = Path.Combine(Cmn.CurDir, "Log");
		public	static	string DateStr = "19991231";
		public	static	string DateFmt = "yyyyMMdd";
		public	static	string LogFileName = "";
		public	static	int LogDelDate = 0;
		public	static	readonly Mutex mut = new Mutex(false,"TPCmnLog");

		/// <summary>
		/// 初期化を行う
		/// 基本的にプログラム全体で最初に1回だけ呼び出されること
		/// </summary>
		public static void InitLog(string dateFmt = "yyyyMMdd", int delDate = 0) {
			DateFmt = dateFmt;
			LogDelDate = delDate;
			bool mutFlg = false;
			try {
				mutFlg = mut.WaitOne();
				if (mutFlg) {
					Directory.CreateDirectory(LogDir);
					DateStr = DateTime.Now.ToString(DateFmt);
					LogFileName = Path.Combine(LogDir, DateStr+".log");
					fs = new FileStream(LogFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
					sw = new StreamWriter(fs);

					fs.Seek(0, SeekOrigin.End);
					sw.Write($"{BR}【プログラム起動】{BR}");
					sw.Write($"----*----*----*----*----*----*----*----*----*----*----{BR}");
					sw.Write($"{TB}Time：{TB}{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff}{BR}");
					sw.Write($"{TB}AppVer：{TB}{Cmn.AppVer}{BR}");
					sw.Write($"{TB}ProcName：{TB}{Cmn.ProcName}{BR}");
					sw.Write($"{TB}ProcId：{TB}{Cmn.ProcId}{BR}");
					sw.Write($"----*----*----*----*----*----*----*----*----*----*----{BR}{BR}");
					sw.Flush();
					_DelOldLog(LogDelDate);
					fLog = true;
				}
			} catch (Exception) {
				fLog = false;
			} finally {
				if (mutFlg) {
					mut.ReleaseMutex();
				}
			}
		}
		public static void InitLog(int delDate) {
			InitLog("yyyyMMdd", delDate);
		}


		public static void CheckReOpen() {
			if (DateStr == DateTime.Now.ToString(DateFmt)) {
				return;
			}
			bool mutFlg = false;
			try {
				mutFlg = mut.WaitOne();
				if (mutFlg) {
					DateStr= DateTime.Now.ToString(DateFmt);
					LogFileName = Path.Combine(LogDir, DateStr+".log");
					sw.Write($"ログファイル切り替え{BR}");
					fs.Close();
					fs = new FileStream(LogFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
					sw = new StreamWriter(fs);
					fs.Seek(0, SeekOrigin.End);
					sw.Write($"ログファイル切り替え{BR}");
					sw.Write($"----*----*----*----*----*----*----*----*----*----*----{BR}");
					sw.Write($"{TB}Time：{TB}{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff}{BR}");
					sw.Write($"{TB}AppVer：{TB}{Cmn.AppVer}{BR}");
					sw.Write($"{TB}ProcName：{TB}{Cmn.ProcName}{BR}");
					sw.Write($"{TB}ProcId：{TB}{Cmn.ProcId}{BR}");
					sw.Write($"----*----*----*----*----*----*----*----*----*----*----{BR}{BR}");
					sw.Flush();
					_DelOldLog(LogDelDate);
				}
			} catch (Exception) {
				fLog = false;
			} finally {
				if (mutFlg) {
					mut.ReleaseMutex();
				}
			}
		}
		public static void DelOldLog(int days = 0) {
			if (days==0) {
				return;
			}
			bool mutFlg = false;
			try {
				mutFlg = mut.WaitOne();
				if (mutFlg) {
					_DelOldLog(days);
				}
			} catch (Exception) {
				//何もしない
			} finally {
				if (mutFlg) {
					mut.ReleaseMutex();
				}
			}
		}

		private static void _DelOldLog(int days = 0) {
			if (days==0) {
				return;
			}
			try {
				string[] files = Directory.GetFiles(LogDir, "20??????*.log");
				foreach (string path in files) {
					string fileName = Path.GetFileName(path).Remove(DateFmt.Length);
					try {
						if (DateTime.TryParseExact(fileName, DateFmt, null, 0, out DateTime chkDate) && DateTime.Now.Date.AddDays(0-days)>chkDate) {
							File.Delete(path);
						}
					} catch (Exception) {
						//何もしない
					}
				}
			} catch (Exception) {
				//何もしない
			}
		}
		/// <summary>
		/// ログを保存する。(文字列)
		/// 引数として必要なのは、保存する文字列のみ
		/// </summary>
		/// <param name="s">保存する文字列</param>
		/// <param name="file">未設定でよい</param>
		/// <param name="line">未設定でよい</param>
		/// <param name="member">未設定でよい</param>
		public static void SaveLog(
			string s,
			LogDummy dummy = null,
			[CallerFilePath] string file = "",
			[CallerLineNumber] int line = 0,
			[CallerMemberName] string member = ""
		) {
			if (!fLog) {
				return;
			}
			CheckReOpen();
			bool mutFlg = false;
			try {
				mutFlg = mut.WaitOne();
				if (mutFlg) {
					fs.Seek(0, SeekOrigin.End);
					sw.Write($"{DateTime.Now:MM/dd HH:mm:ss.fff}{TB}{Path.GetFileName(file)}({line})");
					sw.Write($"{TB}{s.Replace($"{BR}", $"{BR}{TB}{TB}")}{BR}");
					sw.Flush();
				}
			} catch (Exception) {
			} finally {
				if (mutFlg) {
					mut.ReleaseMutex();
				}
			}
		}

		/// <summary>
		/// ログを保存する。(例外)
		/// 引数として必要なのは、保存する例外のみ
		/// </summary>
		/// <param name="ex">保存する例外</param>
		/// <param name="file">未設定でよい</param>
		/// <param name="line">未設定でよい</param>
		/// <param name="member">未設定でよい</param>
		public static void SaveLog(
			Exception ex,
			LogDummy dummy = null,
			[CallerFilePath] string file = "",
			[CallerLineNumber] int line = 0,
			[CallerMemberName] string member = ""
		) {
			if (!fLog) {
				return;
			}
			CheckReOpen();
			bool mutFlg = false;
			try {
				mutFlg = mut.WaitOne();
				if (mutFlg) {
					fs.Seek(0, SeekOrigin.End);
					sw.Write($"{DateTime.Now:MM/dd HH:mm:ss.fff}{TB}{Path.GetFileName(file)}({line}){TB}{member}{BR}");
					sw.Write($"{TB}EXP：{TB}{ex.ToString().Replace($"{BR}", $"{BR}{TB}{TB}")}{BR}");
					if (ex.InnerException != null) {
						sw.Write($"{TB}EXP1：{TB}{ex.InnerException.ToString().Replace($"{BR}", $"{BR}{TB}{TB}")}{BR}");
						if (ex.InnerException.InnerException != null) {
							sw.Write($"{TB}EXP2：{TB}{ex.InnerException.InnerException.ToString().Replace($"{BR}", $"{BR}{TB}{TB}")}{BR}");
						}
					}
					sw.Write($"{BR}");
					sw.Flush();
				}
			} catch (Exception) {
			} finally {
				if (mutFlg) {
					mut.ReleaseMutex();
				}
			}
		}

		/// <summary>
		/// ログを保存する。(文字列と例外)
		/// 引数として必要なのは、保存する文字列と例外のみ
		/// </summary>
		/// <param name="s">保存する文字列</param>
		/// <param name="ex">保存する例外</param>
		/// <param name="file">未設定でよい</param>
		/// <param name="line">未設定でよい</param>
		/// <param name="member">未設定でよい</param>
		public static void SaveLog(
			string s,
			Exception ex,
			LogDummy dummy = null,
			[CallerFilePath] string file = "",
			[CallerLineNumber] int line = 0,
			[CallerMemberName] string member = ""
		) {
			if (!fLog) {
				return;
			}
			CheckReOpen();
			bool mutFlg = false;
			try {
				mutFlg = mut.WaitOne();
				if (mutFlg) {
					fs.Seek(0, SeekOrigin.End);
					sw.Write($"{DateTime.Now:MM/dd HH:mm:ss.fff}{TB}{Path.GetFileName(file)}({line}){TB}{member}{BR}");
					sw.Write($"{TB}{TB}{s.Replace($"{BR}", $"{BR}{TB}{TB}")}{BR}");
					sw.Write($"{TB}EXP：{TB}{ex.ToString().Replace($"{BR}", $"{BR}{TB}{TB}")}{BR}");
					if (ex.InnerException != null) {
						sw.Write($"{TB}EXP1：{TB}{ex.InnerException.ToString().Replace($"{BR}", $"{BR}{TB}{TB}")}{BR}");
						if (ex.InnerException.InnerException != null) {
							sw.Write($"{TB}EXP2：{TB}{ex.InnerException.InnerException.ToString().Replace($"{BR}", $"{BR}{TB}{TB}")}{BR}");
						}
					}
					sw.Write($"{BR}");
					sw.Flush();
				}
			} catch (Exception) {
			} finally {
				if (mutFlg) {
					mut.ReleaseMutex();
				}
			}
		}

		/// <summary>
		/// ログを保存する。(SQLと例外)
		/// </summary>
		//public static void SaveLog(
		//	Exception ex,
		//	string sql,
		//	SqlParameter[] sqlParams,
		//	LogDummy dummy = null,
		//	[CallerFilePath] string file = "",
		//	[CallerLineNumber] int line = 0,
		//	[CallerMemberName] string member = ""
		//) {
		//	if (!fLog) {
		//		return;
		//	}
		//	CheckReOpen();
		//	bool mutFlg = false;
		//	try {
		//		mutFlg = mut.WaitOne();
		//		if (mutFlg) {
		//			fs.Seek(0, SeekOrigin.End);
		//			sw.Write($"{DateTime.Now:MM/dd HH:mm:ss.fff)}{TB}{Path.GetFileName(file)}({line}){TB}{member}{BR}");
		//			var exmes = Regex.Replace(ex.ToString(), @"^   場所 System.*\n?", "", RegexOptions.Multiline).Replace($"{BR}", $"{BR}{TB}{TB}");
		//			sw.Write($"{TB}EXP：{TB}{exmes}{BR}");
		//			if (ex is SqlException sqlexp) {
		//				foreach (SqlError sqlerr in sqlexp.Errors) {
		//					sw.Write($"{TB}SQLERR：{TB}{sqlerr.Number}：{sqlerr.Message.Replace($"{BR}", $"{BR}{TB}{TB}")}{BR}");
		//				}
		//			}
		//			sw.Write($"{TB}SQL：{TB}{sql.Replace($"{BR}", $"{BR}{TB}{TB}")}{BR}");
		//			if (sqlParams != null) {
		//				foreach (SqlParameter p in sqlParams) {
		//					sw.Write($"{TB}{TB}{p.ParameterName}：{p.Value.__ToStrF().Replace($"{BR}", $"{BR}{TB}{TB}")}{BR}");
		//				}
		//			}
		//			sw.Write($"{BR}");
		//			sw.Flush();
		//		}
		//	} catch (Exception) {
		//	} finally {
		//		if (mutFlg) {
		//			mut.ReleaseMutex();
		//		}
		//	}
		//}
	}   //	class Log
	public class LogDummy {

	}   //	class LogDummy
}//namespace TPCmn
