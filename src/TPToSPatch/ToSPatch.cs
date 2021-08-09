using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using TPCmn;

namespace TPToS {
	public class ToSPatch {
		public ToSPatch(string url) {
			UrlFulDat		= Path.Combine(url, "full/data/");
			UrlFulDatLst	= Path.Combine(url, "full/data.file.list.txt");
			UrlPatDat		= Path.Combine(url, "partial/data/");
			UrlPatDatRev	= Path.Combine(url, "partial/data.revision.txt");
			UrlPatRel		= Path.Combine(url, "partial/release/");
			UrlPatRelRev	= Path.Combine(url, "partial/release.revision.txt");
		}

		readonly string UrlFulDat;
		readonly string UrlFulDatLst;
		readonly string UrlPatDat;
		readonly string UrlPatDatRev;
		readonly string UrlPatRel;
		readonly string UrlPatRelRev;

		public List<ToSFullInfo> LstFullInfo = new List<ToSFullInfo>();
		public List<ToSPatchInfo> LstPatchInfo = new List<ToSPatchInfo>();

		public void GetPacthList(int curRev) {
			byte[] bytFulLst;
			byte[] bytDatRev;
			byte[] bytRelRev;
			using (WebClient wc = new WebClient()) {
				bytFulLst = wc.DownloadData(UrlFulDatLst);
				bytDatRev = wc.DownloadData(UrlPatDatRev);
				bytRelRev = wc.DownloadData(UrlPatRelRev);
			}
			string strFulLst = RevDecrypt(bytFulLst);
			string strDatRev = RevDecrypt(bytDatRev);
			string strRelRev = RevDecrypt(bytRelRev);

			List<string> LstFulIpf = strFulLst.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
			List<string> LstPatPac = strDatRev.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
			List<string> LstRelPac = strRelRev.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

			for (int i = 0; i<LstFulIpf.Count; i++) {
				string tmpStr = LstFulIpf[i];
				if (tmpStr._Mtc(@"[\x00]")) {
					LstFulIpf.RemoveAt(i);
					i--;
					continue;
				}
			}
			for (int i = 0; i<LstFulIpf.Count; i++) {
				string tmpStr = LstFulIpf[i];
				ToSFullInfo tfi = new ToSFullInfo(tmpStr,UrlFulDat);
				tfi.SetLastMod();
				LstFullInfo.Add(tfi);
			}
			for (int i = 0; i<LstPatPac.Count; i++) {
				string tmpStr = LstPatPac[i];
				if (tmpStr._Mtc(@"[\x00]")) {
					LstPatPac.RemoveAt(i);
					i--;
					continue;
				}
				if (tmpStr.Replace(" 1", "")._ToInt(0)<=curRev) {
					LstPatPac.RemoveAt(i);
					i--;
					continue;
				}
			}
			for (int i = 0; i<LstRelPac.Count; i++) {
				string tmpStr = LstRelPac[i];
				if (tmpStr._Mtc(@"[\x00]")) {
					LstRelPac.RemoveAt(i);
					i--;
					continue;
				}
				if (tmpStr.Replace(" 1", "")._ToInt(0)<=curRev) {
					LstPatPac.RemoveAt(i);
					i--;
					continue;
				}
			}
			for (int i = 0; i<LstPatPac.Count; i++) {
				string tmpStr = LstPatPac[i];
				bool fR = (from string s in LstRelPac where s == tmpStr select true ).FirstOrDefault();
				ToSPatchInfo tpi = new ToSPatchInfo(tmpStr,UrlPatDat,UrlPatRel,fR);
				tpi.SetLastMod();
				LstPatchInfo.Add(tpi);
			}
		}

		public List<ToSDlInfo> GetDownloadList(string pathDFul, string pathDPat, string pathDRel, bool fForce = false) {
			Directory.CreateDirectory(pathDFul);
			Directory.CreateDirectory(pathDPat);
			Directory.CreateDirectory(pathDRel);

			List<ToSDlInfo> lst = new List<ToSDlInfo>();
			foreach (ToSFullInfo tfi in LstFullInfo) {
				tfi.DlInfo.LocalPath = Path.Combine(pathDFul, tfi.DlInfo.FileName);
				//	更新日チェック
				DateTime? tmpTime = GetFileTime(tfi.DlInfo.LocalPath);
				if (tfi.DlInfo.LastMod == tmpTime && !fForce) {
					continue;
				}
				lst.Add(tfi.DlInfo);
			}
			foreach (ToSPatchInfo tpi in LstPatchInfo) {
				{
					tpi.DlInfoP.LocalPath = Path.Combine(pathDPat, tpi.DlInfoP.FileName);
					//	更新日チェック
					DateTime? tmpTime = GetFileTime(tpi.DlInfoP.LocalPath);
					if (tpi.DlInfoP.LastMod == tmpTime && !fForce) {
						continue;
					}
					lst.Add(tpi.DlInfoP);
				}
				if (tpi.fRelease) {
					tpi.DlInfoR.LocalPath = Path.Combine(pathDRel, tpi.DlInfoR.FileName);
					//	更新日チェック
					DateTime? tmpTime = GetFileTime(tpi.DlInfoR.LocalPath);
					if (tpi.DlInfoR.LastMod == tmpTime && !fForce) {
						continue;
					}
					lst.Add(tpi.DlInfoR);
				}
			}
			return lst;
		}

