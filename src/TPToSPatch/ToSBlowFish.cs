using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPToS {
	/// <summary>
	/// Class that provides blowfish encryption.
	/// </summary>
	public class ToSBlowFish {

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dat">データ列　内容を書き換えるので注意　長さは8の倍数にしておくこと</param>
		public static void Decipher(byte[] dat) {
			int len = dat.Length;
			uint xl, xr;

			if ((len % 8) != 0) {
				throw new Exception("Invalid Length");
			}
			for (int i = 0; i < len; i += 8) {
				// Encode the data in 8 byte blocks.
				xl = (uint)((dat[i] << 24) | (dat[i + 1] << 16) | (dat[i + 2] << 8) | dat[i + 3]);
				xr = (uint)((dat[i + 4] << 24) | (dat[i + 5] << 16) | (dat[i + 6] << 8) | dat[i + 7]);
				Decipher(ref xl, ref xr);
				// Now Replace the data.
				dat[i] = (byte)(xl >> 24);
				dat[i + 1] = (byte)(xl >> 16);
				dat[i + 2] = (byte)(xl >> 8);
				dat[i + 3] = (byte)(xl);
				dat[i + 4] = (byte)(xr >> 24);
				dat[i + 5] = (byte)(xr >> 16);
				dat[i + 6] = (byte)(xr >> 8);
				dat[i + 7] = (byte)(xr);
			}
		}

		private static void Decipher(ref uint xl, ref uint xr) {
			uint Xl;
			uint Xr;
			uint temp;
			short i;

			Xl = xl;
			Xr = xr;

			for (i = 17; i > 1; --i) {	//	17 ～ 2
				Xl ^= 0x5F5F5F5FU;
				Xr = 0x41414140U ^ Xr;

				temp = Xl;
				Xl = Xr;
				Xr = temp;
			}

			temp = Xl;
			Xl = Xr;
			Xr = temp;

			Xr ^= 0x5F5F5F5FU;	//	1
			Xl ^= 0x5F5F5F5FU;	//	0

			xl = Xl;
			xr = Xr;
		}

	}
}
