using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Windows;
using EarlyPusher.Manager;
using EarlyPusher.Models;
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
		private MediaVM selectedMedia;
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
				yield return new { Top = 0.0, Left = 0.0, SelectableItems = this.Teams.Select( t => t.KeyList[0]) };
				yield return new { Top = 0.0, Left = 500.0, SelectableItems = this.Teams.Select( t => t.KeyList[1] ) };
				yield return new { Top = 200.0, Left = 0.0, SelectableItems = this.Teams.Select( t => t.KeyList[2] ) };
				yield return new { Top = 200.0, Left = 500.0, SelectableItems = this.Teams.Select( t => t.KeyList[3] ) };
			}
		}

		public override UIElement PlayView
		{
			get { return this.view; }
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

		/// <summary>
		/// 選択を表示する。
		/// </summary>
		public bool IsChoiceVisible
		{
			get { return this.isChoiceVisible; }
			set { SetProperty( ref this.isChoiceVisible, value ); }
		}

		#endregion

		public OperateChoiceVM( MainVM parent )
			: base( parent )
		{

			this.OpenCommand = new DelegateCommand( Open );
			this.ResetCommand = new DelegateCommand( Reset );
		}

		#region 設定読み書き

		public override void LoadData()
		{
			base.LoadData();

			this.teamAdapter = new ViewModelsAdapter<TeamChoiceVM, TeamData>( CreateTeamVM );
			this.teamAdapter.Adapt( this.Teams, this.Parent.Data.TeamList );

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

		/// <summary>
		/// メディアフォルダが変更されたとき、メディア一覧を更新します。
		/// </summary>
		private void LoadVideos()
		{
			if( !string.IsNullOrEmpty( this.Parent.Data.ChoiceVideoDir ) )
			{
				this.Medias.Clear();
				foreach( string path in Directory.EnumerateFiles( this.Parent.Data.ChoiceVideoDir, "*", SearchOption.AllDirectories ) )
				{
					this.Medias.Add( new MediaVM() { FilePath = path } );
				}
			}
		}
	}
}
