using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using static TPCmn.Stc;

namespace TPCmn {
	public static partial class NumHlp {
		public static bool _IsZero(this int? iz) {
			if (iz==null) {
				return true;
			}
			if (iz==0) {
				return true;
			}
			return false;
		}

		public static bool _IsntZero(this int? iz) {
			if (iz==null) {
				return false;
			}
			if (iz==0) {
				return false;
			}
			return true;
		}

		#region "オブジェクト→int"
		/// <summary>オブジェクトの数値検証</summary>
		public static bool __IsInt(this object o, out int i, int? defVal = null) {
			//    とりあえず返却値
			i = defVal??DEFAULT_INT;

			int? iz = o as int?;
			if (iz == null) {
				return false;
			}
			i = iz.Value;
			return true;
		}


		/// <summary>オブジェクトの数値検証</summary>
		public static bool __IsInt(this object o) {
			return __IsInt(o, out _);
		}

		/// <summary>オブジェクトの数値変換</summary>
		public static int __ToInt(this object o, int? defVal = null) {
			__IsInt(o, out int i, defVal);
			return i;
		}


		/// <summary>
		/// 強化版数値変換(オブジェクト用)
		/// <para>あらゆるobjectに対し、数値変換を実行可能か検証する</para>
		/// </summary>
		/// <param name="o">何でもよい　nullでもOK(null→false)</param>
		/// <returns></returns>
		public static bool __IsIntF(this object o, out int i, int? defVal = null) {
			//    とりあえず返却値
			i = defVal??DEFAULT_INT;
			if (o == null || o is DBNull) {
				return false;       // null
			}
			if (o is int iz) {
				i = iz;
				return true;
			}
			if (o is uint ui) {
				i = (int)ui;
				return (uint)i==ui;
			}
			if (o is bool bl) {
				i=bl ? 1 : 0;
				return true;
			}
			if (o is byte b) {
				i = (int)b;
				return true;
			}
			if (o is short sh) {
				i = sh;
				return true;
			}
			if (o is ushort ush) {
				i = ush;
				return true;
			}
			if (o is long l) {
				i = (int)l;
				return (long)i==l;
			}
			if (o is ulong ul) {
				i = (int)ul;
				return (ulong)i==ul;
			}
			if (o is float fl) {
				i = (int)fl;
				return (float)i==fl;
			}
			if (o is double db) {
				i = (int)db;
				return (double)i==db;
			}
			if (o is decimal mn) {
				i = (int)mn;
				return (decimal)i==mn;
			}
			if (o is string s) {
				return s._IsInt(out i, defVal);       // 数値じゃない文字列
			}
			i = defVal??DEFAULT_INT;
			return false;
		}

		/// <summary>オブジェクトの数値検証</summary>
		public static bool __IsIntF(this object o) {
			return __IsIntF(o, out _);
		}

		/// <summary>オブジェクトの数値検証</summary>
		public static bool __IsIntN(this object o, out int? iz) {
			if (o == null || o is DBNull) {
				iz = null;
				return true;       // null
			}
			bool ret;
			ret = __IsIntF(o, out int i);
			iz = i;
			return ret;
		}

		/// <summary>オブジェクトの数値検証</summary>
		public static bool __IsIntN(this object o) {
			return __IsIntN(o, out _);
		}

		/// <summary>オブジェクトの数値変換</summary>
		public static int __ToIntF(this object o, int? defVal = null) {
			__IsIntF(o, out int i, defVal);
			return i;
		}

		/// <summary>オブジェクトの数値変換</summary>
		public static int? __ToIntN(this object o) {
			__IsIntN(o, out int? iz);
			return iz;
		}
		#endregion

		#region "文字列→int"
		/// <summary>文字列の数値変換</summary>
		public static bool _IsInt(this string s, out int i, int? defVal = null) {
			//    とりあえず返却値
			i = defVal??DEFAULT_INT;

			if (s == null) {
				return false;       // null
			}

			if (int.TryParse(s, out i)) {
				return true;    // 数値になる文字列
			}
			s = Strings.StrConv(s, VbStrConv.Narrow);
			s = s.Replace("\\", "").Replace(",", "");
			if (int.TryParse(s, out i)) {
				return true;    // 頑張れば数値になる文字列
			}
			i = defVal??DEFAULT_INT;
			return false;       // 数値じゃない文字列
		}

		/// <summary>文字列の数値変換</summary>
		public static bool _IsInt(this string s) {
			return _IsInt(s, out _);
		}

		/// <summary>文字列の数値変換</summary>
		public static int _ToInt(this string s, int? defVal = null) {
			_IsInt(s, out int i, defVal);
			return i;
		}
		public static int? _ToIntN(this string s) {
			if (_IsInt(s, out int i)) {
				return i;
			}
			return null;
		}
		#endregion



		#region "オブジェクト→decimal"
		public static bool __IsDec(this object o, out decimal d, decimal? defVal = null) {
			//    とりあえず返却値
			d = defVal??DEFAULT_INT;

			decimal? dn = o as decimal?;
			if (dn == null) {
				return false;
			}
			d = dn.Value;
			return true;
		}


		/// <summary>オブジェクトの日時検証</summary>
		public static bool __IsDec(this object o) {
			return __IsDec(o, out _);
		}

