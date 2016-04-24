using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using EarlyPusher.Modules.BinkanSettingTab.Views;
using EarlyPusher.Utils;
using EarlyPusher.ViewModels;
using Microsoft.Win32;
using StFrLibs.Core.Basis;
using StFrLibs.Core.Commands;

namespace EarlyPusher.Modules.BinkanSettingTab.ViewModels
{
	public class BinkanSettingTabViewModel : OperateTabVMBase
	{
		private string pushPath;
		private string correctPath;
		private string incorrectPath;

		private string selectedItem;

		/// <summary>
		/// プッシュ音
		/// </summary>
		public string PushPath
		{
			get { return this.pushPath; }
			set { SetProperty( ref this.pushPath, value ); }
		}

		/// <summary>
		/// 正解音
		/// </summary>
		public string CorrectPath
		{
			get { return this.correctPath; }
			set { SetProperty( ref this.correctPath, value ); }
		}

		/// <summary>
		/// 不正解音
		/// </summary>
		public string IncorrectPath
		{
			get { return this.incorrectPath; }
			set { SetProperty( ref this.incorrectPath, value ); }
		}

		/// <summary>
		/// ヒント動画のリスト
		/// </summary>
		public ObservableCollection<string> Hints => this.Parent.Data?.Binkan.Hints;

		/// <summary>
		/// 選択中のアイテム
		/// </summary>
		public string SelectedItem
		{
			get { return this.selectedItem; }
			set { SetProperty( ref this.selectedItem, value, RaiseCommand ); }
		}

		public DelegateCommand OpenPushPathCommand { get; }
		public DelegateCommand OpenCorrectPathCommand { get; }
		public DelegateCommand OpenIncorrectPathCommand { get; }

		public DelegateCommand AddCommand { get; }
		public DelegateCommand RemCommand { get; }
		public DelegateCommand UpCommand { get; }
		public DelegateCommand DownCommand { get; }

		public BinkanSettingTabViewModel( MainVM parent )
			: base( parent )
		{
			this.View = new BinkanSettingTabView();
			this.Header = "ビンカン設定";

			this.OpenPushPathCommand      = new DelegateCommand( OpenPushPath );
			this.OpenCorrectPathCommand   = new DelegateCommand( OpenCorrectPath );
			this.OpenIncorrectPathCommand = new DelegateCommand( OpenIncorrectPath );

			this.AddCommand  = new DelegateCommand( Add );
			this.RemCommand  = new DelegateCommand( Rem, p => this.SelectedItem != null );
			this.UpCommand   = new DelegateCommand( Up, CanUp );
			this.DownCommand = new DelegateCommand( Down, CanDown );
		}

		#region 保存読み込み処理

		public override void LoadData()
		{
			NotifyPropertyChanged( nameof( this.Hints ) );

			this.PushPath      = this.Parent.Data.Binkan.PushPath;
			this.CorrectPath   = this.Parent.Data.Binkan.CorrectPath;
			this.IncorrectPath = this.Parent.Data.Binkan.IncorrectPath;

			this.Parent.Data.Binkan.PropertyChanged += Binkan_PropertyChanged;
		}

		private void Binkan_PropertyChanged( object sender, PropertyChangedEventArgs e )
		{
			this.PushPath      = this.Parent.Data.Binkan.PushPath;
			this.CorrectPath   = this.Parent.Data.Binkan.CorrectPath;
			this.IncorrectPath = this.Parent.Data.Binkan.IncorrectPath;
		}

		public override void SaveData()
		{
		}

		#endregion

		#region コマンド処理

		private void RaiseCommand()
		{
			this.AddCommand.RaiseCanExecuteChanged();
			this.RemCommand.RaiseCanExecuteChanged();
			this.UpCommand.RaiseCanExecuteChanged();
			this.DownCommand.RaiseCanExecuteChanged();
		}

		private void OpenPushPath( object obj )
		{
			var dlg = new OpenFileDialog();
			if( dlg.ShowDialog( App.Current.MainWindow ) == true )
			{
				var baseDir = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );
				this.Parent.Data.Binkan.PushPath = PathUtility.GetRelativePath( baseDir, dlg.FileName );
			}
		}

		private void OpenCorrectPath( object obj )
		{
			var dlg = new OpenFileDialog();
			if( dlg.ShowDialog( App.Current.MainWindow ) == true )
			{
				var baseDir = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );
				this.Parent.Data.Binkan.CorrectPath = PathUtility.GetRelativePath( baseDir, dlg.FileName );
			}
		}

		private void OpenIncorrectPath( object obj )
		{
			var dlg = new OpenFileDialog();
			if( dlg.ShowDialog( App.Current.MainWindow ) == true )
			{
				var baseDir = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );
				this.Parent.Data.Binkan.IncorrectPath = PathUtility.GetRelativePath( baseDir, dlg.FileName );
			}
		}

		private void Add( object obj )
		{
			var dlg = new OpenFileDialog();
			if( dlg.ShowDialog( App.Current.MainWindow ) == true )
			{
				var baseDir = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );
				this.Hints.Add( PathUtility.GetRelativePath( baseDir, dlg.FileName ) );
			}
		}

		private void Rem( object obj )
		{
			this.Hints.Remove( this.SelectedItem );
		}

		private bool CanUp( object obj )
		{
			return this.SelectedItem != null && this.Hints.IndexOf( this.SelectedItem ) > 0;
		}

		private void Up( object obj )
		{
			var item = this.SelectedItem;
			var index = this.Hints.IndexOf( item );
			this.Hints.RemoveAt( index );
			this.Hints.Insert( index - 1, item );
			this.SelectedItem = item;
		}

		private bool CanDown( object obj )
		{
			return this.SelectedItem != null && this.Hints.IndexOf( this.SelectedItem ) < this.Hints.Count - 1;
		}

		private void Down( object obj )
		{
			var item = this.SelectedItem;
			var index = this.Hints.IndexOf( item );
			this.Hints.RemoveAt( index );
			this.Hints.Insert( index + 1, item );
			this.SelectedItem = item;
		}

		#endregion
	}
}
