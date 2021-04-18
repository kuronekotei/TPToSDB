using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static TPCmn.Stc;

namespace TPCmn {
	public static partial class DateHlp {
		#region "文字列→日付"

		/// <summary>
		/// <para>日付検証と変換を同時に行う</para>
		/// <para>時刻を含むものはエラーとなる</para>
		/// </summary>
		/// <param name="s">変換元</param>
		/// <param name="format">フォーマット(DateTime.ToStringのもの)</param>
		/// <param name="dt">変換済日付　エラー時はDateTime.MinValue</param>
		/// <returns>成功可否</returns>
		public static bool _IsDate(this string s, string format, out DateTime dt) {
			//    とりあえず返却値
			var defVal = DateTime.MinValue;
			dt = defVal;
			if (string.IsNullOrWhiteSpace(s)) {
				return false;   //    文字列が空
			}
			if (format != null) {
				if (!DateTime.TryParseExact(s, format, null, 0, out dt)) {
					dt = defVal;
					return false;   //    フォーマットエラー
				}
			} else {
				if (!DateTime.TryParse(s, out dt)) {
					dt = defVal;
					return false;   //    フォーマットエラー
				}
			}

			if (dt != dt.Date) {
				return false;   //    時間を含む
			}

			return true;
		}


		/// <summary>
		/// <para>StrIsDate(String s,String format,out DateTime dt)のエイリアス</para>
		/// <para>日付検証のみ</para>
		/// </summary>
		public static bool _IsDate(this string s, string format) {
			return s._IsDate(format, out _);
		}

		/// <summary>
		/// <para>StrIsDate(String s,String format,out DateTime dt)のエイリアス</para>
		/// <para>日付検証のみ　DATE_FORMAT固定</para>
		/// </summary>
		public static bool _IsDate(this string s) {
			return s._IsDate(DATE_CHECK, out _);
		}

		/// <summary>
		/// <para>StrIsDate(String s,String format,out DateTime dt)のエイリアス</para>
		/// <para>DATE_FORMAT固定</para>
		/// </summary>
		public static bool _IsDate(this string s, out DateTime dt) {
			return s._IsDate(DATE_CHECK, out dt);
		}

		/// <summary>
		/// <para>StrIsDate(String s,String format,out DateTime dt)のエイリアス</para>
		/// <para>日付を返却する(失敗時DateTime.MinValue)　formatは自動判別</para>
		/// </summary>
		public static DateTime _ToDate(this string s) {
			s._IsDate(null, out DateTime dt);
			return dt;
		}

		/// <summary>
		/// <para>StrIsDate(String s,String format,out DateTime dt)のエイリアス</para>
		/// <para>日付を返却する(失敗時DateTime.MinValue)</para>
		/// </summary>
		public static DateTime _ToDate(this string s, string format) {
			s._IsDate(format, out DateTime dt);
			return dt;
		}

		/// <summary>
		/// <para>StrIsDate(String s,String format,out DateTime dt)のエイリアス</para>
		/// <para>日付を返却する(失敗時null)　formatは自動判別</para>
		/// </summary>
		public static DateTime? _ToDateN(this string s) {
			if (s._IsDate(null, out DateTime dt)) {
				return dt;
			}
			return null;
		}

		/// <summary>
		/// <para>StrIsDate(String s,String format,out DateTime dt)のエイリアス</para>
		/// <para>日付を返却する(失敗時null)</para>
		/// </summary>
		public static DateTime? _ToDateN(this string s, string format) {
			if (s._IsDate(format, out DateTime dt)) {
				return dt;
			}
			return null;
		}

		/// <summary>
		/// <para>日付検証と変換を同時に行う</para>
		/// <para>時刻を含むものはエラーとなる</para>
		/// </summary>
		/// <param name="i">変換元</param>
		/// <param name="format">フォーマット(DateTime.ToStringのもの)</param>
		/// <param name="dt">変換済日付　エラー時はDateTime.MinValue</param>
		/// <returns>成功可否</returns>
		public static bool _IsDate(this int i, out DateTime dt) {
			//    とりあえず返却値
			string s = i.ToString(INTOFDATE_FORMAT);
			return s._IsDate(DATEOFINT_FORMAT, out dt);
		}


