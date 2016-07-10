using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using EarlyPusher.Models;
using EarlyPusher.Modules.EarlyTab.Views;
using EarlyPusher.Utils;
using EarlyPusher.ViewModels;
using StFrLibs.Core.Adapters;
using StFrLibs.Core.Basis;
using StFrLibs.Core.Commands;

namespace EarlyPusher.Modules.EarlyTab.ViewModels
{
	public class OperateEarlyVM : OperateTabVMBase
	{
		private bool playingQuestion;
		private ViewModelsAdapter<TeamEarlyVM,TeamData> adapter;
		private IEnumerable<SubjectData> subjects;
		private SubjectData selectedSubject;

		private PlayEarlyView view = new PlayEarlyView();

		private MediaVM selectedMedia;
		private MediaVM pushSound = new MediaVM();
		private MediaVM correctSound = new MediaVM();
		private MediaVM missSound = new MediaVM();
		private MediaVM questionSound = new MediaVM();

		private int basePoint;
		private TeamEarlyVM answerTeam;
		private bool receivable;
		private bool answerMode = false;
		private int pointPool = 0;
		private int missCount = 0;

		#region プロパティ

		public DelegateCommand CorrectCommand { get; }
		public DelegateCommand MissCommand { get; }
		public DelegateCommand SetBasePointCommand { get; }
		public DelegateCommand PlayOrPauseCommand { get; }
		public DelegateCommand PlayAnswerCommand { get; }
		public DelegateCommand AddPointCommand { get; }

		public bool PlayingQuestion
		{
			get { return this.playingQuestion; }
			set { SetProperty( ref this.playingQuestion, value ); }
		}

		public IEnumerable<SubjectData> Subjects
		{
			get { return this.subjects; }
			set { SetProperty( ref this.subjects, value ); }
		}

		public SubjectData SelectedSubject
		{
			get { return this.selectedSubject; }
			set { SetProperty( ref this.selectedSubject, value, SubjectChanged ); }
		}

		/// <summary>
		/// メディアのリスト
		/// </summary>
		public ObservableHashVMCollection<MediaVM> Medias { get; }

		/// <summary>
		/// チームのリスト
		/// </summary>
		/// <remarks>
		/// プレイウィンドウに表示するチームごとの列のリスト
		/// </remarks>
		public ObservableVMCollection<TeamData, TeamEarlyVM> Teams { get; }

		/// <summary>
		/// 選択しているメディア
		/// </summary>
		public MediaVM SelectedMedia
		{
			get { return this.selectedMedia; }
			set { SetProperty( ref this.selectedMedia, value, SelectedMediaChanged, SelectedMediaChanging ); }
		}

		public int BasePoint
		{
			get { return this.basePoint; }
			set { SetProperty( ref this.basePoint, value ); }
		}

		/// <summary>
		/// プッシュ受付可能
		/// </summary>
		public bool Receivable
		{
			get { return this.receivable; }
			set { SetProperty( ref this.receivable, value ); }
		}

		/// <summary>
		/// 解答権を持つチーム
		/// </summary>
		public TeamEarlyVM AnswerTeam
		{
			get { return this.answerTeam; }
			set { SetProperty( ref this.answerTeam, value, AnswerTeamChanged, AnswerTeamChanging ); }
		}

		/// <summary>
		/// 追加するポイント
		/// </summary>
		public int PointPool
		{
			get { return this.pointPool; }
			set { SetProperty( ref this.pointPool, value ); }
		}

		public int MissCount
		{
			get { return this.missCount; }
			set { SetProperty( ref this.missCount, value ); }
		}

		#endregion

		public OperateEarlyVM( MainVM parent )
			: base( parent )
		{
			this.Teams = new ObservableVMCollection<TeamData, TeamEarlyVM>();
			this.Medias = new ObservableHashVMCollection<MediaVM>();
			this.Medias.CollectionChanged += Medias_CollectionChanged;

			this.View = new OperateEarlyView();
			this.Header = "早押し";
			this.PlayOrPauseCommand = new DelegateCommand( PlayOrPause, p => this.SelectedMedia != null );
			this.PlayAnswerCommand = new DelegateCommand( PlayAnswer, p => this.Medias.Any( m => Path.GetFileNameWithoutExtension( m.FilePath ).EndsWith( "_ans" ) ) );
			this.CorrectCommand = new DelegateCommand( Correct, o => this.AnswerTeam != null );
			this.MissCommand = new DelegateCommand( Miss, o => this.AnswerTeam != null );
			this.SetBasePointCommand = new DelegateCommand( SetBasePoint );
			this.AddPointCommand = new DelegateCommand( AddPoint );

			this.questionSound.MediaStoped += QuestionSound_MediaStoped;
		}

		public override UIElement PlayView
		{
			get { return this.view; }
		}

		#region 設定読み書き

		public override void LoadData()
		{
			base.LoadData();

			this.adapter = new ViewModelsAdapter<TeamEarlyVM, TeamData>( m => new TeamEarlyVM( m ) );
			this.adapter.Adapt( this.Teams, this.Parent.Data.TeamList );

			this.Subjects = this.Parent.Data.Early.Subjects;

			var baseDir = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );
			{
				this.pushSound.FilePath = PathUtility.GetAbsolutePath( baseDir, this.Parent.Data.Early.PushPath );
				this.pushSound.LoadFile();
			}
			{
				this.correctSound.FilePath = PathUtility.GetAbsolutePath( baseDir, this.Parent.Data.Early.CorrectPath );
				this.correctSound.LoadFile();
			}
			{
				this.missSound.FilePath = PathUtility.GetAbsolutePath( baseDir, this.Parent.Data.Early.IncorrectPath );
				this.missSound.LoadFile();
			}
			{
				this.questionSound.FilePath = PathUtility.GetAbsolutePath( baseDir, this.Parent.Data.Early.QuestionPath );
				this.questionSound.LoadFile();
			}
			this.Parent.Data.Early.PropertyChanged += Early_PropertyChanged;
		}

