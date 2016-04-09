using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using EarlyPusher.Manager;
using EarlyPusher.Models;
using EarlyPusher.Modules.OrderTab.Views;
using EarlyPusher.ViewModels;
using StFrLibs.Core.Adapters;
using StFrLibs.Core.Basis;
using StFrLibs.Core.Commands;
using StFrLibs.Core.Extensions;

namespace EarlyPusher.Modules.OrderTab.ViewModels
{
	public class OperateOrderVM : OperateTabVMBase
	{
		private ObservableHashVMCollection<ChoiceOrderMediaVM> medias = new ObservableHashVMCollection<ChoiceOrderMediaVM>();
		private ObservableVMCollection<TeamData,TeamOrderVM> teams = new ObservableVMCollection<TeamData, TeamOrderVM>();
		private ViewModelsAdapter<TeamOrderVM,TeamData> adapter;

		private PlayOtherOrderView playOtherView;
		private PlayWinnerOrderView playWinnerView;

		private ChoiceOrderMediaVM selectedMedia;
		private UIElement playView;

		private bool isVisiblePlayView;
		private bool winnerResult;

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
		public ObservableHashVMCollection<ChoiceOrderMediaVM> Medias
		{
			get
			{
				return this.medias;
			}
		}

		public ObservableVMCollection<TeamData, TeamOrderVM> Teams
		{
			get { return this.teams; }
		}

		public TeamOrderVM WinnerTeam
		{
			get { return this.Teams.FirstOrDefault( t => t.IsWinner ); }
		}

		public IEnumerable<TeamOrderVM> OtherTeams
		{
			get { return this.Teams.Where( t => !t.IsWinner ); }
		}

		public bool IsCorrectInOtherTeams
		{
			get { return this.OtherTeams.Any( t => t.IsCorrect ); }
		}

		public bool WinnerResult
		{
			get { return this.winnerResult; }
			set { SetProperty( ref this.winnerResult, value ); }
		}

		/// <summary>
		/// 選択しているメディア
		/// </summary>
		public ChoiceOrderMediaVM SelectedMedia
		{
			get { return this.selectedMedia; }
			set { SetProperty( ref this.selectedMedia, value, CommandRaiseCanExecuteChanged, SelectedMediaChanging ); }
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

		public OperateOrderVM( MainVM parent )
			: base( parent )
		{
			this.View = new OperateOrderView();
			this.Header = "並び替え";

			this.OpenWinnerCommand = new DelegateCommand( OpenWinner, CanSelectedMedia );
			this.OpenOtherCommand = new DelegateCommand( OpenOther, CanSelectedMedia );
			this.OpenAnswer1Command = new DelegateCommand( OpenAnswer1, CanSelectedMedia );
			this.OpenAnswer2Command = new DelegateCommand( OpenAnswer2, CanSelectedMedia );
			this.OpenAnswerAllCommand = new DelegateCommand( OpenAnswerAll, CanSelectedMedia );
			this.ResetCommand = new DelegateCommand( Reset, CanSelectedMedia );

			this.adapter = new ViewModelsAdapter<TeamOrderVM, TeamData>( CreateTeamSortVM );

			this.playOtherView = new PlayOtherOrderView() { DataContext = this };
			this.playWinnerView = new PlayWinnerOrderView() { DataContext = this };

			this.PlayView = this.playWinnerView;
		}

		#region 構築

		private TeamOrderVM CreateTeamSortVM( TeamData data )
		{
			return new TeamOrderVM( this, data );
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
				foreach( var item in this.Parent.Data.ChoiceOrderMediaList.Where( i => File.Exists( i.MediaPath ) ) )
				{
					var media = new ChoiceOrderMediaVM( item );
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

		private bool CanSelectedMedia( object obj )
		{
			return this.SelectedMedia != null && this.WinnerTeam != null;
		}

		private void OpenOther( object obj )
		{
			this.IsVisiblePlayView = true;
			this.PlayView = this.playOtherView;

			this.OtherTeams.ForEach( t => t.CheckCorrect( this.SelectedMedia, 4 ) );
			NotifyPropertyChanged( () => this.OtherTeams );
			NotifyPropertyChanged( () => this.IsCorrectInOtherTeams );
		}

		private void OpenWinner( object obj )
		{
			this.IsVisiblePlayView = true;
			NotifyPropertyChanged( () => this.WinnerTeam );
			this.PlayView = this.playWinnerView;
		}

		private void OpenAnswer1( object obj )
		{
			this.IsVisiblePlayView = true;
			this.PlayView = this.playWinnerView;
			this.SelectedMedia.SortedList[0].IsVisible = true;
			this.WinnerTeam.CheckCorrect( this.SelectedMedia, 1 );
			if( !this.WinnerTeam.IsCorrect )
			{
				this.WinnerResult = true;
			}
		}

		private void OpenAnswer2( object obj )
		{
			this.IsVisiblePlayView = true;
			this.PlayView = this.playWinnerView;
			this.SelectedMedia.SortedList[1].IsVisible = true;
			this.WinnerTeam.CheckCorrect( this.SelectedMedia, 2 );
			if( !this.WinnerTeam.IsCorrect )
			{
				this.WinnerResult = true;
			}
		}

		private void OpenAnswerAll( object obj )
		{
			this.IsVisiblePlayView = true;
			this.PlayView = this.playWinnerView;
			this.SelectedMedia.SortedList.ForEach( i => i.IsVisible = true );
			this.WinnerTeam.CheckCorrect( this.SelectedMedia, 4 );
			this.WinnerResult = true;
		}

		private void Reset( object obj )
		{
			this.IsVisiblePlayView = false;
			this.SelectedMedia.Clear();
			this.Teams.ForEach( t => t.Clear() );
			this.WinnerResult = false;
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

		public void CommandRaiseCanExecuteChanged()
		{
			this.OpenWinnerCommand.RaiseCanExecuteChanged();
			this.OpenOtherCommand.RaiseCanExecuteChanged();
			this.OpenAnswer1Command.RaiseCanExecuteChanged();
			this.OpenAnswer2Command.RaiseCanExecuteChanged();
			this.OpenAnswerAllCommand.RaiseCanExecuteChanged();
			this.ResetCommand.RaiseCanExecuteChanged();
		}

		#endregion
	}
}
