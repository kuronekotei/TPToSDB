using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;
using TPCmn;

using static TPCmn.Stc;
using System.Data;

namespace TPToS {
	public partial class DaBase : IDisposable {
		static readonly SQLiteConnectionStringBuilder connStr = new() { DataSource = DbStg.DbFile };
		SQLiteConnection conn = null;
		SQLiteTransaction tran = null;

		public DaBase() {
			Init();
		}
		#region IDisposable Support
		private bool disposedValue;

		protected virtual void Dispose(bool disposing) {
			if (!disposedValue) {
				if (disposing) {
					// TODO: マネージド状態を破棄します (マネージド オブジェクト)
				}

				try {
					Rollback();
				} finally {
					if (conn != null) {
						conn.Close();
					}
					conn = null;
				}

				disposedValue=true;
			}
		}

		~DaBase() {
			Dispose(disposing: false);
		}

		public void Dispose() {
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
		#endregion

		public void Init() {
			//	成否判定は例外の発生で判断
			if (conn != null) {
				conn.Close();
			}
			conn = new SQLiteConnection(connStr.ToString());
			conn.Open();
		}

		public void BeginTran() {
			//	成否判定は例外の発生で判断
			if (tran == null) {
				tran = conn.BeginTransaction();
			}
		}

		public void Commit() {
			//	成否判定は例外の発生で判断
			if (tran == null) {
				return;
			}
			tran.Commit();
			tran = null;
		}

		public void Rollback() {
			//	成否判定は例外の発生で判断
			if (tran == null) {
				return;
			}
			tran.Rollback();
			tran = null;
		}
		public DataTable Select(string sql, List<SQLiteParameter> sqlParams = null) {
			return ExecuteQuery(sql, sqlParams?.ToArray());
		}
		public DataTable Select(string sql, SQLiteParameter[] sqlParams) {
			return ExecuteQuery(sql, sqlParams);
		}
		private DataTable ExecuteQuery(string sql, SQLiteParameter[] sqlParams) {
			DataTable ret = new DataTable();
			try {
				using SQLiteCommand com = new SQLiteCommand();
				com.Connection = conn;
				com.CommandTimeout = 300;
				com.CommandType = CommandType.Text;
				com.CommandText = sql;
				if (sqlParams != null) {
					com.Parameters.AddRange(sqlParams);
				}
				if (tran != null) {
					com.Transaction = tran;
				}
				//	実行して結果を取得
				using SQLiteDataAdapter sqlDataAdapter = new SQLiteDataAdapter(com);
				sqlDataAdapter.Fill(ret);
			} catch (Exception ex) {
				DbLog.SaveLog(ex, sql, sqlParams);
				throw;
			}
			return ret;
		}

		public int Command(string sql, List<SQLiteParameter> sqlParams = null) {
			return ExecuteNonQuery(sql, sqlParams?.ToArray());
		}
		public int Command(string sql, SQLiteParameter[] sqlParams) {
			return ExecuteNonQuery(sql, sqlParams);
		}
		private int ExecuteNonQuery(string sql, SQLiteParameter[] sqlParams) {
			int ret = -1;
			try {
				using SQLiteCommand com = new SQLiteCommand();
				com.Connection = conn;
				com.CommandTimeout = 3600;
				com.CommandType = CommandType.Text;
				com.CommandText = sql;
				if (sqlParams != null) {
					com.Parameters.AddRange(sqlParams);
				}
				if (tran != null) {
					com.Transaction = tran;
				}
				//	実行して結果を取得
				ret = com.ExecuteNonQuery();
			} catch (Exception ex) {
				DbLog.SaveLog(ex, sql, sqlParams);
				throw;
			}
			return ret;
		}
	}
}