		/// <summary>
		/// <para>日付検証と変換を同時に行う</para>
		/// <para>時刻を含むものはエラーとなる</para>
		/// </summary>
		/// <param name="i">変換元</param>
		/// <param name="format">フォーマット(DateTime.ToStringのもの)</param>
		/// <param name="dt">変換済日付　エラー時はDateTime.MinValue</param>
		/// <returns>成功可否</returns>
		public static bool _IsDate(this int i, string format1=null, string format2 = null) {
			//    とりあえず返却値
			string s = i.ToString(format1??INTOFDATE_FORMAT);
			return s._IsDate(format2??DATEOFINT_FORMAT, out _);
		}

		/// <summary>
		/// <para>StrIsDate(String s,String format,out DateTime dt)のエイリアス</para>
		/// <para>日付を返却する(失敗時DateTime.MinValue)</para>
		/// </summary>
		public static DateTime _ToDate(this int i) {
			i._IsDate(out DateTime dt);
			return dt;
		}

		/// <summary>
		/// <para>StrIsDate(String s,String format,out DateTime dt)のエイリアス</para>
		/// <para>日付を返却する(失敗時null)</para>
		/// </summary>
		public static DateTime? _ToDateN(this int i) {
			if (i._IsDate(out DateTime dt)) {
				return dt;
			}
			return null;
		}

		#endregion



		#region "文字列→日時"

		/// <summary>
		/// <para>日時検証と変換を同時に行う</para>
		/// </summary>
		/// <param name="s">変換元</param>
		/// <param name="format">フォーマット(DateTime.ToStringのもの)</param>
		/// <param name="dt">変換済日付　エラー時はDateTime.MinValue</param>
		/// <returns>成功可否</returns>
		public static bool _IsTime(this string s, string format, out DateTime dt) {
			//    とりあえず返却値
			var defVal = DateTime.MinValue;
			dt = defVal;
			if (string.IsNullOrWhiteSpace(s)) {
				return false;   //    文字列が空
			}
			if (format != null) {
				if (!DateTime.TryParseExact(s, format, null, 0, out dt)) {
					dt = defVal;
					return false;   //    フォーマットエラー
				}
			} else {
				if (!DateTime.TryParse(s, out dt)) {
					dt = defVal;
					return false;   //    フォーマットエラー
				}
			}

			return true;
		}

		/// <summary>
		/// <para>StrIsDate(String s,String format,out DateTime dt)のエイリアス</para>
		/// <para>日時検証のみ</para>
		/// </summary>
		public static bool _IsTime(this string s, string format) {
			return s._IsTime(format, out _);
		}

		/// <summary>
		/// <para>StrIsDate(String s,String format,out DateTime dt)のエイリアス</para>
		/// <para>日時検証のみ　DATE_FORMAT固定</para>
		/// </summary>
		public static bool _IsTime(this string s) {
			return s._IsTime(TIME_CHECK, out _);
		}

		/// <summary>
		/// <para>StrIsDate(String s,String format,out DateTime dt)のエイリアス</para>
		/// <para>TIME_FORMAT固定</para>
		/// </summary>
		public static bool _IsTime(this string s, out DateTime dt) {
			return s._IsTime(TIME_CHECK, out dt);
		}

		/// <summary>
		/// <para>StrIsDate(String s,String format,out DateTime dt)のエイリアス</para>
		/// <para>日時を返却する(失敗時DateTime.MinValue)　formatは自動判別</para>
		/// </summary>
		public static DateTime _ToTime(this string s) {
			s._IsTime(null, out DateTime dt);
			return dt;
		}

		/// <summary>
		/// <para>StrIsDate(String s,String format,out DateTime dt)のエイリアス</para>
		/// <para>日時を返却する(失敗時DateTime.MinValue)</para>
		/// </summary>
		public static DateTime _ToTime(this string s, string format) {
			s._IsTime(format, out DateTime dt);
			return dt;
		}

		/// <summary>
		/// <para>StrIsDate(String s,String format,out DateTime dt)のエイリアス</para>
		/// <para>日時を返却する(失敗時null)　formatは自動判別</para>
		/// </summary>
		public static DateTime? _ToTimeN(this string s) {
			if (s._IsTime(null, out DateTime dt)) {
				return dt;
			}
			return null;
		}

		/// <summary>
		/// <para>StrIsDate(String s,String format,out DateTime dt)のエイリアス</para>
		/// <para>日時を返却する(失敗時null)</para>
		/// </summary>
		public static DateTime? _ToTimeN(this string s, string format) {
			if (s._IsTime(format, out DateTime dt)) {
				return dt;
			}
			return null;
		}

		#endregion



