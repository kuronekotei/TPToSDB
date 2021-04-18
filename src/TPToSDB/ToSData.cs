using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TPCmn;

namespace TPToS {
	public static class ToSData {
		public static List<DtServer>LstServ;
		public static readonly Dictionary<E_Serv,DtServer>DicServ = new Dictionary<E_Serv, DtServer>();
		public static readonly Dictionary<E_Serv,RootPath>DicPath = new Dictionary<E_Serv, RootPath>();

		public static void Init() {
			using (DaServer da = new DaServer()) {
				da.InitServer();
				LstServ = da.GetServerList();
			}

			foreach (var serv in LstServ) {
				E_Serv es = (E_Serv)serv.SrvEnum;
				DicServ.Add(es, serv);
				DicPath.Add(es, new RootPath(es,Cmn.CurDir));
			}
		}
	}
	public class RootPath {
		public RootPath(E_Serv es,string root) {
			Serv	= es;
			Root	= root;
			Data	= Path.Combine(Root, "data");
			DwnL    = Path.Combine(Data, "_dl");
			DFul    = Path.Combine(DwnL, "FD", es.ToString());
			DPat    = Path.Combine(DwnL, "PD", es.ToString());
			DRel    = Path.Combine(DwnL, "PR", es.ToString());
			Extr    = Path.Combine(Data, "_ext");
			EFul    = Path.Combine(Extr, "FD", es.ToString());
			EPat    = Path.Combine(Extr, "PD", es.ToString());
			ERel    = Path.Combine(Extr, "PR", es.ToString());
		}
		public E_Serv Serv;
		public string Root;
		public string Data;
		public string DwnL;
		public string DFul;	//フルはIPF
		public string DPat; //パッチはIPF
		public string DRel;	//リリースはPak
		public string Extr;
		public string EFul;
		public string EPat;
		public string ERel;
	}
	public enum E_Serv {
		JP=0,
		IT=1,
		KR=2,
		KT=3,
		TW=4,
	}
}
