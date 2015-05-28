using EarlyPusher.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using EarlyPusher.Views;

namespace EarlyPusher
{
	/// <summary>
	/// App.xaml の相互作用ロジック
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup( StartupEventArgs e )
		{
			this.MainWindow = new SettingWindow();
			this.MainWindow.DataContext = new MainVM();
			this.MainWindow.Show();
		}
	}
}