		private void Early_PropertyChanged( object sender, System.ComponentModel.PropertyChangedEventArgs e )
		{
			var baseDir = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );

			if( e.PropertyName == nameof( this.Parent.Data.Early.PushPath ) )
			{
				this.pushSound.FilePath = PathUtility.GetAbsolutePath( baseDir, this.Parent.Data.Early.PushPath );
				this.pushSound.LoadFile();
			}

			if( e.PropertyName == nameof( this.Parent.Data.Early.CorrectPath ) )
			{
				this.correctSound.FilePath = PathUtility.GetAbsolutePath( baseDir, this.Parent.Data.Early.CorrectPath );
				this.correctSound.LoadFile();
			}

			if( e.PropertyName == nameof( this.Parent.Data.Early.IncorrectPath ) )
			{
				this.missSound.FilePath = PathUtility.GetAbsolutePath( baseDir, this.Parent.Data.Early.IncorrectPath );
				this.missSound.LoadFile();
			}

			if( e.PropertyName == nameof( this.Parent.Data.Early.QuestionPath ) )
			{
				this.questionSound.FilePath = PathUtility.GetAbsolutePath( baseDir, this.Parent.Data.Early.QuestionPath );
				this.questionSound.LoadFile();
			}
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

		private void Manager_KeyPushed( object sender, Manager.DeviceKeyEventArgs e )
		{
			if( this.Receivable )
			{
				var team = this.Teams.FirstOrDefault( t => t.Model.Members.Any( m => m.DeviceGuid == e.InstanceID && m.Key == e.Key ) );
				if( team?.Pushable ?? false )
				{
					this.pushSound.Play();
					this.AnswerTeam = team;
					this.SelectedMedia.Pause();

					this.Receivable = false;
				}
			}
		}

		private void SubjectChanged()
		{
			if( this.SelectedSubject != null )
			{
				this.Medias.Clear();
				foreach( string path in Directory.EnumerateFiles( this.SelectedSubject.Path, "*", SearchOption.AllDirectories ) )
				{
					var media = new MediaVM() { FilePath = path };
					media.LoadFile();
					this.Medias.Add( media );
				}
				this.PlayAnswerCommand.RaiseCanExecuteChanged();
			}
		}

		private void Medias_CollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
		{
			if( e.NewItems != null )
			{
				foreach( MediaVM media in e.NewItems )
				{
					media.MediaPlayed += Media_MediaPlayed;
					media.MediaPaused += Media_MediaStoped;
					media.MediaStoped += Media_MediaStoped;
				}
			}
			if( e.OldItems != null )
			{
				foreach( MediaVM media in e.OldItems )
				{
					media.MediaPlayed -= Media_MediaPlayed;
					media.MediaPaused -= Media_MediaStoped;
					media.MediaStoped -= Media_MediaStoped;
				}
			}
		}

		private void QuestionSound_MediaStoped( object sender, EventArgs e )
		{
			this.SelectedMedia.Play();
			this.PlayingQuestion = false;
		}

		private void Media_MediaStoped( object sender, EventArgs e )
		{
			this.Receivable = false;
			this.answerMode = false;
			Debug.WriteLine( $"MediaStoped : {this.answerMode}" );
		}

		private void Media_MediaPlayed( object sender, EventArgs e )
		{
			Debug.WriteLine( $"MediaStart : {this.answerMode}" );
			if( !this.answerMode )
			{
				this.Receivable = true;
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
			}
		}

		private void SelectedMediaChanged()
		{
			this.PlayOrPauseCommand.RaiseCanExecuteChanged();
		}

		private void AnswerTeamChanging()
		{
			if( this.AnswerTeam != null )
			{
				this.AnswerTeam.Answerable = false;
			}
		}

		private void AnswerTeamChanged()
		{
			if( this.AnswerTeam != null )
			{
				this.AnswerTeam.Answerable = true;
			}
			this.CorrectCommand.RaiseCanExecuteChanged();
			this.MissCommand.RaiseCanExecuteChanged();
		}

		#endregion

		#region コマンド

		private void PlayOrPause( object obj )
		{
			if( !this.SelectedMedia.IsPlaying )
			{
				this.questionSound.Play();
				this.PlayingQuestion = true;
			}
			else
			{
				this.SelectedMedia.Pause();
			}
		}

		private void PlayAnswer( object obj )
		{
			this.MissCount = 0;
			this.SelectedMedia = this.Medias.FirstOrDefault( m => Path.GetFileNameWithoutExtension( m.FilePath ).EndsWith( "_ans" ) );
			if( this.SelectedMedia != null )
			{
				this.answerMode = true;
				Debug.WriteLine( $"PlayAnswer : {this.answerMode}" );
				this.SelectedMedia.Play();
			}
		}

		private void Correct( object obj )
		{
			this.correctSound.Play();
		}

		private void Miss( object obj )
		{
			this.missSound.Play();
			this.AnswerTeam = null;
			this.MissCount++;

			this.PointPool += this.BasePoint;
		}

		private void SetBasePoint( object obj )
		{
			this.PointPool += this.BasePoint;
		}

		private void AddPoint( object obj )
		{
			if( this.PointPool == 0 )
			{
				if( MessageBox.Show( App.Current.MainWindow, "追加ポイント0だけどいいの？", string.Empty, MessageBoxButton.OKCancel ) != MessageBoxResult.OK )
				{
					return;
				}
			}

			this.AnswerTeam.Add( this.PointPool );
			this.PointPool = 0;

			this.AnswerTeam = null;
		}

		#endregion
	}
}
