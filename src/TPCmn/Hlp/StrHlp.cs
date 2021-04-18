using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using static TPCmn.Stc;

namespace TPCmn {
	public static partial class StrHlp {
		/// <summary>
		/// 強化版ToString(オブジェクト用)
		/// <para>StringオブジェクトのみStringにする</para>
		/// <para>int,null等はnullを返却する</para>
		/// </summary>
		/// <param name="o">Stringオブジェクト　nullでもOK(null→null)</param>
		/// <returns></returns>
		public static string __ToStr(this object o) {
			return o as string;
		}

		/// <summary>
		/// 強化版ToString(オブジェクト用)
		/// <para>絶対NULLにならない　NULL参照例外も出さない</para>
		/// <para>あらゆるobjectに対し、ToStringを実行する</para>
		/// <para>objectがnullの時は　String.Empty</para>
		/// </summary>
		/// <param name="o">何でもよい　nullでもOK(null→String.Empty)</param>
		/// <returns></returns>
		public static string __ToStrF(this object o) {
			if (o == null || o is DBNull) {
				return string.Empty;    // null
			}
			if (o is string s) {
				return s;
			}
			return o.ToString();
		}

		public static string __ToCSVCol(this object o, bool preComma = true) {
			string pre = preComma ? "," : "";
			if (o == null || o is DBNull) {
				return pre; // null
			}
			return pre+"\""+o.ToString().Replace("\"", "\"\"")+"\"";
		}

		public static string _ToCSVCol(this string s, bool preComma = true) {
			string pre = preComma ? "," : "";
			if (s == null) {
				return pre; // null
			}
			return pre+"\""+s.Replace("\"", "\"\"")+"\"";
		}

		public static bool _IsEmp(this string s) {
			return string.IsNullOrEmpty(s);
		}
		public static bool _IsntEmp(this string s) {
			return !string.IsNullOrEmpty(s);
		}
		public static bool _IsSpc(this string s) {
			return string.IsNullOrWhiteSpace(s);
		}
		public static bool _IsntSpc(this string s) {
			return !string.IsNullOrWhiteSpace(s);
		}
		/// <summary>Regex.Match</summary>
		public static bool _Mtc(this string s, string pattern) {
			return Regex.Match(s, pattern).Success;
		}

		/// <summary>Regex.Replace</summary>
		public static string _Rpl(this string s, string pattern, string replace) {
			return Regex.Replace(s, pattern, replace);
		}

		public static string _PadRSJIS(this string s, int len, char fill = ' ') {
			return s.PadRight(len-Encoding.GetEncoding(932).GetByteCount(s)+s.Length, fill);
		}

	}
}
