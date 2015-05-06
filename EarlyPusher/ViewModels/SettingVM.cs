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
		private ObservableHashCollection<MediaVM> medias = new ObservableHashCollection<MediaVM>();
		private bool isSettingMode = true;
		private int rank = 0;
		private string videoDir;

		private MediaVM answerSound;
		private MemberVM selectedMember;
		private MediaVM selectedMedia;
		private PlayWindow window;

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
		public DelegateCommand SelectAnswerSoundCommand { get; private set; }

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

		public ObservableHashCollection<MediaVM> Medias
		{
			get
			{
				return this.medias;
			}
		}
		
		public MemberVM SelectedMember
		{
			get { return selectedMember; }
			set { SetProperty( ref selectedMember, value, SelectedMemberChanged ); }
		}

		public MediaVM SelectedMedia
		{
			get { return this.selectedMedia; }
			set { SetProperty( ref this.selectedMedia, value, null, SelectedMediaChanging ); }
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

		public MediaVM AnswerSound
		{
			get { return answerSound; }
			set { SetProperty( ref answerSound, value ); }
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
			this.SelectAnswerSoundCommand = new DelegateCommand( SelectAnwser, null );

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

		private void SelectedMemberChanged()
		{
			this.DelMemberCommand.RaiseCanExecuteChanged();
		}


		private void SelectedMediaChanging()
		{
			if( this.SelectedMedia != null )
			{
				this.SelectedMedia.Stop();
			}
		}

		private void SettingChanged()
		{
			rank = 0;
			this.SelectedMember = null;
			InitRank();
		}

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

		#endregion

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
