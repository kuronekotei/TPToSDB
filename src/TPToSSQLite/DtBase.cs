using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TPCmn;

using static TPCmn.Stc;

namespace TPToS {
	public class DtBase {
		public bool KeyEqual(DtBase tgt) {
			return GetType()._KeyEql(this, tgt);
		}





		public string MakeCreate() {
			string tblName = GetType()._GetTable()??GetType().Name;
			List<MemberInfo> lstMem = GetType()._Defs();
			string retSql1 = $"CREATE TABLE IF NOT EXISTS [{tblName}]({BR}";
			string retSql2 = $");{BR}";
			string Cmm =" ";
			string tmpSql="";
			foreach (MemberInfo mem in lstMem) {
				if (mem.Typ== typeof(int) || mem.Typ== typeof(int?) || mem.Typ== typeof(decimal) || mem.Typ== typeof(decimal?) || mem.Typ== typeof(string) || mem.Typ== typeof(DateTime) || mem.Typ== typeof(DateTime?)) {
					if (mem.Typ== typeof(int) || mem.Typ== typeof(int?)) {
						tmpSql += $"{TB}{Cmm}[{mem.Name}]{TB}NUMERIC";
					}
					if (mem.Typ== typeof(decimal) || mem.Typ== typeof(decimal?)) {
						tmpSql += $"{TB}{Cmm}[{mem.Name}]{TB}NUMERIC";
					}
					if (mem.Typ== typeof(string)) {
						tmpSql += $"{TB}{Cmm}[{mem.Name}]{TB}TEXT";
					}
					if (mem.Typ== typeof(DateTime) || mem.Typ== typeof(DateTime?)) {
						tmpSql += $"{TB}{Cmm}[{mem.Name}]{TB}TEXT";
					}
					if (mem.fKey) {
						tmpSql += $"{TB}PRIMARY KEY";
					}
				}
				if (tmpSql._IsntEmp()) {
					Cmm=",";
					tmpSql += $"{BR}";
				}
			}
			return retSql1 + tmpSql + retSql2;
		}

		public bool CheckReCreate(List<string> existMem) {
			List<MemberInfo> lstMem = GetType()._Defs();
			foreach (MemberInfo mem in lstMem) {
				if (!(from em in existMem where em == mem.Name select true).FirstOrDefault()) {
					return true;
				}
			}
			return false;
		}
		/// <summary>
		/// テーブル再作成用SQLの生成
		/// <para>当SQLを利用するときはトランザクション内で行うことを推奨</para>
		/// </summary>
		public string MakeReCreate(List<string> existMem) {
			string tblName = GetType()._GetTable()??GetType().Name;
			List<MemberInfo> lstMem = GetType()._Defs();
			string retSql1 = $"DROP TABLE IF EXISTS [tmp{tblName}];{BR}{BR}";
			string retSql2 = $"CREATE TABLE IF NOT EXISTS [tmp{tblName}]({BR}";
			string retSql3 = $");{BR}{BR}";
			string retSql4 = $"INSERT OR IGNORE INTO [tmp{tblName}]({BR}";
			string retSql5 = $")SELECT{BR}";
			string retSql6 = $"FROM [{tblName}];{BR}{BR}";
			string retSql7 = $"DROP TABLE IF EXISTS [{tblName}];{BR}{BR}";
			string retSql8 = $"ALTER TABLE [tmp{tblName}] RENAME TO [{tblName}];{BR}";
			string Cmm2 =" ";
			string Cmm4 =" ";
			string Cmm5 =" ";
			string tmpSql2="";
			string tmpSql4="";
			string tmpSql5="";
			string mrgSql2="";
			string mrgSql4="";
			string mrgSql5="";
			foreach (MemberInfo mem in lstMem) {
				if (mem.Typ== typeof(int) || mem.Typ== typeof(int?) || mem.Typ== typeof(decimal) || mem.Typ== typeof(decimal?) || mem.Typ== typeof(string) || mem.Typ== typeof(DateTime) || mem.Typ== typeof(DateTime?)) {
					if (mem.Typ== typeof(int) || mem.Typ== typeof(int?)) {
						tmpSql2 += $"{TB}{Cmm2}[{mem.Name}]{TB}NUMERIC";
					}
					if (mem.Typ== typeof(decimal) || mem.Typ== typeof(decimal?)) {
						tmpSql2 += $"{TB}{Cmm2}[{mem.Name}]{TB}NUMERIC";
					}
					if (mem.Typ== typeof(string)) {
						tmpSql2 += $"{TB}{Cmm2}[{mem.Name}]{TB}TEXT";
					}
					if (mem.Typ== typeof(DateTime) || mem.Typ== typeof(DateTime?)) {
						tmpSql2 += $"{TB}{Cmm2}[{mem.Name}]{TB}TEXT";
					}

					tmpSql4 += $"{TB}{Cmm4}[{mem.Name}]";

					if ((from em in existMem where em == mem.Name select true).FirstOrDefault()) {
						tmpSql5 += $"{TB}{Cmm5}[{mem.Name}]";
					} else {
						tmpSql5 += $"{TB}{Cmm5}NULL";
					}

					if (mem.fKey) {
						tmpSql2 += $"{TB}PRIMARY KEY";
					}
				}
				if (tmpSql2._IsntEmp()) {
					Cmm2=",";
					mrgSql2 += tmpSql2+BR;
					tmpSql2 = "";
				}
				if (tmpSql4._IsntEmp()) {
					Cmm4=",";
					mrgSql4 += tmpSql4+BR;
					tmpSql4 = "";
				}
				if (tmpSql5._IsntEmp()) {
					Cmm5=",";
					mrgSql5 += tmpSql5+BR;
					tmpSql5 = "";
				}
			}
			return retSql1 + retSql2 + mrgSql2 + retSql3 + retSql4 + mrgSql4 + retSql5 + mrgSql5 + retSql6 + retSql7 + retSql8;
		}

