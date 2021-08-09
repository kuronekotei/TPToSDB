using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TPCmn;

namespace TPToS {
	public class DaDownload : DaBase {
		static bool fFirst = true;
		public DaDownload() {
			if (fFirst) {
				ExeCrtOrAltTable<DtDownload>();
			}
			fFirst = false;
		}
		public DtDownload GetDownloadOne(DtDownload dt) {
			Dictionary<string, string> where = new Dictionary<string, string>();
			where.Add(nameof(dt.SrvID), dt.SrvID);
			where.Add(nameof(dt.RemotePath), dt.RemotePath);
			return ExeSelectList<DtDownload>(where).FirstOrDefault();
		}
		public List<DtDownload> GetDownloadList() {
			return ExeSelectList<DtDownload>();
		}
		public int MergeDownload(DtDownload dt) {
			return ExeMerge(dt);
		}
	}
	[ExTable("TDownload")]
	public class DtDownload : DtBase {
		[ExKey,ExOrder]
		public string SrvID;

		[ExKey,ExOrder]
		public string RemotePath;

		[ExOrder]
		public string LocalPath;

		[ExOrder]
		public E_DL_STAT Stat = E_DL_STAT.None;

		[ExOrder]
		public string Msg;

		[ExOrder]
		public int Rev; // 000000

		[ExOrder]
		public string LastMod; // yyyy/MM/dd HH:mm
	}
}
