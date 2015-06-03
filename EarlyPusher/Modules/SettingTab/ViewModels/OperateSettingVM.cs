using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using EarlyPusher.Manager;
using EarlyPusher.Models;
using EarlyPusher.ViewModels;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using SlimDX.DirectInput;
using StFrLibs.Core.Adapters;
using StFrLibs.Core.Basis;
using StFrLibs.Core.Commands;

namespace EarlyPusher.Modules.SettingTab.ViewModels
{
	public class OperateSettingVM : OperateTabVMBase
	{
		private ObservableHashVMCollection<TeamSettingVM> teams = new ObservableHashVMCollection<TeamSettingVM>();
		private ObservableHashVMCollection<MemberSettingVM> members = new ObservableHashVMCollection<MemberSettingVM>();
		private ObservableHashCollection<string> devices = new ObservableHashCollection<string>();
		private ViewModelsAdapter<TeamSettingVM,TeamData> teamAdapter;

		private string earlyVideoDir;
		private string choiceVideoDir;
		private long updateTime;

		private TeamSettingVM selectedTeam;
		private MemberSettingVM selectedMember;

		#region プロパティ

		public DelegateCommand AddMemberCommand { get; private set; }
		public DelegateCommand DelMemberCommand { get; private set; }
		public DelegateCommand SelectEarlyVideoDirCommand { get; private set; }
		public DelegateCommand SelectChoiceVideoDirCommand { get; private set; }
		public DelegateCommand SelectAnswerSoundCommand { get; private set; }

		public DelegateCommand SearchCommand { get; private set; }
		public DelegateCommand AddTeamCommand { get; private set; }
		public DelegateCommand DelTeamCommand { get; private set; }

		public string EarlyVideoDir
		{
			get { return this.earlyVideoDir; }
			set { SetProperty( ref this.earlyVideoDir, value ); }
		}

		public string ChoiceVideoDir
		{
			get { return this.choiceVideoDir; }
			set { SetProperty( ref this.choiceVideoDir, value ); }
		}
				
		/// <summary>
		/// チームのリスト
		/// </summary>
		public ObservableHashCollection<TeamSettingVM> Teams
		{
			get { return this.teams; }
		}

		/// <summary>
		/// メンバーのリスト
		/// </summary>
		public ObservableHashVMCollection<MemberSettingVM> Members
		{
			get { return this.members; }
		}

		/// <summary>
		/// 選択しているチーム
		/// </summary>
		public TeamSettingVM SelectedTeam
		{
			get { return this.selectedTeam; }
			set { SetProperty( ref this.selectedTeam, value, SeletedTeamChanged ); }
		}

		/// <summary>
		/// 選択しているメンバー
		/// </summary>
		public MemberSettingVM SelectedMember
		{
			get { return selectedMember; }
			set { SetProperty( ref selectedMember, value, SelectedMemberChanged ); }
		}

		/// <summary>
		/// デバイスのリスト
		/// </summary>
		public ObservableHashCollection<string> Devices
		{
			get
			{
				return this.devices;
			}
		}

		/// <summary>
		/// デバイスの更新間隔
		/// </summary>
		public long UpdateTime
		{
			get { return updateTime; }
			set { SetProperty( ref updateTime, value ); }
		}

		#endregion

		public OperateSettingVM( MainVM parent )
			: base( parent )
		{
			this.SelectEarlyVideoDirCommand = new DelegateCommand( SelectEarlyVideoDir, null );
			this.SelectChoiceVideoDirCommand = new DelegateCommand( SelectChoiceVideoDir, null );
			this.SelectAnswerSoundCommand = new DelegateCommand( SelectAnwser, null );
			this.AddMemberCommand = new DelegateCommand( AddMember, null );
			this.DelMemberCommand = new DelegateCommand( DelMember, CanDelMember );

			this.SearchCommand = new DelegateCommand( SearchDevice, null );
			this.AddTeamCommand = new DelegateCommand( AddTeam );
			this.DelTeamCommand = new DelegateCommand( DelTeam, CanDelTeam );

			this.Parent.Manager.Devices.CollectionChanged += Devices_CollectionChanged;
			this.Parent.Manager.PropertyChanged += Manager_PropertyChanged;
		}

		public override UIElement PlayView
		{
			get { return null; }
		}

		#region 設定読み書き

		public override void LoadData()
		{
			base.LoadData();

			this.EarlyVideoDir = this.Parent.Data.EarlyVideoDir;
			this.ChoiceVideoDir = this.Parent.Data.ChoiceVideoDir;

			this.teamAdapter = new ViewModelsAdapter<TeamSettingVM, TeamData>( CreateTeamVM, DeleteTeamVM );
			this.teamAdapter.Adapt( this.Teams, this.Parent.Data.TeamList );

		}

		/// <summary>
		/// チームのモデルからVMを作る時の処理
		/// </summary>
		/// <param name="data">モデル</param>
		/// <returns>VM</returns>
		private TeamSettingVM CreateTeamVM( TeamData data )
		{
			var vm = new TeamSettingVM( data );
			vm.Members.CollectionChanged += Members_CollectionChanged;
			return vm;
		}

		/// <summary>
		/// VMが削除されたときの処理
		/// </summary>
		/// <param name="vm"></param>
		private void DeleteTeamVM( TeamSettingVM vm )
		{
			this.Members.RemoveWhere( m => m.Parent == vm );
			vm.Members.CollectionChanged -= Members_CollectionChanged;
		}

		#endregion

		#region アクティブ

		public override void Activate()
		{
			this.Parent.Manager.KeyPushed += Manager_KeyPushed;
		}

		public override void Deactivate()
		{
			this.Parent.Manager.KeyPushed -= Manager_KeyPushed;
		}