		/// <summary>オブジェクトの数値変換</summary>
		public static decimal __ToDec(this object o, decimal? defVal = null) {
			__IsDec(o, out decimal d, defVal);
			return d;
		}


		/// <summary>
		/// 強化版数値変換(オブジェクト用)
		/// <para>あらゆるobjectに対し、数値変換を実行可能か検証する</para>
		/// </summary>
		/// <param name="o">何でもよい　nullでもOK(null→false)</param>
		/// <returns></returns>
		public static bool __IsDecF(this object o, out decimal d, decimal? defVal = null) {
			d = defVal??DEFAULT_INT;
			if (o == null || o is DBNull) {
				return false;       // null
			}
			if (o is decimal mn) {
				d = mn;
				return true;
			}
			if (o is int iz) {
				d = iz;
				return true;
			}
			if (o is uint ui) {
				d = ui;
				return true;
			}
			if (o is bool bl) {
				d=bl ? 1 : 0;
				return true;
			}
			if (o is byte b) {
				d = b;
				return true;
			}
			if (o is short sh) {
				d = sh;
				return true;
			}
			if (o is ushort ush) {
				d = ush;
				return true;
			}
			if (o is long l) {
				d = l;
				return true;
			}
			if (o is ulong ul) {
				d = ul;
				return true;
			}
			if (o is float fl) {
				d = (decimal)fl;
				return (float)d==fl;
			}
			if (o is double db) {
				d = (decimal)db;
				return (double)d==db;
			}
			string s = o as string;
			if ((o as string) != null) {
				if (decimal.TryParse(s, out d)) {
					return true;    // 数値になる文字列
				}
				s = Strings.StrConv(s, VbStrConv.Narrow);
				s = s.Replace("\\", "").Replace(",", "");
				if (decimal.TryParse(s, out d)) {
					return true;    // 頑張れば数値になる文字列
				}
				d = defVal??DEFAULT_INT;
				return false;       // 数値じゃない文字列
			}
			return false;
		}

		/// <summary>オブジェクトの数値検証</summary>
		public static bool __IsDecF(this object o) {
			return __IsDecF(o, out _);
		}

		/// <summary>オブジェクトの数値検証</summary>
		public static bool __IsDecN(this object o, out decimal? dn) {

			if (o is decimal?) {
				dn = o as decimal?;
				return true;
			}
			if (__IsDecF(o, out decimal d)) {
				dn = d;
				return true;
			}
			dn = null;
			return false;
		}

		/// <summary>オブジェクトの数値検証</summary>
		public static bool __IsDecN(this object o) {
			return __IsDecN(o, out _);
		}

		/// <summary>オブジェクトの数値変換</summary>
		public static decimal __ToDecF(this object o, decimal? defVal = null) {
			__IsDecF(o, out decimal d, defVal);
			return d;
		}

		/// <summary>オブジェクトの数値変換</summary>
		public static decimal? __ToDecN(this object o) {
			__IsDecN(o, out decimal? dn);
			return dn;
		}
		#endregion

		/// <summary>
		/// 強化版数値変換(オブジェクト用)
		/// <para>あらゆるobjectに対し、数値変換を実行可能か検証する</para>
		/// </summary>
		/// <param name="o">何でもよい　nullでもOK(null→false)</param>
		/// <returns></returns>
		public static bool __IsDblF(this object o, out double d, double? defVal = null) {
			d = defVal??DEFAULT_INT;
			if (o == null || o is DBNull) {
				return false;       // null
			}
			if (o is double db) {
				d = db;
				return true;
			}
			if (o is int iz) {
				d = iz;
				return true;
			}
			if (o is uint ui) {
				d = ui;
				return true;
			}
			if (o is bool bl) {
				d=bl ? 1 : 0;
				return true;
			}
			if (o is byte b) {
				d = b;
				return true;
			}
			if (o is short sh) {
				d = sh;
				return true;
			}
			if (o is ushort ush) {
				d = ush;
				return true;
			}
			if (o is long l) {
				d = l;
				return true;
			}
			if (o is ulong ul) {
				d = ul;
				return true;
			}
			if (o is float fl) {
				d = (double)fl;
				return (float)d==fl;
			}
			if (o is decimal mn) {
				d = (double)mn;
				return (decimal)d==mn;
			}
			string s = o as string;
			if ((o as string) != null) {
				if (double.TryParse(s, out d)) {
					return true;    // 数値になる文字列
				}
				s = Strings.StrConv(s, VbStrConv.Narrow);
				s = s.Replace("\\", "").Replace(",", "");
				if (double.TryParse(s, out d)) {
					return true;    // 頑張れば数値になる文字列
				}
				d = defVal??DEFAULT_INT;
				return false;       // 数値じゃない文字列
			}
			return false;
		}

		/// <summary>オブジェクトの数値変換</summary>
		public static double __ToDblF(this object o, double? defVal = null) {
			__IsDblF(o, out double d, defVal);
			return d;
		}
		/// <summary>オブジェクトの数値検証</summary>
		public static bool __IsDblN(this object o, out double? dn) {

			if (o is double?) {
				dn = o as double?;
				return true;
			}
			if (__IsDblF(o, out double d)) {
				dn = d;
				return true;
			}
			dn = null;
			return false;
		}
		/// <summary>オブジェクトの数値変換</summary>
		public static double? __ToDblN(this object o) {
			__IsDblN(o, out double? dn);
			return dn;
		}

	}
}
