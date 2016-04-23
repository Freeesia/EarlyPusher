using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using EarlyPusher.Modules.BinkanSettingTab.Views;
using EarlyPusher.ViewModels;
using Microsoft.Win32;
using StFrLibs.Core.Basis;
using StFrLibs.Core.Commands;

namespace EarlyPusher.Modules.BinkanSettingTab.ViewModels
{
	public class BinkanSettingTabViewModel : OperateTabVMBase
	{
		private string selectedItem;

		/// <summary>
		/// ヒント動画のリスト
		/// </summary>
		public ObservableCollection<string> Hints { get; }

		/// <summary>
		/// 選択中のアイテム
		/// </summary>
		public string SelectedItem
		{
			get { return this.selectedItem; }
			set { SetProperty( ref this.selectedItem, value, RaiseCommand ); }
		}

		public DelegateCommand AddCommand { get; }
		public DelegateCommand RemCommand { get; }
		public DelegateCommand UpCommand { get; }
		public DelegateCommand DownCommand { get; }

		public BinkanSettingTabViewModel( MainVM parent )
			: base( parent )
		{
			this.View = new BinkanSettingTabView();
			this.Header = "ビンカン設定";

			this.Hints = new ObservableCollection<string>();
			this.AddCommand = new DelegateCommand( Add );
			this.RemCommand = new DelegateCommand( Rem, p => this.SelectedItem != null );
			this.UpCommand = new DelegateCommand( Up, CanUp );
			this.DownCommand = new DelegateCommand( Down, CanDown );
		}

		#region 保存読み込み処理

		public override void LoadData()
		{
			foreach( var item in this.Parent.Data.BinkanHints )
			{
				this.Hints.Add( item );
			}
		}

		public override void SaveData()
		{
			this.Parent.Data.BinkanHints.Clear();
			foreach( var item in this.Hints )
			{
				this.Parent.Data.BinkanHints.Add( item );
			}
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

		private void Add( object obj )
		{
			var dlg = new OpenFileDialog();
			if( dlg.ShowDialog( App.Current.MainWindow ) == true )
			{
				this.Hints.Add( dlg.FileName );
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