		public bool Download(ToSDlInfo tdi) {
			//	既存ファイルの削除
			if (!DeleteExistFile(tdi.LocalPath)) {
				throw new Exception($"ダウンロード対象が存在し、削除に失敗しました。\n${tdi.LocalPath}");
			}
			//	ダウンロード
			if (!DownloadFile(tdi.LocalPath, tdi.RemotePath)) {
				throw new Exception($"ダウンロードに失敗しました。\n${tdi.LocalPath}\n${tdi.RemotePath}");
			}
			//	更新日を対象ファイルのタイムスタンプに
			File.SetLastWriteTime(tdi.LocalPath, tdi.LastMod);
			return true;
		}

		/// <summary>日付を取る　取れなきゃNull</summary>
		static DateTime? GetFileTime(string path) {
			DateTime? fileTime = null;
			try {
				fileTime = File.GetLastWriteTime(path);
			} catch (Exception) {
			}
			return fileTime;
		}

		/// <summary>無ければTrue　あれば消してTrue　何かあったらFalse</summary>
		static bool DeleteExistFile(string path) {
			try {
				if (!File.Exists(path)) {
					return true;
				}
				File.Delete(path);
				return !File.Exists(path);

			} catch (Exception) {
			}
			return false;
		}

		static bool DownloadFile(string savePath,string tgtUrl) {
			try {
				using (WebClient wc = new WebClient()) {
					wc.DownloadFile(tgtUrl, savePath);
				}
				return File.Exists(savePath);
			} catch (Exception ex) {
				Log.SaveLog(ex);
			}
			return false;
		}

		public static string RevDecrypt(byte[] bytes) {
			byte[] temp = (new ArraySegment<byte>(bytes, 8, bytes.Length-8)).ToArray();
			ToSBlowFish.Decipher(temp);
			return Encoding.ASCII.GetString(temp);
		}

	}
	public class ToSDlInfo {
		public string FileName;
		public string RemotePath;
		public string LocalPath;
		public long FileSize;
		public DateTime LastMod;
	}
	public class ToSFullInfo {
		public ToSFullInfo(string fileName, string urlRoot) {
			DlInfo.FileName = fileName+".ipf";
			DlInfo.RemotePath = Path.Combine(urlRoot, DlInfo.FileName);
		}
		public DateTime SetLastMod() {
			try {
				HttpWebRequest req = (HttpWebRequest)WebRequest.Create(DlInfo.RemotePath);
				req.Proxy = WebRequest.GetSystemWebProxy();
				req.Timeout = 2000;
				req.Method = "HEAD";
				using HttpWebResponse res = (HttpWebResponse)req.GetResponse();
				DlInfo.LastMod = res.LastModified;
				DlInfo.FileSize = res.ContentLength;
			} catch (Exception ex) {
				Log.SaveLog(ex);
			}
			return DlInfo.LastMod;
		}
		public ToSDlInfo DlInfo = new ToSDlInfo();
	}
	public class ToSPatchInfo {
		public ToSPatchInfo(string rev, string urlRootP, string urlRootR,bool fR) {
			RevText = rev.Replace(" 1","");
			DlInfoP.FileName = RevText+"_001001.ipf";
			DlInfoP.RemotePath = Path.Combine(urlRootP, DlInfoP.FileName);
			DlInfoR.FileName = RevText+"_001001.pak";
			DlInfoR.RemotePath = Path.Combine(urlRootR, DlInfoR.FileName);
			fRelease = fR;
		}
		public void SetLastMod() {
			try {
				HttpWebRequest req = (HttpWebRequest)WebRequest.Create(DlInfoP.RemotePath);
				req.Proxy = WebRequest.GetSystemWebProxy();
				req.Timeout = 2000;
				req.Method = "HEAD";
				using HttpWebResponse res = (HttpWebResponse)req.GetResponse();
				DlInfoP.LastMod = res.LastModified;
				DlInfoP.FileSize = res.ContentLength;
			} catch (Exception) {
			}
			try {
				if (fRelease) {
					HttpWebRequest req = (HttpWebRequest)WebRequest.Create(DlInfoR.RemotePath);
					req.Proxy = WebRequest.GetSystemWebProxy();
					req.Timeout = 2000;
					req.Method = "HEAD";
					using HttpWebResponse res = (HttpWebResponse)req.GetResponse();
					DlInfoR.LastMod = res.LastModified;
					DlInfoR.FileSize = res.ContentLength;
				}
			} catch(Exception) {
				fRelease = false;
			}
		}
		public int RevInt { get { return RevText._ToInt(0); } }
		public ToSDlInfo DlInfoP = new ToSDlInfo();
		public ToSDlInfo DlInfoR = new ToSDlInfo();
		public string RevText;
		public bool fRelease;
	}
}