		#region "文字列→年月"

		/// <summary>
		/// <para>年月検証と変換を同時に行う</para>
		/// </summary>
		/// <param name="s">変換元</param>
		/// <param name="format">フォーマット(DateTime.ToStringのもの)</param>
		/// <param name="dt">変換済日付　エラー時はDateTime.MinValue</param>
		/// <returns>成功可否</returns>
		public static bool _IsMonth(this string s, string format, out DateTime dt) {
			//    とりあえず返却値
			var defVal = DateTime.MinValue;
			dt = defVal;
			if (string.IsNullOrWhiteSpace(s)) {
				return false;   //    文字列が空
			}
			if (format != null) {
				if (!DateTime.TryParseExact(s, format, null, 0, out dt)) {
					dt = defVal;
					return false;   //    フォーマットエラー
				}
			} else {
				if (!DateTime.TryParse(s, out dt)) {
					dt = defVal;
					return false;   //    フォーマットエラー
				}
			}

			return true;
		}

		/// <summary>
		/// <para>StrIsDate(String s,String format,out DateTime dt)のエイリアス</para>
		/// <para>年月検証のみ</para>
		/// </summary>
		public static bool _IsMonth(this string s, string format) {
			return s._IsMonth(format, out _);
		}

		/// <summary>
		/// <para>StrIsDate(String s,String format,out DateTime dt)のエイリアス</para>
		/// <para>年月検証のみ　DATE_FORMAT固定</para>
		/// </summary>
		public static bool _IsMonth(this string s) {
			return s._IsMonth(MONTH_CHECK, out _);
		}

		/// <summary>
		/// <para>StrIsDate(String s,String format,out DateTime dt)のエイリアス</para>
		/// <para>MONTH_FORMAT固定</para>
		/// </summary>
		public static bool _IsMonth(this string s, out DateTime dt) {
			return s._IsMonth(MONTH_CHECK, out dt);
		}

		/// <summary>
		/// <para>StrIsDate(String s,String format,out DateTime dt)のエイリアス</para>
		/// <para>年月を返却する(失敗時DateTime.MinValue)　formatは自動判別</para>
		/// </summary>
		public static DateTime _ToMonth(this string s) {
			s._IsMonth(null, out DateTime dt);
			return dt;
		}

		/// <summary>
		/// <para>StrIsDate(String s,String format,out DateTime dt)のエイリアス</para>
		/// <para>年月を返却する(失敗時DateTime.MinValue)</para>
		/// </summary>
		public static DateTime _ToMonth(this string s, string format) {
			s._IsMonth(format, out DateTime dt);
			return dt;
		}

		/// <summary>
		/// <para>StrIsDate(String s,String format,out DateTime dt)のエイリアス</para>
		/// <para>年月を返却する(失敗時null)　formatは自動判別</para>
		/// </summary>
		public static DateTime? _ToMonthN(this string s) {
			if (s._IsMonth(null, out DateTime dt)) {
				return dt;
			}
			return null;
		}

		/// <summary>
		/// <para>StrIsDate(String s,String format,out DateTime dt)のエイリアス</para>
		/// <para>年月を返却する(失敗時null)</para>
		/// </summary>
		public static DateTime? _ToMonthN(this string s, string format) {
			if (s._IsMonth(format, out DateTime dt)) {
				return dt;
			}
			return null;
		}

		#endregion



		#region "オブジェクト→日時"

		/// <summary>オブジェクトの日時検証</summary>
		public static bool __IsTime(this object o, out DateTime dt) {
			//    とりあえず返却値
			dt = DateTime.MinValue;

			DateTime? dtn = o as DateTime?;
			if (dtn == null) {
				return false;
			}
			dt = dtn.Value;
			return true;
		}

		/// <summary>オブジェクトの日時検証</summary>
		public static bool __IsTime(this object o) {
			return __IsTime(o, out _);
		}

		/// <summary>オブジェクトの日時検証</summary>
		public static bool __IsTimeN(this object o, out DateTime? dtn) {
			dtn = o as DateTime?;
			if (dtn == null) {
				return false;
			}
			return true;
		}

		/// <summary>オブジェクトの日時検証</summary>
		public static bool __IsTimeN(this object o) {
			return __IsTimeN(o, out _);
		}

		/// <summary>オブジェクトの日時変換</summary>
		public static DateTime __ToTime(this object o) {
			__IsTime(o, out DateTime dt);
			return dt;
		}

