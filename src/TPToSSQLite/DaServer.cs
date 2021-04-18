using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TPCmn;

namespace TPToS {
	public class DaServer : DaBase {
		readonly List<DtServer> lstServer = new List<DtServer>{
			new DtServer{ SrvID="JP", SrvEnum=0, SrvName="日本", SrvValid=1, SrvURL="http://d3bbj7hlpo9jjy.cloudfront.net/live/patch/" },
			new DtServer{ SrvID="IT", SrvEnum=1, SrvName="国際", SrvValid=0, SrvURL="http://drygkhncipyq8.cloudfront.net/toslive/patch/" },
			new DtServer{ SrvID="KR", SrvEnum=2, SrvName="韓国", SrvValid=0, SrvURL="http://tosg.dn.nexoncdn.co.kr/patch/live/" },
			new DtServer{ SrvID="KT", SrvEnum=3, SrvName="Test", SrvValid=0, SrvURL="http://tosg.dn.nexoncdn.co.kr/patch/test/" },
			new DtServer{ SrvID="TW", SrvEnum=4, SrvName="台湾", SrvValid=0, SrvURL="http://tospatch.x2game.com.tw/live/patch/" },
		};
		public void InitServer() {
			{//常に再作成
				ExeCrtOrAltTable<DtServer>();

				List<DtServer> lst = ExeSelectListM(lstServer);
				lstServer.Clear();
				lstServer.AddRange(lst);
			}
			foreach (DtServer dtServ in lstServer) {
				List<SQLiteParameter> prms = new List<SQLiteParameter>();
				string sql = dtServ.MakeMerge(prms);
				Command(sql, prms);
			}
		}
		public List<DtServer> GetServerList() {
			return lstServer;
		}
	}
	[ExTable("MServer")]
	public class DtServer : DtBase {
		[ExKey,ExOrder]
		public string SrvID;

		[ExOrder]
		public int SrvEnum;

		[ExOrder]
		public string SrvName;

		[ExOrder]
		public int SrvValid;

		[ExOrder]
		public string SrvURL;

		[ExOrder]
		public int? LastRev; // 000000

		[ExOrder]
		public string LastMod; // yyyy/MM/dd HH:mm
	}
}
