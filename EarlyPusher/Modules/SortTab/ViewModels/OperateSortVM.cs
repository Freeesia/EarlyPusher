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

namespace EarlyPusher.Modules.SortTab.ViewModels
{
	public class OperateSortVM : OperateTabVMBase
	{
		private ObservableHashVMCollection<MediaVM> medias = new ObservableHashVMCollection<MediaVM>();
		private ObservableVMCollection<TeamData,TeamSortVM> teams = new ObservableVMCollection<TeamData, TeamSortVM>();
		private ViewModelsAdapter<TeamSortVM,TeamData> adapter;

		private MediaVM selectedMedia;
		private bool isPlayVisible;

		#region プロパティ

		public DelegateCommand OpenCommand { get; private set; }
		public DelegateCommand ResetCommand { get; private set; }

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

		public ObservableVMCollection<TeamData, TeamSortVM> Teams
		{
			get { return this.teams; }
		}

		/// <summary>
		/// 選択しているメディア
		/// </summary>
		public MediaVM SelectedMedia
		{
			get { return this.selectedMedia; }
			set { SetProperty( ref this.selectedMedia, value, null, SelectedMediaChanging ); }
		}

		public bool IsPlayVisible
		{
			get { return this.isPlayVisible; }
			set { SetProperty( ref this.isPlayVisible, value ); }
		}

		public override UIElement PlayView
		{
			get { return new PlaySortView(); }
		}

		#endregion

		public OperateSortVM( MainVM parent )
			: base( parent )
		{
			this.OpenCommand = new DelegateCommand( Open );
			this.ResetCommand = new DelegateCommand( Reset );

			this.adapter = new ViewModelsAdapter<TeamSortVM, TeamData>( CreateTeamSortVM );
		}

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

		public override void Activate()
		{
			this.Parent.Manager.KeyPushed += Manager_KeyPushed;
		}

		public override void Deactivate()
		{
			this.Parent.Manager.KeyPushed -= Manager_KeyPushed;
		}

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

		#region コマンド関係

		private void Open( object obj )
		{
			this.IsPlayVisible = true;
		}

		private void Reset( object obj )
		{
			foreach( var team in this.Teams )
			{
				team.Clear();
			}
			this.IsPlayVisible = false;
		}

		#endregion

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
					var media = new MediaVM() { FilePath = path, FileName = Path.GetFileName( path ) };
					media.LoadFile();
					this.Medias.Add( media );
				}
			}
		}
	}
}
