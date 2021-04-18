using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static TPCmn.Stc;

namespace TPCmn {
	public static partial class DicHlp {
		#region "Dictionaryより値取得"

		/// <summary>
		/// Dictionaryより値取得
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dic"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public static T _GetVal<T>(this Dictionary<string, T> dic, string key) {
			if (key == null) {
				return default;
			}
			dic.TryGetValue(key, out T value);
			return value;
		}

		/// <summary>
		/// Dictionaryより値取得
		/// </summary>
		/// <param name="dic"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public static string _GetVal(this Dictionary<string, string> dic, string key) {
			if (key == null) {
				return null;
			}
			dic.TryGetValue(key, out string value);
			return value;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="dic"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public static string _GetValF(this Dictionary<string, string> dic, string key) {
			if (key == null) {
				return "";
			}
			dic.TryGetValue(key, out string value);
			if (value == null) {
				return "";
			}
			return value;
		}

		/// <summary>
		/// Dictionaryより値取得
		/// </summary>
		/// <param name="dic"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public static string _GetVal(this Dictionary<string, string> dic, object key) {
			if (key is not string str) {
				return null;
			}
			dic.TryGetValue(str, out string value);
			return value;
		}

		/// <summary>
		/// Dictionaryより値取得
		/// </summary>
		/// <param name="dic"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public static string _GetValF(this Dictionary<string, string> dic, object key) {
			if (key is not string str) {
				return null;
			}
			dic.TryGetValue(str, out string value);
			if (value == null) {
				return "";
			}
			return value;
		}
		#endregion
	}
}
