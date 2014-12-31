using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using HayaoshiPusher.ViewModels;
using HayaoshiPusher.Views;

namespace HayaoshiPusher
{
	/// <summary>
	/// App.xaml の相互作用ロジック
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup( StartupEventArgs e )
		{
			this.MainWindow = new SettingWindow();
			this.MainWindow.DataContext = new SettingVM();
			this.MainWindow.Show();
		}
	}
}
