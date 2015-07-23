using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using StFrLibs.Core.Extensions;

namespace EarlyPusher.Modules.Setting1Tab.ViewModels
{
	public class OperateSetting1VM : OperateTabVMBase
	{
		private ObservableHashVMCollection<TeamSetting1VM> teams = new ObservableHashVMCollection<TeamSetting1VM>();
		private ObservableHashVMCollection<MemberSetting1VM> members = new ObservableHashVMCollection<MemberSetting1VM>();
		private ObservableCollection<string> devices = new ObservableCollection<string>();
		private ViewModelsAdapter<TeamSetting1VM,TeamData> teamAdapter;

		private string earlyVideoDir;
		private string choiceVideoDir;
		private string sortVideoDir;

		private long updateTime;

		private TeamSetting1VM selectedTeam;
		private MemberSetting1VM selectedMember;
		private MediaVM standSound;
		private MediaVM questionSound;
		private MediaVM answerSound;
		private MediaVM correctSound;
		private MediaVM missSound;
		private MediaVM checkSound;

		#region プロパティ

		public DelegateCommand AddMemberCommand { get; private set; }
		public DelegateCommand DelMemberCommand { get; private set; }
		public DelegateCommand AllKeyLockCommand { get; private set; }
		public DelegateCommand AllKeyReleaseCommand { get; private set; }

		public DelegateCommand SelectEarlyVideoDirCommand { get; private set; }
		public DelegateCommand SelectChoiceVideoDirCommand { get; private set; }
		public DelegateCommand SelectSortVideoDirCommand { get; private set; }
		public DelegateCommand SelectStandSoundCommand { get; private set; }
		public DelegateCommand SelectQuestionSoundCommand { get; private set; }
		public DelegateCommand SelectAnswerSoundCommand { get; private set; }
		public DelegateCommand SelectCorrectSoundCommand { get; private set; }
		public DelegateCommand SelectMissSoundCommand { get; private set; }
		public DelegateCommand SelectCheckSoundCommand { get; private set; }

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

		public string SortVideoDir
		{
			get { return this.sortVideoDir; }
			set { SetProperty( ref this.sortVideoDir, value ); }
		}
						
		/// <summary>
		/// チームのリスト
		/// </summary>
		public ObservableHashCollection<TeamSetting1VM> Teams
		{
			get { return this.teams; }
		}

		/// <summary>
		/// メンバーのリスト
		/// </summary>
		public ObservableHashVMCollection<MemberSetting1VM> Members
		{
			get { return this.members; }
		}

		/// <summary>
		/// 選択しているチーム
		/// </summary>
		public TeamSetting1VM SelectedTeam
		{
			get { return this.selectedTeam; }
			set { SetProperty( ref this.selectedTeam, value, SeletedTeamChanged ); }
		}

		/// <summary>
		/// 選択しているメンバー
		/// </summary>
		public MemberSetting1VM SelectedMember
		{
			get { return selectedMember; }
			set { SetProperty( ref selectedMember, value, SelectedMemberChanged ); }
		}

		/// <summary>
		/// デバイスのリスト
		/// </summary>
		public ObservableCollection<string> Devices
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

		public MediaVM StandSound
		{
			get { return this.standSound; }
			set { SetProperty( ref this.standSound, value ); }
		}

		public MediaVM QuestionSound
		{
			get { return this.questionSound; }
			set { SetProperty( ref this.questionSound, value ); }
		}
		
		public MediaVM AnswerSound
		{
			get { return answerSound; }
			set { SetProperty( ref answerSound, value ); }
		}

		public MediaVM CorrectSound
		{
			get { return correctSound; }
			set { SetProperty( ref correctSound, value ); }
		}

		public MediaVM MissSound
		{
			get { return missSound; }
			set { SetProperty( ref missSound, value ); }
		}

		public MediaVM CheckSound
		{
			get { return checkSound; }
			set { SetProperty( ref checkSound, value ); }
		}

		#endregion

		public OperateSetting1VM( MainVM parent )
			: base( parent )
		{
			this.SelectEarlyVideoDirCommand = new DelegateCommand( SelectEarlyVideoDir, null );
			this.SelectChoiceVideoDirCommand = new DelegateCommand( SelectChoiceVideoDir, null );
			this.SelectSortVideoDirCommand = new DelegateCommand( SelectSortVideoDir, null );
			this.SelectStandSoundCommand = new DelegateCommand( SelectStandSound, null );
			this.SelectQuestionSoundCommand = new DelegateCommand( SelectQuestionSound, null );
			this.SelectAnswerSoundCommand = new DelegateCommand( SelectAnwser, null );
			this.SelectCorrectSoundCommand = new DelegateCommand( SelectCorrectSound, null );
			this.SelectMissSoundCommand = new DelegateCommand( SelectMissSound, null );
			this.SelectCheckSoundCommand = new DelegateCommand( SelectCheckSound, null );

			this.AddMemberCommand = new DelegateCommand( AddMember, null );
			this.DelMemberCommand = new DelegateCommand( DelMember, CanDelMember );

			this.AllKeyLockCommand = new DelegateCommand( AllKeyLock );
			this.AllKeyReleaseCommand = new DelegateCommand( AllKeyRelease );

			this.SearchCommand = new DelegateCommand( SearchDevice, null );
			this.AddTeamCommand = new DelegateCommand( AddTeam );
			this.DelTeamCommand = new DelegateCommand( DelTeam, CanDelTeam );

			this.Parent.Manager.Devices.CollectionChanged += Devices_CollectionChanged;
			this.Parent.Manager.PropertyChanged += Manager_PropertyChanged;
		}

