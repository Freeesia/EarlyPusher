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
using System.Windows.Shapes;

namespace EarlyPusher.Views
{
	/// <summary>
	/// PlayWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class PlayWindow : Window
	{
		public PlayWindow()
		{
			InitializeComponent();
		}

		protected override void OnMouseLeftButtonDown( MouseButtonEventArgs e )
		{
			this.DragMove();
		}

		private void Window_PreviewKeyDown( object sender, KeyEventArgs e )
		{
			if( e.Key == Key.Escape || e.ImeProcessedKey == Key.Escape )
			{
				this.Close();
			}
		}
	}
}
