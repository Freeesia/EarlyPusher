using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using EarlyPusher.Manager;
using EarlyPusher.Models;
using EarlyPusher.Modules.SortTab.Views;
using EarlyPusher.ViewModels;
using StFrLibs.Core.Adapters;
using StFrLibs.Core.Basis;
using StFrLibs.Core.Commands;
using StFrLibs.Core.Extensions;

namespace EarlyPusher.Modules.SortTab.ViewModels
{
	public class OperateSortVM : OperateTabVMBase
	{
		private ObservableHashVMCollection<SortMediaVM> medias = new ObservableHashVMCollection<SortMediaVM>();
		private ObservableVMCollection<TeamData,TeamSortVM> teams = new ObservableVMCollection<TeamData, TeamSortVM>();
		private ViewModelsAdapter<TeamSortVM,TeamData> adapter;

		private PlayOtherSortView playOtherView;
		private PlayWinnerSortView playWinnerView;

		private SortMediaVM selectedMedia;
		private UIElement playView;

		private bool isVisiblePlayView;

		#region プロパティ

		public DelegateCommand OpenWinnerCommand { get; private set; }
		public DelegateCommand OpenOtherCommand { get; private set; }
		public DelegateCommand OpenAnswer1Command { get; private set; }
		public DelegateCommand OpenAnswer2Command { get; private set; }
		public DelegateCommand OpenAnswerAllCommand { get; private set; }
		public DelegateCommand ResetCommand { get; private set; }

		/// <summary>
		/// メディアのリスト
		/// </summary>
		public ObservableHashVMCollection<SortMediaVM> Medias
		{
			get
			{
				return this.medias;
			}
		}

		public ObservableVMCollection<TeamData, TeamSortVM> Teams
		{
			get { return this.teams; }
		}

		public TeamSortVM WinnerTeam
		{
			get { return this.Teams.FirstOrDefault( t => t.IsWinner ); }
		}

		public IEnumerable<TeamSortVM> OtherTeams
		{
			get { return this.Teams.Where( t => !t.IsWinner ); }
		}

		/// <summary>
		/// 選択しているメディア
		/// </summary>
		public SortMediaVM SelectedMedia
		{
			get { return this.selectedMedia; }
			set { SetProperty( ref this.selectedMedia, value, null, SelectedMediaChanging ); }
		}

		public bool IsVisiblePlayView
		{
			get { return this.isVisiblePlayView; }
			set { SetProperty( ref this.isVisiblePlayView, value ); }
		}
		
		public override UIElement PlayView
		{
			get { return this.playView; }
			set { SetProperty( ref this.playView, value ); }
		}

		#endregion

		public OperateSortVM( MainVM parent )
			: base( parent )
		{
			this.OpenWinnerCommand = new DelegateCommand( OpenWinner );
			this.OpenOtherCommand = new DelegateCommand( OpenOther );
			this.OpenAnswer1Command = new DelegateCommand( OpenAnswer1 );
			this.OpenAnswer2Command = new DelegateCommand( OpenAnswer2 );
			this.OpenAnswerAllCommand = new DelegateCommand( OpenAnswerAll );
			this.ResetCommand = new DelegateCommand( Reset );

			this.adapter = new ViewModelsAdapter<TeamSortVM, TeamData>( CreateTeamSortVM );

			this.playOtherView = new PlayOtherSortView() { DataContext = this };
			this.playWinnerView = new PlayWinnerSortView() { DataContext = this };

			this.PlayView = this.playWinnerView;
		}

		#region 構築

		private TeamSortVM CreateTeamSortVM( TeamData data )
		{
			return new TeamSortVM( data );
		}

		public override void LoadData()
		{
			base.LoadData();

			this.Parent.Data.PropertyChanged += Data_PropertyChanged;

			this.adapter.Adapt( this.Teams, this.Parent.Data.TeamList );

			LoadVideos();
		}

		/// <summary>
		/// メディアフォルダが変更されたとき、メディア一覧を更新します。
		/// </summary>
		private void LoadVideos()
		{
			if( !string.IsNullOrEmpty( this.Parent.Data.SortVideoDir ) && Directory.Exists( this.Parent.Data.SortVideoDir ) )
			{
				this.Medias.Clear();
				foreach( string path in Directory.EnumerateFiles( this.Parent.Data.SortVideoDir, "*", SearchOption.AllDirectories ) )
				{
					var media = new SortMediaVM() { FilePath = path, FileName = Path.GetFileName( path ) };
					media.LoadFile();
					this.Medias.Add( media );
				}
			}
		}

		public override void Activate()
		{
			this.Parent.Manager.KeyPushed += Manager_KeyPushed;
		}

		public override void Deactivate()
		{
			this.Parent.Manager.KeyPushed -= Manager_KeyPushed;
		}

		#endregion

		#region コマンド関係

		private void OpenOther( object obj )
		{
			this.IsVisiblePlayView = true;
			NotifyPropertyChanged( () => this.OtherTeams );
			this.PlayView = this.playOtherView;
			this.OtherTeams.SelectMany( t => t.SortedList ).ForEach( i => i.IsVisible = true );
			this.OtherTeams.ForEach( t => t.CheckCorrect( this.SelectedMedia ) );
		}

		private void OpenWinner( object obj )
		{
			this.IsVisiblePlayView = true;
			NotifyPropertyChanged( () => this.WinnerTeam );
			this.PlayView = this.playWinnerView;
			this.WinnerTeam.SortedList.ForEach( i => i.IsVisible = true );
		}

		private void OpenAnswer1( object obj )
		{
			this.IsVisiblePlayView = true;
			this.PlayView = this.playWinnerView;
			this.SelectedMedia.SortedList[0].IsVisible = true;
		}

		private void OpenAnswer2( object obj )
		{
			this.IsVisiblePlayView = true;
			this.PlayView = this.playWinnerView;
			this.SelectedMedia.SortedList[1].IsVisible = true;
		}

		private void OpenAnswerAll( object obj )
		{
			this.IsVisiblePlayView = true;
			this.PlayView = this.playWinnerView;
			this.SelectedMedia.SortedList.ForEach( i => i.IsVisible = true );
			this.WinnerTeam.CheckCorrect( this.SelectedMedia );
		}

		private void Reset( object obj )
		{
			this.IsVisiblePlayView = false;
			this.SelectedMedia.Clear();
			this.Teams.ForEach( t => t.Clear() );
		}

		#endregion

		#region イベント

		private void Manager_KeyPushed( object sender, DeviceKeyEventArgs e )
		{
			foreach( var team in this.Teams )
			{
				if( team.SetKey( e.InstanceID, e.Key ) )
				{
					return;
				}
			}
		}

		private void Data_PropertyChanged( object sender, PropertyChangedEventArgs e )
		{
			switch( e.PropertyName )
			{
				case "SortVideoDir":
					LoadVideos();
					break;
				default:
					break;
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

		#endregion
	}
}
