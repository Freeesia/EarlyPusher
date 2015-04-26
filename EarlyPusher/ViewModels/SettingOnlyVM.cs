using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EarlyPusher.Models;
using SlimDX.DirectInput;
using StFrLibs.Core.Basis;
using StFrLibs.Core.Commands;

namespace EarlyPusher.ViewModels
{
	public class SettingOnlyVM : ViewModelBase<SettingData>
	{
		#region フィールド

		private SettingVM parentVM;
		private ObservableHashCollection<string> devices = new ObservableHashCollection<string>();
		private ObservableHashCollection<TeamVM> teams = new ObservableHashCollection<TeamVM>();
		private TeamVM selectedTeam;

		private ViewModelsAdapter<TeamVM,TeamData> teamAdapter;

		#endregion

		#region プロパティ

		public ObservableHashCollection<string> Devices
		{
			get
			{
				return this.devices;
			}
		}

		public ObservableHashCollection<TeamVM> Teams
		{
			get { return this.teams; }
		}

		public TeamVM SelectedTeam
		{
			get { return this.selectedTeam; }
			set { SetProperty( ref this.selectedTeam, value, SeletedTeamChanged ); }
		}

		public DelegateCommand SearchCommand { get; private set; }

		public DelegateCommand AddTeamCommand { get; private set; }
		public DelegateCommand DelTeamCommand { get; private set; }

		#endregion

		#region 生成構築

		public SettingOnlyVM( SettingData data, SettingVM parent )
			: base( data )
		{
			this.SearchCommand = new DelegateCommand( SearchDevice, null );

			this.AddTeamCommand = new DelegateCommand( AddTeam );
			this.DelTeamCommand = new DelegateCommand( DelTeam, CanDelTeam );

			this.parentVM = parent;

			this.parentVM.Manager.Devices.CollectionChanged += Devices_CollectionChanged;

			this.teamAdapter = new ViewModelsAdapter<TeamVM, TeamData>( CreateTeamVM, DeleteTeamVM );
			this.teamAdapter.Adapt( this.Teams, this.Model.TeamList );
		}

		private TeamVM CreateTeamVM( TeamData data )
		{
			var vm = new TeamVM( data );
			vm.Members.CollectionChanged += Members_CollectionChanged;
			return vm;
		}

		private void DeleteTeamVM( TeamVM vm )
		{
			vm.Members.CollectionChanged -= Members_CollectionChanged;
		}

		private void Members_CollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
		{
			if( e.OldItems != null )
			{
				foreach( MemberVM item in e.OldItems )
				{
					this.parentVM.Members.Remove( item );
				}
			}
			if( e.NewItems != null )
			{
				foreach( MemberVM item in e.NewItems )
				{
					this.parentVM.Members.Add( item );
				}
			}

			if( e.Action == NotifyCollectionChangedAction.Reset )
			{
				var parent = sender as TeamVM;
				this.parentVM.Members.RemoveWhere( vm => vm.Parent == parent );
			}
		}

		private void Devices_CollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
		{
			switch( e.Action )
			{
				case NotifyCollectionChangedAction.Add:
				case NotifyCollectionChangedAction.Remove:
				case NotifyCollectionChangedAction.Replace:
					if( e.NewItems != null )
					{
						foreach( Device d in e.NewItems )
						{
							this.devices.Add( d.Information.InstanceName );
						}
					}
					if( e.OldItems != null )
					{
						foreach( Device d in e.OldItems )
						{
							this.devices.Remove( d.Information.InstanceName );
						}
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					this.devices.Clear();
					break;
				default:
					break;
			}
		}

		#endregion

		#region コマンド

		private void SearchDevice( object obj )
		{
			this.parentVM.Manager.SearchDevice();
		}

		private bool CanDelTeam( object obj )
		{
			return this.SelectedTeam != null;
		}

		private void DelTeam( object obj )
		{
			this.Model.TeamList.Remove( this.SelectedTeam.Model );
			this.SelectedTeam = null;
		}

		private void AddTeam( object obj )
		{
			this.Model.TeamList.Add( new TeamData() { TeamName = "チーム" } );
		}

		#endregion

		private void SeletedTeamChanged( bool obj )
		{
			if( obj )
			{
				this.DelTeamCommand.RaiseCanExecuteChanged();
			}
		}
	}
}