		/// <summary>オブジェクトの日時変換</summary>
		public static DateTime? __ToTimeN(this object o) {
			__IsTimeN(o, out DateTime? dtn);
			return dtn;
		}

		#endregion



		#region "オブジェクト→日時/日付/年月→文字列/数値"

		/// <summary>オブジェクトの日付文字列変換</summary>
		public static string __ToStrOfDate(this object o, string format) {
			if (o.__IsTime(out DateTime dt)) {
				return dt.Date.ToString(format);
			}
			return string.Empty;
		}

		/// <summary>オブジェクトの日付文字列変換</summary>
		public static string __ToStrOfDate(this object o) {
			if (o.__IsTime(out DateTime dt)) {
				return dt.Date.ToString(DATE_FORMAT);
			}
			return string.Empty;
		}

		/// <summary>オブジェクトの日付文字列変換</summary>
		public static int __ToIntOfDate(this object o) {
			if (o.__IsTime(out DateTime dt)) {
				return dt.Date.ToString(DATEOFINT_FORMAT).__ToInt();
			}
			return 0;
		}

		/// <summary>オブジェクトの日時文字列変換</summary>
		public static string __ToStrOfTime(this object o, string format) {
			if (o.__IsTime(out DateTime dt)) {
				return dt.ToString(format);
			}
			return string.Empty;
		}

		/// <summary>オブジェクトの日時文字列変換</summary>
		public static string __ToStrOfTime(this object o) {
			if (o.__IsTime(out DateTime dt)) {
				return dt.ToString(TIME_FORMAT);
			}
			return string.Empty;
		}

		/// <summary>オブジェクトの年月文字列変換</summary>
		public static string __ToStrOfMonth(this object o, string format) {
			if (o.__IsTime(out DateTime dt)) {
				return dt._ToMonth().ToString(format);
			}
			return string.Empty;
		}

		/// <summary>オブジェクトの年月文字列変換</summary>
		public static string __ToStrOfMonth(this object o) {
			if (o.__IsTime(out DateTime dt)) {
				return dt._ToMonth().ToString(MONTH_FORMAT);
			}
			return string.Empty;
		}

		#endregion

		#region "日付操作"

		/// <summary>
		/// 日付を　当日　0時0分0秒にする
		/// </summary>
		/// <param name="dt">ある時間</param>
		/// <returns>DateTime　当月1日</returns>
		public static DateTime _ToTomorrow(this DateTime dt) {
			return dt.Date.AddDays(1);
		}

		/// <summary>
		/// 日付を　前日　0時0分0秒にする
		/// </summary>
		/// <param name="dt">ある時間</param>
		/// <returns>DateTime　翌月1日</returns>
		public static DateTime _ToYesterday(this DateTime dt) {
			return dt.Date.AddDays(-1);
		}

		/// <summary>
		/// 日付を　当月1日　0時0分0秒にする
		/// </summary>
		/// <param name="dt">ある時間</param>
		/// <returns>DateTime　当月1日</returns>
		public static DateTime _ToMonth(this DateTime dt) {
			return new DateTime(dt.Year, dt.Month, 1);
		}

		/// <summary>
		/// 日付を　翌月1日　0時0分0秒にする
		/// </summary>
		/// <param name="dt">ある時間</param>
		/// <returns>DateTime　翌月1日</returns>
		public static DateTime _ToNextMonth(this DateTime dt) {
			return new DateTime(dt.Year, dt.Month, 1).AddMonths(1);
		}

		/// <summary>
		/// 日付を　当月最終日　0時0分0秒にする
		/// </summary>
		/// <param name="dt">ある時間</param>
		/// <returns>DateTime　当月1日</returns>
		public static DateTime _ToMonthEnd(this DateTime dt) {
			return new DateTime(dt.Year, dt.Month, 1).AddMonths(1).AddDays(-1);
		}

		/// <summary>
		/// 日時から年月日時分を取得　(0秒になる)
		/// </summary>
		/// <param name="dt">ある時間</param>
		/// <returns>DateTime　当月1日</returns>
		public static DateTime _ToMinits(this DateTime dt) {
			return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
		}

		/// <summary>
		/// 日時から年月日時分を取得　(0秒になる)
		/// </summary>
		/// <param name="dt">ある時間</param>
		/// <returns>DateTime　当月1日</returns>
		public static DateTime _Slice5Min(this DateTime dt) {
			return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, (dt.Minute/5)*5, 0);
		}
		#endregion
	}
}
