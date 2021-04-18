using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static TPCmn.Stc;

namespace TPCmn {
	public static partial class InHlp {
		public static bool _In(this int tgt, params int[] items) {
			return IntIn(tgt, items);
		}
		public static bool IntIn(int tgt, params int[] items) {
			foreach (int item in items) {
				if (tgt==item) {
					return true;
				}
			}
			return false;
		}
		public static bool _In(this string tgt, params string[] items) {
			return StrIn(tgt, false, items);
		}
		public static bool _In(this string tgt, bool ignoreCase = false, params string[] items) {
			return StrIn(tgt, ignoreCase, items);
		}
		public static bool StrIn(string tgt, bool ignoreCase = false, params string[] items) {
			StringComparison c = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
			foreach (string item in items) {
				if (tgt?.IndexOf(item, c) != -1) {
					return true;
				}
			}
			return false;
		}
	}
}
