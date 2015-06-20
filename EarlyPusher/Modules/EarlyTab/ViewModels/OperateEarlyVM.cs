using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using EarlyPusher.Models;
using EarlyPusher.Modules.EarlyTab.Views;
using EarlyPusher.ViewModels;
using StFrLibs.Core.Adapters;
using StFrLibs.Core.Basis;
using StFrLibs.Core.Commands;

namespace EarlyPusher.Modules.EarlyTab.ViewModels
{
	public class OperateEarlyVM : OperateTabVMBase
	{
		private ObservableHashVMCollection<MediaVM> medias = new ObservableHashVMCollection<MediaVM>();
		private ObservableVMCollection<TeamData, TeamEarlyVM> teams = new ObservableVMCollection<TeamData, TeamEarlyVM>();
		private ObservableVMCollection<MemberData, MemberEarlyVM> members = new ObservableVMCollection<MemberData, MemberEarlyVM>();

		private ViewModelsAdapter<TeamEarlyVM,TeamData> adapter;

		private PlayEarlyView view = new PlayEarlyView();

		private MediaVM selectedMedia;

		private int rank = 0;

		#region プロパティ

		public DelegateCommand StartCommand { get; private set; }
		public DelegateCommand ResetCommand { get; private set; }

		/// <summary>
		/// チームのリスト
		/// </summary>
		/// <remarks>
		/// プレイウィンドウに表示するチームごとの列のリスト
		/// </remarks>
		public ObservableVMCollection<TeamData, TeamEarlyVM> Teams
		{
			get { return this.teams; }
		}

		/// <summary>
		/// メンバーのリスト
		/// </summary>
		/// <remarks>
		/// オペレーションウィンドウに表示する回答者のリスト
		/// </remarks>
		public ObservableVMCollection<MemberData, MemberEarlyVM> Members
		{
			get { return this.members; }
		}

		/// <summary>
		/// 選択しているメディア
		/// </summary>
		public MediaVM SelectedMedia
		{
			get { return this.selectedMedia; }
			set { SetProperty( ref this.selectedMedia, value, null, SelectedMediaChanging ); }
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

		#endregion

		public OperateEarlyVM( MainVM parent )
			: base( parent )
		{
			this.StartCommand = new DelegateCommand( Start, null );
			this.ResetCommand = new DelegateCommand( Reset, null );
		}

		public override UIElement PlayView
		{
			get { return this.view; }
		}

		#region 設定読み書き

		public override void LoadData()
		{
			base.LoadData();

			this.Parent.Data.PropertyChanged += Data_PropertyChanged;

			this.adapter = new ViewModelsAdapter<TeamEarlyVM, TeamData>( CreateTeamVM, DeleteTeamVM );
			this.adapter.Adapt( this.Teams, this.Parent.Data.TeamList );

			LoadVideos();
		}

		private TeamEarlyVM CreateTeamVM( TeamData arg )
		{
			var vm = new TeamEarlyVM( this, arg );
			vm.Members.CollectionChanged += Members_CollectionChanged;

			return vm;
		}

		private void DeleteTeamVM( TeamEarlyVM obj )
		{
			obj.Members.CollectionChanged -= Members_CollectionChanged;
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

		private void Members_CollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
		{
			if( e.OldItems != null )
			{
				foreach( MemberEarlyVM member in e.OldItems )
				{
					this.Members.Remove( member );
				}
			}

			if( e.NewItems != null )
			{
				foreach( MemberEarlyVM member in e.NewItems )
				{
					this.Members.Add( member );
				}
			}
		}

		private void Data_PropertyChanged( object sender, PropertyChangedEventArgs e )
		{
			switch( e.PropertyName )
			{
				case "VideoDir":
					LoadVideos();
					break;
				default:
					break;
			}
		}

		private void Manager_KeyPushed( object sender, Manager.DeviceKeyEventArgs e )
		{
			var item = this.Members.FirstOrDefault( i => i.Model.DeviceGuid == e.InstanceID && i.Model.Key == e.Key );
			if( item != null && item.Rank == 100 && item.CanAnswer )
			{
				rank++;
				item.Rank = rank;
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

		#region コマンド

		/// <summary>
		/// 解答順位の初期化
		/// </summary>
		/// <param name="obj"></param>
		private void Start( object obj )
		{
			InitRank();
		}

		/// <summary>
		/// 解答権のリセット
		/// </summary>
		/// <param name="obj"></param>
		private void Reset( object obj )
		{
			foreach( var item in this.Members )
			{
				item.CanAnswer = true;
			}
		}

		#endregion

		/// <summary>
		/// メディアフォルダが変更されたとき、メディア一覧を更新します。
		/// </summary>
		private void LoadVideos()
		{
			if( !string.IsNullOrEmpty( this.Parent.Data.EarlyVideoDir ) )
			{
				this.Medias.Clear();
				foreach( string path in Directory.EnumerateFiles( this.Parent.Data.EarlyVideoDir, "*", SearchOption.AllDirectories ) )
				{
					this.Medias.Add( new MediaVM() { FilePath = path } );
				}
			}
		}

		/// <summary>
		/// 順位を初期化します。
		/// </summary>
		private void InitRank()
		{
			this.rank = 0;
			foreach( var i in this.Members )
			{
				i.Rank = 100;
			}
		}
	}
}