		private void AllKeyLock( object obj )
		{
			this.Members.ForEach( m => m.IsKeyLock = true );
		}

		private void AllKeyRelease( object obj )
		{
			this.Members.ForEach( m => m.IsKeyLock = false );
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
			this.SortVideoDir = this.Parent.Data.SortVideoDir;

			this.teamAdapter = new ViewModelsAdapter<TeamSetting1VM, TeamData>( CreateTeamVM, DeleteTeamVM );
			this.teamAdapter.Adapt( this.Teams, this.Parent.Data.TeamList );

			if( !string.IsNullOrEmpty( this.Parent.Data.StandSoundPath ) )
			{
				this.StandSound = new MediaVM();
				this.StandSound.FilePath = this.Parent.Data.StandSoundPath;
				this.StandSound.LoadFile();
			}

			if( !string.IsNullOrEmpty( this.Parent.Data.QuestionSoundPath ) )
			{
				this.QuestionSound = new MediaVM();
				this.QuestionSound.FilePath = this.Parent.Data.QuestionSoundPath;
				this.QuestionSound.LoadFile();
			}

			if( !string.IsNullOrEmpty( this.Parent.Data.AnswerSoundPath ) )
			{
				this.AnswerSound = new MediaVM();
				this.AnswerSound.FilePath = this.Parent.Data.AnswerSoundPath;
				this.AnswerSound.LoadFile();
			}

			if( !string.IsNullOrEmpty( this.Parent.Data.CorrectSoundPath ) )
			{
				this.CorrectSound = new MediaVM();
				this.CorrectSound.FilePath = this.Parent.Data.CorrectSoundPath;
				this.CorrectSound.LoadFile();
			}

			if( !string.IsNullOrEmpty( this.Parent.Data.MissSoundPath ) )
			{
				this.MissSound = new MediaVM();
				this.MissSound.FilePath = this.Parent.Data.MissSoundPath;
				this.MissSound.LoadFile();
			}

			if( !string.IsNullOrEmpty( this.Parent.Data.CheckSoundPath ) )
			{
				this.CheckSound = new MediaVM();
				this.CheckSound.FilePath = this.Parent.Data.CheckSoundPath;
				this.CheckSound.LoadFile();
			}
		}

		/// <summary>
		/// チームのモデルからVMを作る時の処理
		/// </summary>
		/// <param name="data">モデル</param>
		/// <returns>VM</returns>
		private TeamSetting1VM CreateTeamVM( TeamData data )
		{
			var vm = new TeamSetting1VM( data );
			vm.Members.CollectionChanged += Members_CollectionChanged;
			return vm;
		}

		/// <summary>
		/// VMが削除されたときの処理
		/// </summary>
		/// <param name="vm"></param>
		private void DeleteTeamVM( TeamSetting1VM vm )
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
				foreach( MemberSetting1VM member in e.OldItems )
				{
					this.Members.Remove( member );
				}
			}

			if( e.NewItems != null )
			{
				foreach( MemberSetting1VM member in e.NewItems )
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
			if( this.SelectedMember != null && !this.SelectedMember.IsKeyLock )
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

		private void SelectSortVideoDir( object obj )
		{
			VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
			if( string.IsNullOrEmpty( this.Parent.Data.SortVideoDir ) )
			{
				dlg.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;
			}
			else
			{
				dlg.SelectedPath = this.Parent.Data.SortVideoDir;
			}
			if( dlg.ShowDialog() == true )
			{
				this.Parent.Data.SortVideoDir = dlg.SelectedPath;
				this.SortVideoDir = this.Parent.Data.SortVideoDir;
			}
		}

		private void SelectStandSound( object obj )
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
			dlg.Multiselect = false;
			if( dlg.ShowDialog() == true )
			{
				this.Parent.Data.StandSoundPath = dlg.FileName;

				this.StandSound = new MediaVM() { FilePath = dlg.FileName };
				this.StandSound.LoadFile();
			}
		}

		private void SelectQuestionSound( object obj )
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
			dlg.Multiselect = false;
			if( dlg.ShowDialog() == true )
			{
				this.Parent.Data.QuestionSoundPath = dlg.FileName;

				this.QuestionSound = new MediaVM() { FilePath = dlg.FileName };
				this.QuestionSound.LoadFile();
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
				this.Parent.Data.AnswerSoundPath = dlg.FileName;

				this.AnswerSound = new MediaVM() { FilePath = dlg.FileName };
				this.AnswerSound.LoadFile();
			}
		}

		private void SelectCorrectSound( object obj )
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
			dlg.Multiselect = false;
			if( dlg.ShowDialog() == true )
			{
				this.Parent.Data.CorrectSoundPath = dlg.FileName;

				this.CorrectSound = new MediaVM() { FilePath = dlg.FileName };
				this.CorrectSound.LoadFile();
			}
		}

		private void SelectMissSound( object obj )
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
			dlg.Multiselect = false;
			if( dlg.ShowDialog() == true )
			{
				this.Parent.Data.MissSoundPath = dlg.FileName;

				this.MissSound = new MediaVM() { FilePath = dlg.FileName };
				this.MissSound.LoadFile();
			}
		}

		private void SelectCheckSound( object obj )
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
			dlg.Multiselect = false;
			if( dlg.ShowDialog() == true )
			{
				this.Parent.Data.CheckSoundPath = dlg.FileName;

				this.CheckSound = new MediaVM() { FilePath = dlg.FileName };
				this.CheckSound.LoadFile();
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
