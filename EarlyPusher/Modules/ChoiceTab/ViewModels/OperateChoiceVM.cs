using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Windows;
using EarlyPusher.Manager;
using EarlyPusher.Models;
using EarlyPusher.Modules.ChoiceTab.Views;
using EarlyPusher.Modules.EarlyTab.Views;
using EarlyPusher.ViewModels;
using StFrLibs.Core.Adapters;
using StFrLibs.Core.Basis;
using StFrLibs.Core.Commands;

namespace EarlyPusher.Modules.ChoiceTab.ViewModels
{
	public class OperateChoiceVM : OperateTabVMBase
	{
		private PlayChoiceView view = new PlayChoiceView();

		private ObservableHashVMCollection<TeamChoiceVM> teams = new ObservableHashVMCollection<TeamChoiceVM>();
		private ObservableHashVMCollection<MediaVM> medias = new ObservableHashVMCollection<MediaVM>();
		private ViewModelsAdapter<TeamChoiceVM,TeamData> teamAdapter;
		private MediaVM standSound;
		private MediaVM questionSound;
		private MediaVM selectedMedia;
		private MediaVM checkSound;
		private bool isChoiceVisible;

		#region プロパティ

		public DelegateCommand OpenCommand { get; private set; }
		public DelegateCommand ResetCommand { get; private set; }

		/// <summary>
		/// チームのリスト
		/// </summary>
		public ObservableHashCollection<TeamChoiceVM> Teams
		{
			get { return this.teams; }
		}

		public IEnumerable SelectableItems
		{
			get
			{
				yield return new { Top = 80.0, Right = 760.0, SelectableItems = this.Teams.Select( t => t.KeyList[0] ) };
				yield return new { Top = 80.0, Right = 150.0, SelectableItems = this.Teams.Select( t => t.KeyList[1] ) };
				yield return new { Top = 520.0, Right = 760.0, SelectableItems = this.Teams.Select( t => t.KeyList[2] ) };
				yield return new { Top = 520.0, Right = 150.0, SelectableItems = this.Teams.Select( t => t.KeyList[3] ) };
			}
		}

		public override UIElement PlayView
		{
			get { return this.view; }
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

		/// <summary>
		/// 選択しているメディア
		/// </summary>
		public MediaVM SelectedMedia
		{
			get { return this.selectedMedia; }
			set { SetProperty( ref this.selectedMedia, value, SelectedMediaChanged, SelectedMediaChanging ); }
		}

		/// <summary>
		/// メディアのリスト
		/// </summary>
		public ObservableHashVMCollection<MediaVM> Medias
		{
			get
			{
				return this.medias;
			}
		}

		/// <summary>
		/// 選択を表示する。
		/// </summary>
		public bool IsChoiceVisible
		{
			get { return this.isChoiceVisible; }
			set { SetProperty( ref this.isChoiceVisible, value ); }
		}

		public override UIElement View
		{
			get;
		}

		public override string Header
		{
			get;
		}

		#endregion

		public OperateChoiceVM( MainVM parent )
			: base( parent )
		{
			this.View = new OperateChoiceView();
			this.Header = "4択";
			this.OpenCommand = new DelegateCommand( Open );
			this.ResetCommand = new DelegateCommand( Reset );
		}

		#region 設定読み書き

		public override void LoadData()
		{
			base.LoadData();

			this.Parent.Data.PropertyChanged += Data_PropertyChanged;

			this.teamAdapter = new ViewModelsAdapter<TeamChoiceVM, TeamData>( CreateTeamVM );
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

			if( !string.IsNullOrEmpty( this.Parent.Data.CheckSoundPath ) )
			{
				this.checkSound = new MediaVM();
				this.checkSound.FilePath = this.Parent.Data.CheckSoundPath;
				this.checkSound.LoadFile();
			}

			LoadVideos();
		}

		/// <summary>
		/// チームのモデルからVMを作る時の処理
		/// </summary>
		/// <param name="data">モデル</param>
		/// <returns>VM</returns>
		private TeamChoiceVM CreateTeamVM( TeamData data )
		{
			var vm = new TeamChoiceVM( data );
			return vm;
		}

		#endregion

		#region コマンド関係

		private void Open( object obj )
		{
			this.IsChoiceVisible = true;

			if( this.checkSound != null )
			{
				this.checkSound.Play();
			}
		}

		private void Reset( object obj )
		{
			foreach( var team in this.Teams )
			{
				team.ResetSeletectedItem();
			}
			this.IsChoiceVisible = false;
		}

		#endregion

		#region イベント

		private void Data_PropertyChanged( object sender, System.ComponentModel.PropertyChangedEventArgs e )
		{
			if( e.PropertyName == "ChoiceVideoDir" )
			{
				LoadVideos();
			}
		}

		private void Manager_KeyPushed( object sender, DeviceKeyEventArgs e )
		{
			foreach( var team in this.Teams )
			{
				if( team.ExistSelectedItem( e.InstanceID, e.Key ) )
				{
					return;
				}
			}
		}

		private void SelectedMediaChanged()
		{
			if( this.SelectedMedia != null )
			{
				this.SelectedMedia.MediaPlayed += SelectedMedia_MediaPlayed;
				this.SelectedMedia.MediaStoped += SelectedMedia_MediaStoped;
			}
		}

		/// <summary>
		/// 選択しているメディアが変わるとき、以前のメディアを停止します。
		/// </summary>
		private void SelectedMediaChanging()
		{
			if( this.SelectedMedia != null )
			{
				this.SelectedMedia.Stop();
				this.SelectedMedia.MediaPlayed -= SelectedMedia_MediaPlayed;
				this.SelectedMedia.MediaStoped -= SelectedMedia_MediaStoped;
			}
		}

		private void SelectedMedia_MediaPlayed( object sender, EventArgs e )
		{
			this.Parent.Manager.KeyPushed += Manager_KeyPushed;
		}

		private void SelectedMedia_MediaStoped( object sender, EventArgs e )
		{
			this.Parent.Manager.KeyPushed -= Manager_KeyPushed;
		}

		#endregion

		/// <summary>
		/// メディアフォルダが変更されたとき、メディア一覧を更新します。
		/// </summary>
		private void LoadVideos()
		{
			this.Medias.Clear();
			if( !string.IsNullOrEmpty( this.Parent.Data.ChoiceVideoDir ) && Directory.Exists( this.Parent.Data.ChoiceVideoDir ) )
			{
				foreach( string path in Directory.EnumerateDirectories( this.Parent.Data.ChoiceVideoDir, "*", SearchOption.TopDirectoryOnly ) )
				{
					var media = new MediaChoiceVM( path ) { FileName = Path.GetFileName( path ) };
					this.Medias.Add( media );
				}
			}
		}
	}
}
