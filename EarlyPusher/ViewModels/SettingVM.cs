using EarlyPusher.Manager;
using EarlyPusher.Models;
using SlimDX.DirectInput;
using StFrLibs.Core.Basis;
using StFrLibs.Core.Commands;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using System.Xml.Serialization;
using System.Windows.Media;
using Microsoft.Win32;
using EarlyPusher.Views;
using Ookii.Dialogs.Wpf;

namespace EarlyPusher.ViewModels
{
	public class SettingVM : ViewModelBase
	{
		private SettingData data;

		private SettingOnlyVM onlyVM;

		private Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
		private DeviceManager manager;
		private ObservableHashCollection<MemberVM> members = new ObservableHashCollection<MemberVM>();
		private ObservableHashCollection<VideoVM> videos = new ObservableHashCollection<VideoVM>();
		private bool isSettingMode = true;
		private int rank = 0;
		private string videoDir;

		private SoundVM anserSound;
		private MemberVM selectedMember;
		private PlayWindow window;

		private Uri videoSource;
		private bool isVideoPlaying;

		#region プロパティ

		public DelegateCommand LoadedCommand { get; private set; }
		public DelegateCommand ClosingCommand { get; private set; }

		public DelegateCommand AddMemberCommand { get; private set; }
		public DelegateCommand DelMemberCommand { get; private set; }

		public DelegateCommand StartCommand { get; private set; }
		public DelegateCommand ResetCommand { get; private set; }
		public DelegateCommand WindowCommand { get; private set; }
		public DelegateCommand WindowMaxCommand { get; private set; }

		public DelegateCommand SelectVideoDirCommand { get; private set; }
		public DelegateCommand SelectAnserSoundCommand { get; private set; }

		public DelegateCommand PlayVideoCommand { get; private set; }

		public DeviceManager Manager
		{
			get { return this.manager; }
		}

		public SettingOnlyVM SettingOnlyVM
		{
			get { return this.onlyVM; }
			set { SetProperty( ref this.onlyVM, value ); }
		}
		
		public ObservableHashCollection<MemberVM> Members
		{
			get
			{
				return this.members;
			}
		}

		public ObservableHashCollection<VideoVM> Videos
		{
			get
			{
				return this.videos;
			}
		}
		
		public MemberVM SelectedMember
		{
			get { return selectedMember; }
			set { SetProperty( ref selectedMember, value, SelectedPanelChanged ); }
		}

		public bool IsSettingMode
		{
			get { return isSettingMode; }
			set { SetProperty( ref isSettingMode, value, SettingChanged ); }
		}

		public string VideoDir
		{
			get { return videoDir; }
			set { SetProperty( ref videoDir, value, LoadVideos ); }
		}

		public SoundVM AnserSound
		{
			get { return anserSound; }
			set { SetProperty( ref anserSound, value ); }
		}

		public Uri VideoSource
		{
			get { return videoSource; }
			set { SetProperty( ref videoSource, value ); }
		}

		public bool IsVideoPlaying
		{
			get { return isVideoPlaying; }
			set { SetProperty( ref isVideoPlaying, value ); }
		}


		#endregion

		public SettingVM()
		{
			this.LoadedCommand = new DelegateCommand( Inited, null );
			this.ClosingCommand = new DelegateCommand( Closing, null );
			this.AddMemberCommand = new DelegateCommand( AddMember, null );
			this.DelMemberCommand = new DelegateCommand( DelMember, CanDelMember );
			this.StartCommand = new DelegateCommand( Start, null );
			this.ResetCommand = new DelegateCommand( Reset, null );
			this.WindowCommand = new DelegateCommand( ShowCloseWindow, null );
			this.WindowMaxCommand = new DelegateCommand( MaximazeWindow, CanMaximaize );
			this.SelectVideoDirCommand = new DelegateCommand( SelectVideoDir, null );
			this.SelectAnserSoundCommand = new DelegateCommand( SelectAnser, null );
			this.PlayVideoCommand = new DelegateCommand( PlayVideo );


			this.manager = new DeviceManager();
			this.manager.KeyPushed += Manager_KeyPushed;
		}

