using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

using TPCmn;
using static TPToS.ToSData;

namespace TPToS {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		public MainWindow() {
			InitializeComponent();
		}
		ObservableCollection<string> LstLog = new ObservableCollection<string>();

		public void LogInsert(string s) {
			_=Task.Run(() => {
				try {
					Dispatcher.Invoke(() => { if (IsLoaded) { LstLog.Add(s); while (LstLog.Count > 100) { LstLog.RemoveAt(0); } } });
				} catch { }
			});
		}

		private async void Window_Loaded(object sender, RoutedEventArgs e) {
			GridLog.ItemsSource = LstLog;
			Log.LogInsert = LogInsert;
			Init();
			await Download(E_Serv.JP);
		}
	}
}