		#endregion

		#region イベント

		/// <summary>
		/// 更新にかかった時間をアップデートします。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Manager_PropertyChanged( object sender, PropertyChangedEventArgs e )
		{
			if( e.PropertyName == "UpdateTime" )
			{
				this.UpdateTime = this.Parent.Manager.UpdateTime;
			}
		}

		/// <summary>
		/// メンバーが追加削除されたときにメンバー一覧を更新します。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Members_CollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
		{
			if( e.OldItems != null )
			{
				foreach( MemberSettingVM member in e.OldItems )
				{
					this.Members.Remove( member );
				}
			}

			if( e.NewItems != null )
			{
				foreach( MemberSettingVM member in e.NewItems )
				{
					this.Members.Add( member );
				}
			}
		}

		/// <summary>
		/// 選択しているチームが変わったとき削除条件を再チェックします。
		/// </summary>
		private void SeletedTeamChanged()
		{
			this.DelTeamCommand.RaiseCanExecuteChanged();
		}

		/// <summary>
		/// 選択しているメンバーが変わったとき、削除条件を再チェックします。
		/// </summary>
		private void SelectedMemberChanged()
		{
			this.DelMemberCommand.RaiseCanExecuteChanged();
		}

		/// <summary>
		/// ボタンが押された時の処理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Manager_KeyPushed( object sender, DeviceKeyEventArgs e )
		{
			if( this.SelectedMember != null )
			{
				this.SelectedMember.Model.DeviceGuid = e.InstanceID;
				this.SelectedMember.Model.Key = e.Key;
			}
		}

		/// <summary>
		/// 検出しているデバイスが変化したときにリストアップします。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Devices_CollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
		{
			switch( e.Action )
			{
				case NotifyCollectionChangedAction.Add:
				case NotifyCollectionChangedAction.Remove:
				case NotifyCollectionChangedAction.Replace:
					if( e.NewItems != null )
					{
						foreach( Device d in e.NewItems )
						{
							this.devices.Add( d.Information.InstanceName );
						}
					}
					if( e.OldItems != null )
					{
						foreach( Device d in e.OldItems )
						{
							this.devices.Remove( d.Information.InstanceName );
						}
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					this.devices.Clear();
					break;
				default:
					break;
			}
		}

		#endregion

		#region コマンド

		/// <summary>
		/// メディアフォルダの選択
		/// </summary>
		/// <param name="obj"></param>
		private void SelectEarlyVideoDir( object obj )
		{
			VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
			if( string.IsNullOrEmpty( this.Parent.Data.EarlyVideoDir ) )
			{
				dlg.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;
			}
			else
			{
				dlg.SelectedPath = this.Parent.Data.EarlyVideoDir;
			}
			if( dlg.ShowDialog() == true )
			{
				this.Parent.Data.EarlyVideoDir = dlg.SelectedPath;
				this.EarlyVideoDir = this.Parent.Data.EarlyVideoDir;
			}
		}

		/// <summary>
		/// メディアフォルダの選択
		/// </summary>
		/// <param name="obj"></param>
		private void SelectChoiceVideoDir( object obj )
		{
			VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
			if( string.IsNullOrEmpty( this.Parent.Data.ChoiceVideoDir ) )
			{
				dlg.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;
			}
			else
			{
				dlg.SelectedPath = this.Parent.Data.ChoiceVideoDir;
			}
			if( dlg.ShowDialog() == true )
			{
				this.Parent.Data.ChoiceVideoDir = dlg.SelectedPath;
				this.ChoiceVideoDir = this.Parent.Data.ChoiceVideoDir;
			}
		}

		/// <summary>
		/// 解答音の選択
		/// </summary>
		/// <param name="obj"></param>
		private void SelectAnwser( object obj )
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
			dlg.Multiselect = false;
			if( dlg.ShowDialog() == true )
			{
				this.AnswerSound = new MediaVM() { FilePath = dlg.FileName };
				this.Parent.Data.AnswerSoundPath = dlg.FileName;
			}
		}

		/// <summary>
		/// メンバーの追加
		/// </summary>
		/// <param name="obj"></param>
		private void AddMember( object obj )
		{
			var team = obj as TeamData;
			team.Members.Add( new MemberData() );
		}

		/// <summary>
		/// メンバーを削除できるかチェック
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		private bool CanDelMember( object obj )
		{
			return this.SelectedMember != null;
		}

		/// <summary>
		/// メンバーの削除
		/// </summary>
		/// <param name="obj"></param>
		private void DelMember( object obj )
		{
			this.SelectedMember.Parent.Model.Members.Remove( this.SelectedMember.Model );
			this.SelectedMember = null;
		}

		/// <summary>
		/// デバイスの検索
		/// </summary>
		/// <param name="obj"></param>
		private void SearchDevice( object obj )
		{
			this.Parent.Manager.SearchDevice();
		}

		/// <summary>
		/// チームの追加
		/// </summary>
		/// <param name="obj"></param>
		private void AddTeam( object obj )
		{
			var team = new TeamData() { TeamName = "チーム" };
			for( int i = 0; i < 4; i++ )
			{
				team.Members.Add( new MemberData() );
			}
			this.Parent.Data.TeamList.Add( team );
		}

		/// <summary>
		/// チームを削除できるかチェック
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		private bool CanDelTeam( object obj )
		{
			return this.SelectedTeam != null;
		}

		/// <summary>
		/// チームの削除
		/// </summary>
		/// <param name="obj"></param>
		private void DelTeam( object obj )
		{
			this.Parent.Data.TeamList.Remove( this.SelectedTeam.Model );
			this.SelectedTeam = null;
		}

		#endregion

	}
}
