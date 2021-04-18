using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TPCmn;

namespace TPToS {
	public partial class DaBase {
		public void ExeCrtOrAltTable<T>() where T : DtBase,new() {
			T dto = new T();
			string sql;
			sql = dto.MakeCreate();
			Command(sql);

			List<string> lstCol =  GetCols<T>();
			if (dto.CheckReCreate(lstCol)) {
				bool fTran = false;
				if (tran == null) {
					BeginTran();
					fTran = true;
				}
				sql = dto.MakeReCreate(lstCol);
				Command(sql);
				if (fTran) {
					Commit();
				}
			}
		}
		public List<string> GetCols<T>() where T : DtBase,new() {
			T dto = new T();
			string sql;
			sql = dto.MakeSelectCols();
			DataTable dt = Select(sql);
			return (from DataRow dr in dt.Rows select dr["name"].__ToStr()).ToList();
		}
		public List<T> ExeSelectList<T>() where T : DtBase, new() {
			T dto = new T();
			List<T> lst = new List<T>();

			DataTable dt = Select(dto.MakeSelectAll());

			DataHandle.Table2List(dt, lst);

			return lst;
		}
		public List<T> ExeSelectListM<T>(List<T> baselst) where T : DtBase, new() {
			T dto = new T();
			List<T> lst = new List<T>();

			DataTable dt = Select(dto.MakeSelectAll());

			DataHandle.Table2List(dt, lst);

			for (int i = 0; i< baselst.Count; i++) {
				T itm1 = baselst[i];
				bool fEql = false;
				for (int j = 0; j< lst.Count; j++) {
					T itm2 = lst[j];
					if (itm1.KeyEqual(itm2)) {
						fEql = true;
						break;
					}
				}
				if (!fEql) {
					lst.Add(itm1);
				}
			}

			return lst;
		}
	}
}
