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

		private Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
		private StringBuilder logBuilder;
		private DeviceManager manager;
		private DispatchObservableCollection<string> devices;
		private DispatchObservableCollection<ItemVM> items;
		private DispatchObservableCollection<SoundVM> sounds;
		private DispatchObservableCollection<VideoVM> videos;
		private long updateTime;
		private bool isSettingMode = true;
		private int rank = 0;
		private int wrapCount = 1;
		private string videoDir;

		private SoundVM anserSound;
		private ItemVM selectedPanel;
		private SoundVM selectedSound;
		private PlayWindow window;

		private Uri videoSource;
		private bool isVideoPlaying;

		#region プロパティ

		public DelegateCommand SearchCommand { get; private set; }

		public DelegateCommand LoadedCommand { get; private set; }
		public DelegateCommand ClosingCommand { get; private set; }

		public DelegateCommand AddPanelCommand { get; private set; }
		public DelegateCommand DelPanelCommand { get; private set; }

		public DelegateCommand StartCommand { get; private set; }
		public DelegateCommand ResetCommand { get; private set; }
		public DelegateCommand WindowCommand { get; private set; }
		public DelegateCommand WindowMaxCommand { get; private set; }

		public DelegateCommand AddSoundCommand { get; private set; }
		public DelegateCommand DelSoundCommand { get; private set; }

		public DelegateCommand SelectVideoDirCommand { get; private set; }
		public DelegateCommand SelectAnserSoundCommand { get; private set; }

		public DelegateCommand PlayVideoCommand { get; private set; }

		public DispatchObservableCollection<string> Devices
		{
			get
			{
				return this.devices;
			}
		}

		public DispatchObservableCollection<ItemVM> Items
		{
			get
			{
				return this.items;
			}
		}

		public DispatchObservableCollection<VideoVM> Videos
		{
			get
			{
				return this.videos;
			}
		}

		public ItemVM SelectedPanel
		{
			get { return selectedPanel; }
			set { SetProperty( ref selectedPanel, value, SelectedPanelChanged ); }
		}

		public string Log
		{
			get { return this.logBuilder.ToString(); }
		}

		public long UpdateTime
		{
			get { return updateTime; }
			set { SetProperty( ref updateTime, value ); }
		}

		public int WrapCount
		{
			get { return wrapCount; }
			set { SetProperty( ref wrapCount, value ); }
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

		public DispatchObservableCollection<SoundVM> Sounds
		{
			get { return this.sounds; }
		}

		public SoundVM SelectedSound
		{
			get { return selectedSound; }
			set { SetProperty( ref selectedSound, value, SelectedSoundChanged ); }
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
			this.SearchCommand = new DelegateCommand( SearchDevice, null );
			this.LoadedCommand = new DelegateCommand( Inited, null );
			this.ClosingCommand = new DelegateCommand( Closing, null );
			this.AddPanelCommand = new DelegateCommand( AddPanel, null );
			this.DelPanelCommand = new DelegateCommand( DelPanel, CanDelPanel );
			this.StartCommand = new DelegateCommand( Start, null );
			this.ResetCommand = new DelegateCommand( Reset, null );
			this.WindowCommand = new DelegateCommand( ShowCloseWindow, null );
			this.WindowMaxCommand = new DelegateCommand( MaximazeWindow, CanMaximaize );
			this.SelectVideoDirCommand = new DelegateCommand( SelectVideoDir, null );
			this.SelectAnserSoundCommand = new DelegateCommand( SelectAnser, null );
			this.AddSoundCommand = new DelegateCommand( AddSound, null );
			this.DelSoundCommand = new DelegateCommand( DelSound, CanDelSound );
			this.PlayVideoCommand = new DelegateCommand( PlayVideo );

			this.manager = new DeviceManager();
			this.manager.Devices.CollectionChanged += Devices_CollectionChanged;
			this.manager.PropertyChanged += Manager_PropertyChanged;
			this.manager.KeyPushed += Manager_KeyPushed;
			this.devices = new DispatchObservableCollection<string>();
			this.items = new DispatchObservableCollection<ItemVM>();
			this.sounds = new DispatchObservableCollection<SoundVM>();
			this.videos = new DispatchObservableCollection<VideoVM>();
			this.logBuilder = new StringBuilder();
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
				this.window.DataContext = this;
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
			foreach( var item in this.Items )
			{
				item.CanAnswer = true;
			}
		}

		private void Start( object obj )
		{
			rank = 0;
			InitRank();
		}

		private void AddPanel( object obj )
		{
			var item = new ItemVM();
			item.Data = new PanelData();
			this.Items.Add( item );
		}

		private bool CanDelPanel( object obj )
		{
			return this.SelectedPanel != null;
		}

		private void DelPanel( object obj )
		{
			this.Items.Remove( this.SelectedPanel );
			this.SelectedPanel = null;
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

		private void AddSound( object obj )
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
			dlg.Multiselect = false;
			if( dlg.ShowDialog() == true )
			{
				this.Sounds.Add( new SoundVM() { Path = dlg.FileName } );
			}
		}

		private bool CanDelSound( object obj )
		{
			return this.SelectedSound != null;
		}

		private void DelSound( object obj )
		{
			this.Sounds.Remove( this.SelectedSound );
			this.SelectedSound = null;
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

			foreach( var item in this.Sounds )
			{
				item.Dispose();
			}
			this.manager.Dispose();
		}

		private void SearchDevice( object obj )
		{
			this.manager.SearchDevice();
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
			WriteLogLine( e.Key.ToString() );
			if( this.IsSettingMode )
			{
				if( this.SelectedPanel != null )
				{
					Contract.Assert( this.SelectedPanel.Data != null );
					this.SelectedPanel.Data.DeviceGuid = e.InstanceID;
					this.SelectedPanel.Data.Key = e.Key;
				}
			}
			else
			{
				var item = this.Items.FirstOrDefault( i => i.Data.DeviceGuid == e.InstanceID && i.Data.Key == e.Key );
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

		private void Manager_PropertyChanged( object sender, PropertyChangedEventArgs e )
		{
			if( e.PropertyName == "UpdateTime" )
			{
				this.UpdateTime = this.manager.UpdateTime;
			}
		}

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

		private void SelectedSoundChanged( bool obj )
		{
			this.DelSoundCommand.RaiseCanExecuteChanged();
		}

		private void SelectedPanelChanged( bool obj )
		{
			this.DelPanelCommand.RaiseCanExecuteChanged();
		}

		#endregion

		#region 設定

		/// <summary>
		/// 設定を読み込みます。
		/// </summary>
		private void LoadData()
		{
			if( File.Exists( SettingData.FileName ) )
			{
				using( FileStream file = new FileStream( SettingData.FileName, FileMode.Open ) )
				{
					XmlSerializer xml = new XmlSerializer( typeof( SettingData ) );
					this.data = xml.Deserialize( file ) as SettingData;
				}
			}
			else
			{
				this.data = new SettingData();
			}

			foreach( var item in this.data.KeyBindCollection )
			{
				this.Items.Add( new ItemVM() { Data = item } );
			}
			foreach( var item in this.data.SoundPaths )
			{
				this.sounds.Add( new SoundVM() { Path = item } );
			}
			this.VideoDir = this.data.VideoDir;
			this.AnserSound = new SoundVM() { Path = this.data.AnserSoundPath };
			this.WrapCount = this.data.WrapCount;
		}

		/// <summary>
		/// 設定を保存します。
		/// </summary>
		public void SaveData()
		{
			this.data.KeyBindCollection.Clear();
			foreach( var item in this.Items )
			{
				this.data.KeyBindCollection.Add( item.Data );
			}
			this.data.SoundPaths.Clear();
			foreach( var item in this.sounds )
			{
				this.data.SoundPaths.Add( item.Path );
			}
			this.data.VideoDir = this.VideoDir;
			this.data.AnserSoundPath = this.AnserSound.Path;
			this.data.WrapCount = this.WrapCount;

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
			this.SelectedPanel = null;
			InitRank();
		}

		private void LoadVideos( bool isSucceed )
		{
			if( isSucceed )
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
			foreach( var i in this.Items )
			{
				i.Rank = string.Empty;
			}
		}

		private void WriteLogLine( string str )
		{
			this.logBuilder.AppendLine( str );
			this.NotifyPropertyChanged( "Log" );
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
