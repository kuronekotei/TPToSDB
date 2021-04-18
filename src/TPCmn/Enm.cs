using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPCmn {
	//	列挙型で許容される型は、byte、sbyte、short、ushort、int、uint、long、ulong
	//	デフォルトはint
	//	列挙型をToStringすると、名前が取れます　//E_EraJ.大正.ToString() → "大正"
	//	nameofでも名前が取れます　//nameof(E_EraJ.大正) → "大正"

	//	属性と拡張メソッドの組み合わせで、追加のテキストが設定できます
	//	E_EraJ.明治.GetText1() → "M"
	/// <summary>
	/// 元号を表す(サンプル)
	/// </summary>
	public enum E_EraJ {
		[EnumText1(@"M")]
		明治 = 0,
		[EnumText1(@"T")]
		大正,
		[EnumText1(@"S")]
		昭和,
		[EnumText1(@"H")]
		平成,
		[EnumText1(@"R")]
		令和
	}

	/// <summary>
	/// 元号を表す
	/// </summary>
	public enum E_EraE {
		M = 0,
		T,
		S,
		H,
		R
	}

	/// <summary>
	/// 曜日
	/// </summary>
	public enum E_WeekDayJ {
		日 = 0,
		月,
		火,
		水,
		木,
		金,
		土,
	}

	[AttributeUsage(AttributeTargets.Field)]
	public class EnumText1 : Attribute {
		public EnumText1(string text) {
			_text = text;
		}
		private readonly string _text;
		public string Text { get { return _text; } }
	}

	[AttributeUsage(AttributeTargets.Field)]
	public class EnumText2 : Attribute {
		public EnumText2(string text) {
			_text = text;
		}
		private readonly string _text;
		public string Text { get { return _text; } }
	}


	public static class EnumExtension {
		private static readonly Dictionary<Enum, string> text1Cache = new Dictionary<Enum, string>();

		public static string GetText1(this Enum en) {
			if (en == null) {
				return null;
			}
			if (text1Cache.ContainsKey(en)) {
				return text1Cache[en];
			}
			var enumType = en.GetType();
			if (enumType == null) { return null; }
			var name = Enum.GetName(enumType, en);
			if (name == null) { return null; }
			var attr = enumType.GetField(name).GetCustomAttributes(typeof(EnumText1), true);
			if ((attr == null) || (attr.Length < 1)) {
				text1Cache.Add(en, name);
			} else {
				text1Cache.Add(en, ((EnumText1)attr[0]).Text);
			}
			return text1Cache[en];
		}

		private static readonly Dictionary<Enum, string> text2Cache = new Dictionary<Enum, string>();

		public static string GetText2(this Enum en) {
			if (en == null) { return null; }
			if (text2Cache.ContainsKey(en)) {
				return text2Cache[en];
			}
			var enumType = en.GetType();
			if (enumType == null) { return null; }
			var name = Enum.GetName(enumType, en);
			if (name == null) { return null; }
			var attr = enumType.GetField(name).GetCustomAttributes(typeof(EnumText2), true);
			if ((attr == null) || (attr.Length < 1)) {
				text2Cache.Add(en, name);
			} else {
				text2Cache.Add(en, ((EnumText2)attr[0]).Text);
			}
			return text2Cache[en];

		}
	}
}