		public string MakeDrop() {
			string tblName = GetType()._GetTable()??GetType().Name;
			string retSql1 = $"DROP TABLE IF EXISTS [{tblName}];{BR}";
			return retSql1;
		}

		public string MakeSelectCols() {
			string tblName = GetType()._GetTable()??GetType().Name;
			string retSql1 = $"PRAGMA table_info('{tblName}');{BR}";
			return retSql1;
		}
		public string MakeSelectAll() {
			string tblName = GetType()._GetTable()??GetType().Name;
			string retSql1 = $"SELECT * FROM [{tblName}];{BR}";
			return retSql1;
		}

		/// <summary>
		/// データ登録SQLの生成
		/// <para>当SQLを利用するときはトランザクション内で行うことを推奨</para>
		/// <para>Hashについて、特別処理</para>
		/// </summary>
		public string MakeMerge(List<SQLiteParameter> prms) {
			Type typ = GetType();
			string tblName = typ._GetTable()??typ.Name;
			List<MemberInfo> lstMem = typ._Defs(this);

			string retSql1 = $"INSERT INTO [{tblName}]({BR}";
			string retSql2 = $")VALUES({BR}";
			string retSql3 = $")ON CONFLICT({BR}";
			string retSql4 = $")DO UPDATE SET{BR}";
			string retSql6 = $";{BR}";
			string Cmm1 =" ";
			string Cmm3 =" ";
			string Cmm4 =" ";
			string tmpSql1="";
			string tmpSql2="";
			string tmpSql3="";
			string tmpSql4="";
			string mrgSql1="";
			string mrgSql2="";
			string mrgSql3="";
			string mrgSql4="";
			string mrgSql5="";
			foreach (var mem in lstMem) {
				if (mem.Typ== typeof(int) || mem.Typ== typeof(int?) || mem.Typ== typeof(decimal) || mem.Typ== typeof(decimal?) || mem.Typ== typeof(string) || mem.Typ== typeof(DateTime) || mem.Typ== typeof(DateTime?)) {
					tmpSql1 += $"{TB}{Cmm1}[{mem.Name}]";
					tmpSql2 += $"{TB}{Cmm1}@{mem.Name}";
					if (mem.fKey) {
						tmpSql3 += $"{TB}{Cmm3}[{mem.Name}]";
					} else {
						tmpSql4 += $"{TB}{Cmm4}[{mem.Name}]{TB}= @{mem.Name}";
					}
					if (mem.Typ== typeof(int) || mem.Typ== typeof(int?)) {
						if (mem.ValI==null) {
							prms.Add(new SQLiteParameter($"@{mem.Name}", DBNull.Value));
						} else {
							prms.Add(new SQLiteParameter($"@{mem.Name}", mem.ValI));
						}
					}
					if (mem.Typ== typeof(decimal) || mem.Typ== typeof(decimal?)) {
						if (mem.ValN==null) {
							prms.Add(new SQLiteParameter($"@{mem.Name}", DBNull.Value));
						} else {
							prms.Add(new SQLiteParameter($"@{mem.Name}", mem.ValN));
						}
					}
					if (mem.Typ== typeof(string)) {
						if (mem.ValS==null) {
							prms.Add(new SQLiteParameter($"@{mem.Name}", DBNull.Value));
						} else {
							prms.Add(new SQLiteParameter($"@{mem.Name}", mem.ValS));
						}
					}
					if (mem.Typ== typeof(DateTime) || mem.Typ== typeof(DateTime?)) {
						if (mem.ValS==null) {
							prms.Add(new SQLiteParameter($"@{mem.Name}", DBNull.Value));
						} else {
							prms.Add(new SQLiteParameter($"@{mem.Name}", mem.ValS));
						}
					}
					if (mem.Name == "Hash") {
						mrgSql5 = $"WHERE	[{mem.Name}] <> @{mem.Name}{BR}";	//	行ハッシュが異なる場合のみ更新
					}
				}
				if (tmpSql1._IsntSpc()) {
					Cmm1=",";
					mrgSql1 += tmpSql1+BR;
					tmpSql1 = "";
					mrgSql2 += tmpSql2+BR;
					tmpSql2 = "";
				}
				if (tmpSql3._IsntSpc()) {
					Cmm3=",";
					mrgSql3 += tmpSql3+BR;
					tmpSql3 = "";
				}
				if (tmpSql4._IsntSpc()) {
					Cmm4=",";
					mrgSql4 += tmpSql4+BR;
					tmpSql4 = "";
				}
			}
			return retSql1 + mrgSql1 + retSql2 + mrgSql2 + retSql3 + mrgSql3 + retSql4 + mrgSql4 + mrgSql5 + retSql6;
		}
	}
}
