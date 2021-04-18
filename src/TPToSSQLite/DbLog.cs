using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using static TPCmn.Stc;
using static TPCmn.Log;
using System.Text.RegularExpressions;

namespace TPCmn {
	public static class DbLog {
		public static void SaveLog(
			Exception ex,
			string sql,
			SQLiteParameter[] sqlParams,
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
					sw.Write($"{DateTime.Now:MM/dd HH:mm:ss.fff)}{TB}{Path.GetFileName(file)}({line}){TB}{member}{BR}");
					var exmes = Regex.Replace(ex.ToString(), @"^   場所 System.*\n?", "", RegexOptions.Multiline).Replace($"{BR}", $"{BR}{TB}{TB}");
					sw.Write($"{TB}EXP：{TB}{exmes}{BR}");
					if (ex is SQLiteException sqlexp) {
						sw.Write($"{TB}SQLERR：{TB}{sqlexp.ErrorCode}：{sqlexp.Message.Replace($"{BR}", $"{BR}{TB}{TB}")}{BR}");
						if (sqlexp.InnerException is SQLiteException iex) {
							sw.Write($"{TB}SQLERR：{TB}{iex.ErrorCode}：{iex.Message.Replace($"{BR}", $"{BR}{TB}{TB}")}{BR}");

						}
					}
					sw.Write($"{TB}SQL：{TB}{sql.Replace($"{BR}", $"{BR}{TB}{TB}")}{BR}");
					if (sqlParams != null) {
						foreach (SQLiteParameter p in sqlParams) {
							sw.Write($"{TB}{TB}{p.ParameterName}：{p.Value.__ToStrF().Replace($"{BR}", $"{BR}{TB}{TB}")}{BR}");
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
	}
}
