using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using System.Xml.Serialization;
using EarlyPusher.Basis;
using EarlyPusher.Manager;
using EarlyPusher.Models;
using EarlyPusher.Views;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using SlimDX.DirectInput;
using StFrLibs.Core.Basis;
using StFrLibs.Core.Commands;

namespace EarlyPusher.ViewModels
{
	public class SettingVM : ViewModelBase
	{
		private SettingData data;

		private DeviceManager manager;

		private ObservableHashVMCollection<TeamVM> teams = new ObservableHashVMCollection<TeamVM>();
		private ObservableHashCollection<MediaVM> medias = new ObservableHashCollection<MediaVM>();
		private ObservableHashCollection<string> devices = new ObservableHashCollection<string>();

		private ViewModelsAdapter<TeamVM,TeamData> teamAdapter;

		private PlayMode mode;
		private bool isSettingMode = true;
		private int rank = 0;
		private string videoDir;
		private long updateTime;

		private TeamVM selectedTeam;
		private MemberVM selectedMember;
		private MediaVM answerSound;
		private MediaVM selectedMedia;
		private PlayWindow window;

		#region プロパティ

		public DelegateCommand LoadedCommand { get; private set; }
		public DelegateCommand ClosingCommand { get; private set; }

		public DelegateCommand AddMemberCommand { get; private set; }
		public DelegateCommand DelMemberCommand { get; private set; }
		public DelegateCommand SelectVideoDirCommand { get; private set; }
		public DelegateCommand SelectAnswerSoundCommand { get; private set; }

		public DelegateCommand StartCommand { get; private set; }
		public DelegateCommand ResetCommand { get; private set; }
		public DelegateCommand WindowCommand { get; private set; }
		public DelegateCommand WindowMaxCommand { get; private set; }

		public DelegateCommand SearchCommand { get; private set; }
		public DelegateCommand AddTeamCommand { get; private set; }
		public DelegateCommand DelTeamCommand { get; private set; }

