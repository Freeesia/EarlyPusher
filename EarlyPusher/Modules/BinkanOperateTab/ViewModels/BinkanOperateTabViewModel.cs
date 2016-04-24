using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using EarlyPusher.Manager;
using EarlyPusher.Models;
using EarlyPusher.Modules.BinkanOperateTab.Views;
using EarlyPusher.Utils;
using EarlyPusher.ViewModels;
using StFrLibs.Core.Adapters;
using StFrLibs.Core.Commands;

namespace EarlyPusher.Modules.BinkanOperateTab.ViewModels
{
	public class BinkanOperateTabViewModel : OperateTabVMBase
	{
		private ViewModelsAdapter<MediaVM,string> mediaAdapter;
		private ViewModelsAdapter<TeamViewModel,TeamData> teamAdapter;
		private MediaVM selectedMedia;
		private bool receivable;
		private TeamViewModel answerTeam;
		private int addPoint;

		private MediaVM pushSound= new MediaVM();
		private MediaVM correctSound= new MediaVM();
		private MediaVM incorrectSound= new MediaVM();

		#region プロパティ

		/// <summary>
		/// ヒント動画一覧
		/// </summary>
		public ObservableCollection<MediaVM> Medias { get; }

		public MediaVM SelectedMedia
		{
			get { return this.selectedMedia; }
			set { SetProperty( ref this.selectedMedia, value ); }
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
		/// チーム一覧
		/// </summary>
		public ObservableCollection<TeamViewModel> Teams { get; }

		/// <summary>
		/// 解答権を持つチーム
		/// </summary>
		public TeamViewModel AnswerTeam
		{
			get { return this.answerTeam; }
			set { SetProperty( ref this.answerTeam, value, AnswerTeamChanged ); }
		}

		/// <summary>
		/// 追加するポイント
		/// </summary>
		public int AddPoint
		{
			get { return this.addPoint; }
			set { SetProperty( ref this.addPoint, value ); }
		}

		public DelegateCommand CorrectCommand { get; }
		public DelegateCommand IncorrectCommand { get; }

		#endregion

		public BinkanOperateTabViewModel( MainVM parent ) : base( parent )
		{
			this.Header = "ビンカン";
			this.View = new BinkanOperateTabView();

			this.mediaAdapter = new ViewModelsAdapter<MediaVM, string>( CreateMediaViewModel, DeleteMediaViewModel );
			this.teamAdapter = new ViewModelsAdapter<TeamViewModel, TeamData>( CrateTeamViewModel );

			this.Medias = new ObservableCollection<MediaVM>();
			this.Teams = new ObservableCollection<TeamViewModel>();

			this.CorrectCommand = new DelegateCommand( Correct, p => this.AnswerTeam != null );
			this.IncorrectCommand = new DelegateCommand( Incorrect, p => this.AnswerTeam != null );
		}

		#region コマンド

		private void AnswerTeamChanged()
		{
			this.CorrectCommand.RaiseCanExecuteChanged();
			this.IncorrectCommand.RaiseCanExecuteChanged();
		}

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

		private void Incorrect( object obj )
		{
			this.incorrectSound.Play();
			this.AnswerTeam = null;
		}

		#endregion

		#region 読み込み

		public override void LoadData()
		{
			this.Medias.Clear();
			this.SelectedMedia = null;

			this.mediaAdapter.Adapt( this.Medias, this.Parent.Data.Binkan.Hints );

			if( this.Medias.Count > 0 )
			{
				this.SelectedMedia = this.Medias[0];
			}

			this.Teams.Clear();
			this.teamAdapter.Adapt( this.Teams, this.Parent.Data.TeamList );

			var baseDir = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );
			{
				this.pushSound.FilePath = PathUtility.GetAbsolutePath( baseDir, this.Parent.Data.Binkan.PushPath );
				this.pushSound.LoadFile();
			}
			{
				this.correctSound.FilePath = PathUtility.GetAbsolutePath( baseDir, this.Parent.Data.Binkan.CorrectPath );
				this.correctSound.LoadFile();
			}
			{
				this.incorrectSound.FilePath = PathUtility.GetAbsolutePath( baseDir, this.Parent.Data.Binkan.IncorrectPath );
				this.incorrectSound.LoadFile();
			}
			this.Parent.Data.Binkan.PropertyChanged += Binkan_PropertyChanged;
		}

		private void Binkan_PropertyChanged( object sender, System.ComponentModel.PropertyChangedEventArgs e )
		{
			var baseDir = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );

			if( e.PropertyName == nameof( this.Parent.Data.Binkan.PushPath ) )
			{
				this.pushSound.FilePath = PathUtility.GetAbsolutePath( baseDir, this.Parent.Data.Binkan.PushPath );
				this.pushSound.LoadFile();
			}

			if( e.PropertyName == nameof( this.Parent.Data.Binkan.CorrectPath ) )
			{
				this.correctSound.FilePath = PathUtility.GetAbsolutePath( baseDir, this.Parent.Data.Binkan.CorrectPath );
				this.correctSound.LoadFile();
			}

			if( e.PropertyName == nameof( this.Parent.Data.Binkan.IncorrectPath ) )
			{
				this.incorrectSound.FilePath = PathUtility.GetAbsolutePath( baseDir, this.Parent.Data.Binkan.IncorrectPath );
				this.incorrectSound.LoadFile();
			}
		}

		#endregion

		#region ViewMode関連

		private TeamViewModel CrateTeamViewModel( TeamData arg )
		{
			return new TeamViewModel( arg );
		}

		private MediaVM CreateMediaViewModel( string arg )
		{
			var baseDir = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );
			var media = new MediaVM() { FilePath = PathUtility.GetAbsolutePath( baseDir, arg ) };
			media.LoadFile();
			media.Media.MediaEnded += Media_MediaEnded;
			media.MediaPlayed += Media_MediaPlayed;

			return media;
		}

		private void DeleteMediaViewModel( MediaVM media )
		{
			media.Media.MediaEnded -= Media_MediaEnded;
			media.MediaPlayed -= Media_MediaPlayed;
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

		private void Media_MediaPlayed( object sender, EventArgs e )
		{
			this.Receivable = true;
		}

		private void Media_MediaEnded( object sender, System.Windows.RoutedEventArgs e )
		{
			var index = this.Medias.IndexOf( this.SelectedMedia );

			if( index < this.Medias.Count - 1 )
			{
				this.SelectedMedia = this.Medias[index + 1];
				this.SelectedMedia.Play();
			}
		}

		private void Manager_KeyPushed( object sender, DeviceKeyEventArgs e )
		{
			if( this.Receivable )
			{
				var team = this.Teams.FirstOrDefault( t => t.Model.Members.Any( m => m.DeviceGuid == e.InstanceID && m.Key == e.Key ) );
				if( team?.Pushable ?? false )
				{
					this.pushSound.Play();
					this.AnswerTeam = team;
					this.SelectedMedia.Pause();
				}
			}
		}
	}
}
