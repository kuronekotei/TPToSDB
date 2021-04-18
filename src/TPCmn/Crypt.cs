using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TPCmn {
	public static class Crypt {
		/// <summary>MD5をBASE64変換して出力する</summary>
		/// <returns>22文字のstring</returns>
		public static string GetMD5BASE64(string s) {
			byte[] hash = GetHash<MD5CryptoServiceProvider>(s);
			return Convert.ToBase64String(hash);    //	22文字 (128/6=21.33)
		}


		public static byte[] GetHash<T>(string text) where T : HashAlgorithm, new() {
			// 文字列をバイト型配列に変換する
			byte[] data = Encoding.UTF8.GetBytes(text);
			byte[] bs;
			// ハッシュアルゴリズム生成
			using (var algorithm = new T()) {
				// ハッシュ値を計算する
				bs = algorithm.ComputeHash(data);
			}
			return bs;
		}
	}
}
