using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TPCmn {
	public static class DataHandle {
		//変換速度はLINQを使った方が早い(約5倍)
		//あくまでも簡易変換用にとどめたほうがいい
		//	spList = (from DataRow dr in dt.Rows
		//		select new SampleProperty() {
		//				Sample1 = dr["Sample1"].ToString(),
        //				Sample2 = dr["Sample2"].ToString(),
        //				Sample3 = dr["Sample3"].ToString(),
        //				Sample4 = dr["Sample4"].ToString()
		//		}).ToList();
		//
		//
		/// <summary>
		/// DataRowから任意のオブジェクトへ変換する
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dr"></param>
		/// <param name="tgt"></param>
		/// <returns></returns>
		public static T Row2Class<T>(DataRow dr, T tgt = default) where T : new() {

			if (dr == null) {
				return default;
			}

			var prps = typeof(T).GetProperties();
			var flds = typeof(T).GetFields();

			if (tgt == null) {
				tgt = new T();
			}

			foreach (var prp in prps) {
				if (!prp.CanWrite) {
					continue;
				}
				if ((!dr.Table.Columns.Contains(prp.Name)) ||  dr[prp.Name]==null || dr[prp.Name] == DBNull.Value) {
					continue;
				}
				if (prp.PropertyType == typeof(int)) {
					prp.SetValue(tgt, dr[prp.Name].__ToIntF());
					continue;
				}
				if (prp.PropertyType == typeof(int?)) {
					prp.SetValue(tgt, dr[prp.Name].__ToIntN());
					continue;
				}
				if (prp.PropertyType == typeof(decimal)) {
					prp.SetValue(tgt, dr[prp.Name].__ToDecF());
					continue;
				}
				if (prp.PropertyType == typeof(decimal?)) {
					prp.SetValue(tgt, dr[prp.Name].__ToDecF());
					continue;
				}
				if (prp.PropertyType == typeof(string)) {
					prp.SetValue(tgt, dr[prp.Name].__ToStr());
					continue;
				}
				prp.SetValue(tgt, dr[prp.Name]);
			}

			foreach (var fld in flds) {
				if (fld.IsInitOnly || fld.IsStatic) {
					continue;
				}
				if ((!dr.Table.Columns.Contains(fld.Name)) ||  dr[fld.Name]==null || dr[fld.Name] == DBNull.Value) {
					continue;
				}
				if (fld.FieldType == typeof(int)) {
					fld.SetValue(tgt, dr[fld.Name].__ToIntF());
					continue;
				}
				if (fld.FieldType == typeof(int?)) {
					fld.SetValue(tgt, dr[fld.Name].__ToIntN());
					continue;
				}
				if (fld.FieldType == typeof(decimal)) {
					fld.SetValue(tgt, dr[fld.Name].__ToDecF());
					continue;
				}
				if (fld.FieldType == typeof(decimal?)) {
					fld.SetValue(tgt, dr[fld.Name].__ToDecF());
					continue;
				}
				if (fld.FieldType == typeof(string)) {
					fld.SetValue(tgt, dr[fld.Name].__ToStr());
					continue;
				}
				fld.SetValue(tgt, dr[fld.Name]);
			}

			return tgt;
		}

		/// <summary>
		/// DataTableから任意のオブジェクトリストへ変換する
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dr"></param>
		/// <param name="lst"></param>
		/// <returns></returns>
		public static List<T> Table2List<T>(DataTable dt, List<T> lst = null) where T : new() {
			if (dt == null) {
				return lst;
			}

			if (lst == null) {
				lst = new List<T>();
			}

			if (dt.Rows == null || dt.Rows.Count <1) {
				return lst;
			}

			foreach (DataRow dr in dt.Rows) {
				lst.Add(Row2Class<T>(dr));
			}

			return lst;
		}



	}
}
