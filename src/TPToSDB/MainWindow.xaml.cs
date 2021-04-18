using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using static TPToS.ToSData;

namespace TPToS {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		public MainWindow() {

			Init();
			ToSPatch ptc = new ToSPatch(DicServ[E_Serv.JP].SrvURL);
			ptc.GetPacthList(0);
			ptc.DownloadFull(DicPath[E_Serv.JP].DFul);
			MessageBox.Show("完了");
		}
	}
}