		#region コマンド関係

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
				this.window.DataContext = new PlayViewModel( this.SettingOnlyVM.Teams );
				this.window.Show();
			}
			this.WindowMaxCommand.RaiseCanExecuteChanged();
		}

		private bool CanMaximaize( object obj )
		{
			return this.window != null;
		}

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

		private void Reset( object obj )
		{
			foreach( var item in this.Members )
			{
				item.CanAnswer = true;
			}
		}

		private void Start( object obj )
		{
			rank = 0;
			InitRank();
		}

		private void AddMember( object obj )
		{
			var team = obj as TeamData;
			team.Members.Add( new MemberData() );
		}

		private bool CanDelMember( object obj )
		{
			return this.SelectedMember != null;
		}

		private void DelMember( object obj )
		{
			this.SelectedMember.Parent.Model.Members.Remove( this.SelectedMember.Model );
			this.SelectedMember = null;
		}

		private void SelectVideoDir( object obj )
		{
			VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
			if( string.IsNullOrEmpty(this.VideoDir) )
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

		private void SelectAnser( object obj )
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
			dlg.Multiselect = false;
			if( dlg.ShowDialog() == true )
			{
				this.AnserSound = new SoundVM() { Path = dlg.FileName };
			}
		}

		private void Inited( object obj )
		{
			LoadData();
		}

		private void Closing( object obj )
		{
			SaveData();
			if( this.window != null )
			{
				this.window.Close();
			}

			this.manager.Dispose();
		}

		private void PlayVideo( object obj )
		{
			Contract.Assert( obj is string );
			this.IsVideoPlaying = false;
			this.VideoSource = new Uri( obj as string );
		}

		#endregion

		#region イベント関係

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
					if( rank == 1 && this.AnserSound.PlayCommand.CanExecute( null ) )
					{
						this.AnserSound.PlayCommand.Execute( null );
					}
					this.IsVideoPlaying = false;
				}
			}
		}

		private void SelectedPanelChanged( bool obj )
		{
			this.DelMemberCommand.RaiseCanExecuteChanged();
		}

		#endregion

		#region 設定

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

			this.SettingOnlyVM = new SettingOnlyVM( this.data, this );

			this.VideoDir = this.data.VideoDir;
			this.AnserSound = new SoundVM() { Path = this.data.AnserSoundPath };
		}

		/// <summary>
		/// 設定を保存します。
		/// </summary>
		public void SaveData()
		{
			this.data.VideoDir = this.VideoDir;
			this.data.AnserSoundPath = this.AnserSound.Path;

			using( Stream file = new FileStream( SettingData.FileName, FileMode.Create ) )
			{
				XmlSerializer xml = new XmlSerializer( typeof( SettingData ) );
				xml.Serialize( file, this.data );
			}
		}

		#endregion

		private void SettingChanged( bool isSucceed )
		{
			rank = 0;
			this.SelectedMember = null;
			InitRank();
		}

		private void LoadVideos( bool isSucceed )
		{
			if( isSucceed && !string.IsNullOrEmpty( this.VideoDir ) )
			{
				this.Videos.Clear();
				foreach( string path in Directory.EnumerateFiles( this.VideoDir, "*", SearchOption.AllDirectories ) )
				{
					this.Videos.Add( new VideoVM() { FilePath = path } );
				}
			}
		}

		private void InitRank()
		{
			foreach( var i in this.Members )
			{
				i.Rank = string.Empty;
			}
		}

		public void DispatcherInvoke( Action action )
		{
			if( !this.dispatcher.CheckAccess() )
			{
				this.dispatcher.BeginInvoke( action );
			}
			else
			{
				action();
			}
		}
	}
}
