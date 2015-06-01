using System;
using System.Collections;
using System.Linq;
using System.Windows;
using EarlyPusher.Manager;
using EarlyPusher.Models;
using EarlyPusher.Modules.EarlyTab.Views;
using EarlyPusher.ViewModels;
using StFrLibs.Core.Basis;
using StFrLibs.Core.Commands;

namespace EarlyPusher.Modules.ChoiceTab.ViewModels
{
	public class OperateChoiceVM : OperateTabVMBase
	{
		private PlayChoiceView view = new PlayChoiceView();

		private ObservableHashVMCollection<TeamChoiceVM> teams = new ObservableHashVMCollection<TeamChoiceVM>();
		private ViewModelsAdapter<TeamChoiceVM,TeamData> teamAdapter;
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

		private void Manager_KeyPushed( object sender, DeviceKeyEventArgs e )
		{
			foreach( var team in this.Teams )
			{
				if( team.ExistSelectedItem(e.InstanceID, e.Key) )
				{
					return;
				}
			}
		}
	}
}
