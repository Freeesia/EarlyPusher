using EarlyPusher.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using EarlyPusher.Views;
using System.IO;
using EarlyPusher.Models;
using System.Text;

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

		private void Application_DispatcherUnhandledException( object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e )
		{
			var saved = false;
			var fileName = Path.GetFileNameWithoutExtension( SettingData.FileName ) + "_tmp.xml";
			try
			{
				var vm = this.MainWindow.DataContext as MainVM;
				vm.SaveData( fileName );

				saved = true;
			}
			catch
			{
			}

			var builder = new StringBuilder();
			builder.AppendLine( "ごめーん！落ちた！！" );

			if( saved )
			{
				builder.AppendLine( "とりあえず、" + fileName + "に保存したんで、また使いたかったら使ってください。" );
			}

			builder.AppendLine();
			builder.AppendLine();
			builder.AppendLine( "以下、例外" );
			builder.AppendLine();

			builder.AppendLine( e.Exception.ToString() );

			MessageBox.Show( builder.ToString() );
		}
	}
}
