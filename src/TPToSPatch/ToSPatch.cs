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

		public void DownloadFull(string path,bool fForce=false) {
			Directory.CreateDirectory(path);
			foreach (ToSFullInfo tfi in LstFullInfo) {
				string tmpPath = Path.Combine(path, tfi.FileName);
				//	更新日チェック
				DateTime? tmpTime = GetFileTime(tmpPath);
				if (tfi.LastMod == tmpTime && !fForce) {
					continue;
				}
				//	既存ファイルの削除
				if (!DeleteExistFile(tmpPath)){
					throw new Exception($"ダウンロード対象が存在し、削除に失敗しました。\n${tmpPath}");
				}
				//	ダウンロード
				if (!DownloadFile(tmpPath, tfi.FileUrl)) {
					throw new Exception($"ダウンロードに失敗しました。\n${tmpPath}\n${tfi.FileUrl}");
				}
				//	更新日を対象ファイルのタイムスタンプに
				File.SetLastWriteTime(tmpPath, tfi.LastMod);
			}
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
	public class ToSFullInfo {
		public ToSFullInfo(string fileName, string urlRoot) {
			FileName = fileName+".ipf";
			FileUrl = Path.Combine(urlRoot, FileName);
		}
		public DateTime SetLastMod() {
			try {
				HttpWebRequest req = (HttpWebRequest)WebRequest.Create(FileUrl);
				req.Proxy = WebRequest.GetSystemWebProxy();
				req.Timeout = 2000;
				req.Method = "HEAD";
				using HttpWebResponse res = (HttpWebResponse)req.GetResponse();
				LastMod = res.LastModified;
				FileSize = res.ContentLength;
			} catch (Exception ex) {
				Log.SaveLog(ex);
			}
			return LastMod;
		}
		public string FileName;
		public string FileUrl;
		public long FileSize;
		public DateTime LastMod;
	}
	public class ToSPatchInfo {
		public ToSPatchInfo(string rev, string urlRootP, string urlRootR,bool fR) {
			RevText = rev.Replace(" 1","");
			FileNameP = RevText+"_001001.ipf";
			FileUrlP = Path.Combine(urlRootP, FileNameP);
			FileNameR = RevText+"_001001.pak";
			FileUrlR = Path.Combine(urlRootR, FileNameR);
			fRelease = fR;
		}
		public void SetLastMod() {
			try {
				HttpWebRequest req = (HttpWebRequest)WebRequest.Create(FileUrlP);
				req.Proxy = WebRequest.GetSystemWebProxy();
				req.Timeout = 2000;
				req.Method = "HEAD";
				using HttpWebResponse res = (HttpWebResponse)req.GetResponse();
				LastModP = res.LastModified;
				FileSizeP = res.ContentLength;
			} catch (Exception) {
			}
			try {
				if (fRelease) {
					HttpWebRequest req = (HttpWebRequest)WebRequest.Create(FileUrlR);
					req.Proxy = WebRequest.GetSystemWebProxy();
					req.Timeout = 2000;
					req.Method = "HEAD";
					using HttpWebResponse res = (HttpWebResponse)req.GetResponse();
					LastModR = res.LastModified;
					FileSizeR = res.ContentLength;
				}
			} catch(Exception) {
				fRelease = false;
			}
		}
		public int RevInt { get { return RevText._ToInt(0); } }
		public string RevText;
		public string FileNameP;
		public string FileUrlP;
		public long FileSizeP;
		public DateTime LastModP;
		public bool fRelease;
		public string FileNameR;
		public string FileUrlR;
		public long FileSizeR;
		public DateTime LastModR;
	}
}
