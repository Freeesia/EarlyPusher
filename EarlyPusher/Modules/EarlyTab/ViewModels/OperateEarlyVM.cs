using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
		private ViewModelsAdapter<TeamEarlyVM,TeamData> adapter;
		private IEnumerable<SetData> sets;
		private SetData selectedSet;

		private PlayEarlyView view = new PlayEarlyView();

		private MediaVM selectedMedia;
		private MediaVM pushSound = new MediaVM();
		private MediaVM correctSound = new MediaVM();
		private MediaVM missSound = new MediaVM();

		private TeamEarlyVM answerTeam;
		private bool receivable;
		private int addPoint;

		#region プロパティ

		public DelegateCommand CorrectCommand { get; }
		public DelegateCommand MissCommand { get; }

		public IEnumerable<SetData> Sets
		{
			get { return this.sets; }
			set { SetProperty( ref this.sets, value ); }
		}

		public SetData SelectedSet
		{
			get { return this.selectedSet; }
			set { SetProperty( ref this.selectedSet, value, SetChanged ); }
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
			set { SetProperty( ref this.selectedMedia, value, null, SelectedMediaChanging ); }
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
		public int AddPoint
		{
			get { return this.addPoint; }
			set { SetProperty( ref this.addPoint, value ); }
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
			this.CorrectCommand = new DelegateCommand( Correct );
			this.MissCommand = new DelegateCommand( Miss );
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

			this.Sets = this.Parent.Data.Early.Sets;

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

		private void SetChanged()
		{
			if( this.SelectedSet != null )
			{
				this.Medias.Clear();
				foreach( string path in Directory.EnumerateFiles( this.SelectedSet.Path, "*", SearchOption.AllDirectories ) )
				{
					var media = new MediaVM() { FilePath = path };
					media.LoadFile();
					this.Medias.Add( media );
				}
			}
		}

		private void Medias_CollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
		{
			if( e.NewItems != null )
			{
				foreach( MediaVM media in e.NewItems )
				{
					media.MediaPlayed += Media_MediaPlayed;
					media.MediaStoped += Media_MediaStoped;
				}
			}
			if( e.OldItems != null )
			{
				foreach( MediaVM media in e.OldItems )
				{
					media.MediaPlayed -= Media_MediaPlayed;
					media.MediaStoped -= Media_MediaStoped;
				}
			}
		}

		private void Media_MediaStoped( object sender, EventArgs e )
		{
			this.Receivable = false;
		}

		private void Media_MediaPlayed( object sender, EventArgs e )
		{
			this.Receivable = true;
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

		private void Correct( object obj )
		{
			if( this.AddPoint == 0 )
			{
				if( MessageBox.Show( App.Current.MainWindow, "追加ポイント0だけどいいの？", string.Empty, MessageBoxButton.OKCancel ) != MessageBoxResult.OK )
				{
					return;
				}
			}

			this.correctSound.Play();
			this.AnswerTeam.Add( this.AddPoint );
			this.AddPoint = 0;

			this.AnswerTeam = null;
		}

		private void Miss( object obj )
		{
			this.missSound.Play();
			this.AnswerTeam = null;
		}

		#endregion
	}
}
