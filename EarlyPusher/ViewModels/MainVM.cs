using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using EarlyPusher.Manager;
using EarlyPusher.Models;
using EarlyPusher.Modules.ChoiceTab.ViewModels;
using EarlyPusher.Modules.EarlyTab.ViewModels;
using EarlyPusher.Modules.SettingTab.ViewModels;
using EarlyPusher.Modules.SortTab.ViewModels;
using EarlyPusher.Views;
using StFrLibs.Core.Basis;
using StFrLibs.Core.Commands;

namespace EarlyPusher.ViewModels
{
	public class MainVM : ViewModelBase
	{
		private SettingData data;

		private PlayWindow window;
		private DeviceManager manager;

		private List<OperateTabVMBase> operateVMs = new List<OperateTabVMBase>();

		private OperateTabVMBase selectedTab;

		private OperateSettingVM operateSetting;
		private OperateChoiceVM operateChoice;
		private OperateEarlyVM operateEarly;
		private OperateSortVM operateSort;

		#region プロパティ

		public DelegateCommand WindowCommand { get; private set; }
		public DelegateCommand WindowMaxCommand { get; private set; }

		public DelegateCommand LoadedCommand { get; private set; }
		public DelegateCommand ClosingCommand { get; private set; }

		/// <summary>
		/// ボタンデバイスの管理クラス
		/// </summary>
		public DeviceManager Manager
		{
			get { return this.manager; }
		}

		public SettingData Data
		{
			get { return this.data; }
		}

		public OperateTabVMBase SelectedTab
		{
			get { return this.selectedTab; }
			set { SetProperty( ref this.selectedTab, value, SelectedTabChanged, SelectedTabChanging ); }
		}
		
		public OperateSettingVM OperateSetting
		{
			get { return this.operateSetting; }
			set { SetProperty( ref this.operateSetting, value ); }
		}

		public OperateChoiceVM OperateChoice
		{
			get { return this.operateChoice; }
			set { SetProperty( ref this.operateChoice, value ); }
		}

		public OperateEarlyVM OperateEarly
		{
			get { return this.operateEarly; }
			set { SetProperty( ref this.operateEarly, value ); }
		}

		public OperateSortVM OperateSort
		{
			get { return this.operateSort; }
			set { SetProperty( ref this.operateSort, value ); }
		}
		
		#endregion
		
		public MainVM()
		{
			this.WindowCommand = new DelegateCommand( ShowCloseWindow, CanShowCloseWindow );
			this.WindowMaxCommand = new DelegateCommand( MaximazeWindow, CanMaximaize );

			this.LoadedCommand = new DelegateCommand( Inited, null );
			this.ClosingCommand = new DelegateCommand( Closing, null );

			this.manager = new DeviceManager();

			this.OperateSetting = new OperateSettingVM( this );
			this.OperateChoice = new OperateChoiceVM( this );
			this.OperateEarly = new OperateEarlyVM( this );
			this.OperateSort = new OperateSortVM( this );

			this.operateVMs.Add( this.OperateSetting );
			this.operateVMs.Add( this.OperateChoice );
			this.operateVMs.Add( this.OperateEarly );
			this.operateVMs.Add( this.OperateSort );
		}

		private void SelectedTabChanging()
		{
			if( this.SelectedTab != null )
			{
				this.SelectedTab.Deactivate();
			}
		}

		private void SelectedTabChanged()
		{
			if( this.SelectedTab != null )
			{
				this.SelectedTab.Activate();
			}
			this.WindowCommand.RaiseCanExecuteChanged();
		}

		#region コマンド

		private bool CanShowCloseWindow( object obj )
		{
			return this.SelectedTab.PlayView != null;
		}

		/// <summary>
		/// プレイウィンドウの開閉処理
		/// </summary>
		/// <param name="obj"></param>
		private void ShowCloseWindow( object obj )
		{
			if( this.window != null )
			{
				this.window.Close();
				this.window = null;

			}
			else
			{
				this.window = new PlayWindow();
				this.window.DataContext = this.SelectedTab;
				this.window.Show();
			}
			this.WindowMaxCommand.RaiseCanExecuteChanged();
		}

		/// <summary>
		/// プレイウィンドウの最大化可能かどうか
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		private bool CanMaximaize( object obj )
		{
			return this.window != null;
		}

		/// <summary>
		/// プレイウィンドウの最大化・通常化
		/// </summary>
		/// <param name="obj"></param>
		private void MaximazeWindow( object obj )
		{
			Contract.Assert( this.window != null );

			if( this.window.WindowState != System.Windows.WindowState.Maximized )
			{
				this.window.WindowState = System.Windows.WindowState.Maximized;
			}
			else
			{
				this.window.WindowState = System.Windows.WindowState.Normal;
			}
		}

		/// <summary>
		/// ウィンドウが開くときの処理
		/// </summary>
		/// <param name="obj"></param>
		private void Inited( object obj )
		{
			LoadData();

			this.SelectedTab = this.OperateSetting;
		}

		/// <summary>
		/// ウィンドウが閉じるときの処理
		/// </summary>
		/// <param name="obj"></param>
		private void Closing( object obj )
		{
			SaveData();
			if( this.window != null )
			{
				this.window.Close();
			}

			this.manager.Dispose();
		}

		#endregion

		#region 設定読み書き

		/// <summary>
		/// 設定を読み込みます。
		/// </summary>
		private void LoadData()
		{
			try
			{
				if( File.Exists( SettingData.FileName ) )
				{
					using( FileStream file = new FileStream( SettingData.FileName, FileMode.Open ) )
					{
						XmlSerializer xml = new XmlSerializer( typeof( SettingData ) );
						this.data = xml.Deserialize( file ) as SettingData;
					}
				}
			}
			catch
			{
			}
			finally
			{
				if( this.data == null )
				{
					this.data = new SettingData();
				}
			}

			//4択用に必ず1チーム4人は確保する。
			foreach( var team in this.data.TeamList )
			{
				while( team.Members.Count < 4 )
				{
					team.Members.Add( new MemberData() );
				}
			}

			//各タブのロード処理
			foreach( var vm in this.operateVMs )
			{
				vm.LoadData();
			}
		}

		/// <summary>
		/// 設定を保存します。
		/// </summary>
		public void SaveData()
		{
			//各タブのセーブ処理
			foreach( var vm in this.operateVMs )
			{
				vm.SaveData();
			}

			using( Stream file = new FileStream( SettingData.FileName, FileMode.Create ) )
			{
				XmlSerializer xml = new XmlSerializer( typeof( SettingData ) );
				xml.Serialize( file, this.data );
			}
		}

		#endregion

	}
}