		/// <summary>
		/// ボタンデバイスの管理クラス
		/// </summary>
		public DeviceManager Manager
		{
			get { return this.manager; }
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
		/// チームのリスト
		/// </summary>
		public ObservableHashCollection<TeamVM> Teams
		{
			get { return this.teams; }
		}

		public IEnumerable<IEnumerable<MemberVM>> Choices
		{
			get
			{
				return this.Members.GroupBy( m => m.Parent.Members.IndexOf( m ) );
			}
		}
		
		/// <summary>
		/// メンバーのリスト
		/// </summary>
		public IEnumerable<MemberVM> Members
		{
			get
			{
				return this.Teams.SelectMany( t => t.Members );
			}
		}

		/// <summary>
		/// メディアのリスト
		/// </summary>
		public ObservableHashCollection<MediaVM> Medias
		{
			get
			{
				return this.medias;
			}
		}

		/// <summary>
		/// 選択しているチーム
		/// </summary>
		public TeamVM SelectedTeam
		{
			get { return this.selectedTeam; }
			set { SetProperty( ref this.selectedTeam, value, SeletedTeamChanged ); }
		}

		/// <summary>
		/// 選択しているメンバー
		/// </summary>
		public MemberVM SelectedMember
		{
			get { return selectedMember; }
			set { SetProperty( ref selectedMember, value, SelectedMemberChanged ); }
		}

		/// <summary>
		/// 選択しているメディア
		/// </summary>
		public MediaVM SelectedMedia
		{
			get { return this.selectedMedia; }
			set { SetProperty( ref this.selectedMedia, value, null, SelectedMediaChanging ); }
		}
		
		/// <summary>
		/// 設定モードかどうか
		/// </summary>
		public bool IsSettingMode
		{
			get { return isSettingMode; }
			set { SetProperty( ref isSettingMode, value, SettingChanged ); }
		}

		/// <summary>
		/// プレイモード
		/// </summary>
		public PlayMode Mode
		{
			get { return this.mode; }
			set { SetProperty( ref this.mode, value ); }
		}

		/// <summary>
		/// メディアフォルダのパス
		/// </summary>
		public string VideoDir
		{
			get { return videoDir; }
			set { SetProperty( ref videoDir, value, LoadVideos ); }
		}

		/// <summary>
		/// 解答音
		/// </summary>
		public MediaVM AnswerSound
		{
			get { return answerSound; }
			set { SetProperty( ref answerSound, value ); }
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

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SettingVM()
		{
			this.LoadedCommand = new DelegateCommand( Inited, null );
			this.ClosingCommand = new DelegateCommand( Closing, null );
			this.WindowCommand = new DelegateCommand( ShowCloseWindow, null );
			this.WindowMaxCommand = new DelegateCommand( MaximazeWindow, CanMaximaize );
	
			this.StartCommand = new DelegateCommand( Start, null );
			this.ResetCommand = new DelegateCommand( Reset, null );
			this.SelectVideoDirCommand = new DelegateCommand( SelectVideoDir, null );
			this.SelectAnswerSoundCommand = new DelegateCommand( SelectAnwser, null );
			this.AddMemberCommand = new DelegateCommand( AddMember, null );
			this.DelMemberCommand = new DelegateCommand( DelMember, CanDelMember );

			this.SearchCommand = new DelegateCommand( SearchDevice, null );
			this.AddTeamCommand = new DelegateCommand( AddTeam );
			this.DelTeamCommand = new DelegateCommand( DelTeam, CanDelTeam );


			this.manager = new DeviceManager();
			this.manager.KeyPushed += Manager_KeyPushed;
			this.manager.PropertyChanged += Manager_PropertyChanged;
			this.manager.Devices.CollectionChanged += Devices_CollectionChanged;

			this.teams.CollectionChanged += Members_CollectionChanged;
		}

		#region コマンド関係

		/// <summary>
		/// ウィンドウが開くときの処理
		/// </summary>
		/// <param name="obj"></param>
		private void Inited( object obj )
		{
			LoadData();
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
				this.window.DataContext = this;
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
		/// 解答順位の初期化
		/// </summary>
		/// <param name="obj"></param>
		private void Start( object obj )
		{
			rank = 0;
			InitRank();
		}

		/// <summary>
		/// 解答権のリセット
		/// </summary>
		/// <param name="obj"></param>
		private void Reset( object obj )
		{
			foreach( var item in this.Members )
			{
				item.CanAnswer = true;
			}
		}

		/// <summary>
		/// メディアフォルダの選択
		/// </summary>
		/// <param name="obj"></param>
		private void SelectVideoDir( object obj )
		{
			VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
			if( string.IsNullOrEmpty( this.VideoDir ) )
			{
				dlg.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;
			}
			else
			{
				dlg.SelectedPath = this.VideoDir;
			}
			if( dlg.ShowDialog() == true )
			{
				this.VideoDir = dlg.SelectedPath;
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
			this.Manager.SearchDevice();
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
			this.data.TeamList.Add( team );
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
			this.data.TeamList.Remove( this.SelectedTeam.Model );
			this.SelectedTeam = null;
		}

		#endregion

		#region イベント関係

		/// <summary>
		/// ボタンが押された時の処理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Manager_KeyPushed( object sender, DeviceKeyEventArgs e )
		{
			if( this.IsSettingMode )
			{
				if( this.SelectedMember != null )
				{
					Contract.Assert( this.SelectedMember.Model != null );
					this.SelectedMember.Model.DeviceGuid = e.InstanceID;
					this.SelectedMember.Model.Key = e.Key;
				}
			}
			else
			{
				var item = this.Members.FirstOrDefault( i => i.Model.DeviceGuid == e.InstanceID && i.Model.Key == e.Key );
				if( item != null && string.IsNullOrEmpty( item.Rank ) && item.CanAnswer )
				{
					rank++;
					item.Rank = rank.ToString();
					if( !this.AnswerSound.IsPlaying )
					{
						this.AnswerSound.Play();
					}
					if( this.SelectedMedia != null )
					{
						this.SelectedMedia.Pause();
					}
				}
			}
		}

		/// <summary>
		/// 更新にかかった時間をアップデートします。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Manager_PropertyChanged( object sender, PropertyChangedEventArgs e )
		{
			if( e.PropertyName == "UpdateTime" )
			{
				this.UpdateTime = this.Manager.UpdateTime;
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

		/// <summary>
		/// メンバーが追加削除されたときにメンバー一覧を更新します。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Members_CollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
		{
			NotifyPropertyChanged( () => this.Members );
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
		/// 選択しているメディアが変わるとき、以前のメディアを停止します。
		/// </summary>
		private void SelectedMediaChanging()
		{
			if( this.SelectedMedia != null )
			{
				this.SelectedMedia.Stop();
			}
		}

		/// <summary>
		/// 設定モードが切り替わったときの処理
		/// </summary>
		private void SettingChanged()
		{
			rank = 0;
			this.SelectedMember = null;
			InitRank();
		}

		/// <summary>
		/// メディアフォルダが変更されたとき、メディア一覧を更新します。
		/// </summary>
		private void LoadVideos()
		{
			if( !string.IsNullOrEmpty( this.VideoDir ) )
			{
				this.Medias.Clear();
				foreach( string path in Directory.EnumerateFiles( this.VideoDir, "*", SearchOption.AllDirectories ) )
				{
					this.Medias.Add( new MediaVM() { FilePath = path } );
				}
			}
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
				while( team.Members.Count != 4 )
				{
					team.Members.Add( new MemberData() );
				}
			}

			this.teamAdapter = new ViewModelsAdapter<TeamVM, TeamData>( CreateTeamVM, DeleteTeamVM );
			this.teamAdapter.Adapt( this.Teams, this.data.TeamList );

			this.VideoDir = this.data.VideoDir;
			this.AnswerSound = new MediaVM() { FilePath = this.data.AnswerSoundPath };
		}

		/// <summary>
		/// 設定を保存します。
		/// </summary>
		public void SaveData()
		{
			this.data.VideoDir = this.VideoDir;
			this.data.AnswerSoundPath = this.AnswerSound.FilePath;

			using( Stream file = new FileStream( SettingData.FileName, FileMode.Create ) )
			{
				XmlSerializer xml = new XmlSerializer( typeof( SettingData ) );
				xml.Serialize( file, this.data );
			}
		}

		/// <summary>
		/// チームのモデルからVMを作る時の処理
		/// </summary>
		/// <param name="data">モデル</param>
		/// <returns>VM</returns>
		private TeamVM CreateTeamVM( TeamData data )
		{
			var vm = new TeamVM( data );
			vm.Members.CollectionChanged += Members_CollectionChanged;
			return vm;
		}

		/// <summary>
		/// VMが削除されたときの処理
		/// </summary>
		/// <param name="vm"></param>
		private void DeleteTeamVM( TeamVM vm )
		{
			vm.Members.CollectionChanged -= Members_CollectionChanged;
		}

		#endregion

		/// <summary>
		/// 順位を初期化します。
		/// </summary>
		private void InitRank()
		{
			foreach( var i in this.Members )
			{
				i.Rank = string.Empty;
			}
		}
	}
}
