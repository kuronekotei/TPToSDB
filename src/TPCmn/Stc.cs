using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPCmn {
	public static class Stc {
		public static readonly string TB = "\t";
		public static readonly string BR = "\r\n";

		public static readonly long KiB = 1024;
		public static readonly long MiB = 1024*1024;
		public static readonly long GiB = 1024*1024*1024;


		/// <summary>デフォルト日付フォーマット</summary>
		public static readonly string DATE_FORMAT = @"yyyy/MM/dd";
		/// <summary>デフォルト日付フォーマット</summary>
		public static readonly string DATE_CHECK = @"yyyy/M/d";
		/// <summary>デフォルト日付フォーマット</summary>
		public static readonly string DATEOFINT_FORMAT = @"yyyyMMdd";
		/// <summary>デフォルト日付フォーマット</summary>
		public static readonly string INTOFDATE_FORMAT = @"00000000";

		/// <summary>デフォルト日時フォーマット</summary>
		public static readonly string TIME_FORMAT = @"yyyy/MM/dd HH:mm:ss.fff";
		/// <summary>デフォルト日時フォーマット</summary>
		public static readonly string TIME_CHECK = @"yyyy/M/d H:m:s.f";

		/// <summary>デフォルト年月フォーマット</summary>
		public static readonly string MONTH_FORMAT = @"yyyy/MM";
		/// <summary>デフォルト年月フォーマット</summary>
		public static readonly string MONTH_CHECK = @"yyyy/M";

		/// <summary>デフォルト数値</summary>
		public static readonly int DEFAULT_INT = 0;

		/// <summary>デフォルト通貨</summary>
		public static readonly string MONEY_FORMAT = @"\\#,0";

	}
}
