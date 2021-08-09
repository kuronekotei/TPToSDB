using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TPCmn;
using static TPCmn.Stc;

namespace TPToS {
	public static class ToSData {
		public static List<DtServer>LstServ;
		public static readonly Dictionary<E_Serv,DtServer>DicServ = new Dictionary<E_Serv, DtServer>();
		public static readonly Dictionary<E_Serv,RootPath>DicPath = new Dictionary<E_Serv, RootPath>();

		public static void Init() {
			Log.SaveLog("▼▼▼Init");
			using (DaServer da = new DaServer()) {
				da.InitServer();
				LstServ = da.GetServerList();
			}

			foreach (var serv in LstServ) {
				E_Serv es = (E_Serv)serv.SrvEnum;
				DicServ.Add(es, serv);
				DicPath.Add(es, new RootPath(es,Cmn.CurDir));
			}
			Log.SaveLog("▲▲▲Init");
		}
		public static async Task<bool> Download(E_Serv es) {
			Log.SaveLog("▼▼▼Download");
			List<ToSDlInfo> lstDl = null;
			using (DaDownload daDl = new DaDownload()) {
				await Task.Run(() => {
					ToSPatch ptc = new ToSPatch(DicServ[es].SrvURL);
					ptc.GetPacthList(0);
					lstDl = ptc.GetDownloadList(DicPath[es].DFul, DicPath[es].DPat, DicPath[es].DRel);
					for (int i = 0; i<lstDl.Count; i++) {
						ToSDlInfo xDl = lstDl[i];
						DtDownload dtDl = new DtDownload(){
							SrvID=es.ToString()
							,RemotePath=xDl.RemotePath
							,LocalPath=xDl.LocalPath
							,LastMod=xDl.LastMod.ToString()
							,Stat=E_DL_STAT.None
						};
						var tmpDl = daDl.GetDownloadOne(dtDl);
						if (tmpDl?.Stat == E_DL_STAT.Skip || tmpDl?.Stat == E_DL_STAT.Success) {
							lstDl.RemoveAt(i);
							i--;
						}
					}
				});
				Log.SaveLog($"{TB}Download {lstDl.Count}ファイル");
				for (int i = 0; i<lstDl.Count; i++) {
					ToSDlInfo xDl = lstDl[i];
					Log.SaveLog($"{TB}Download {i+1}/{lstDl.Count} {xDl.RemotePath}");
					ToSPatch ptc = new ToSPatch(DicServ[es].SrvURL);
					DtDownload dtDl = new DtDownload(){
						SrvID=es.ToString()
						,RemotePath=xDl.RemotePath
						,LocalPath=xDl.LocalPath
						,LastMod=xDl.LastMod.ToString()
						,Stat=E_DL_STAT.None
					};
					try {
						ptc.Download(xDl);
						dtDl.Stat = E_DL_STAT.Success;
						dtDl.Msg = "Success";
					} catch (Exception ex) {
						dtDl.Stat = E_DL_STAT.Error;
						dtDl.Msg = ex.Message;
					}
					daDl.MergeDownload(dtDl);
					Log.SaveLog($"{TB}Download {i+1}/{lstDl.Count} {dtDl.Msg}");
				}
			}
			Log.SaveLog("▲▲▲Download");
			return true;
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
