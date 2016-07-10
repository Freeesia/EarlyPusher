using System;
using System.Linq;
using EarlyPusher.Models;
using StFrLibs.Core.Adapters;
using StFrLibs.Core.Basis;

namespace EarlyPusher.ViewModels
{
	public class TeamViewModel : ViewModelBase<TeamData>
	{
		private ViewModelsAdapter<MemberViewModel,MemberData> adapter;
		private bool pushPermission = true;

		/// <summary>
		/// 解答権
		/// </summary>
		public bool Answerable
		{
			get { return this.Members.Any( m => m.Answerable ); }
		}

		/// <summary>
		/// プッシュ権
		/// </summary>
		public bool Pushable
		{
			get { return this.pushPermission; }
			set { SetProperty( ref this.pushPermission, value ); }
		}

		public ObservableHashVMCollection<MemberViewModel> Members { get; } = new ObservableHashVMCollection<MemberViewModel>();

		public TeamViewModel( TeamData model ) : base( model )
		{
			this.adapter = new ViewModelsAdapter<MemberViewModel, MemberData>( CreateMemberVM, DeleteMemberVM );
		}

		private MemberViewModel CreateMemberVM( MemberData data )
		{
			var vm = new MemberViewModel( this, data );
			vm.PropertyChanged += MemberViewModel_PropertyChanged;

			return vm;
		}

		private void DeleteMemberVM( MemberViewModel vm )
		{
			vm.PropertyChanged += MemberViewModel_PropertyChanged;
		}

		private void MemberViewModel_PropertyChanged( object sender, System.ComponentModel.PropertyChangedEventArgs e )
		{
			if( e.PropertyName == nameof( MemberViewModel.Answerable ) )
			{
				NotifyPropertyChanged( nameof( this.Answerable ) );
			}
		}

		public override void AttachModel()
		{
			this.adapter.Adapt( this.Members, this.Model.Members );

			base.AttachModel();
		}

		public override void DettachModel()
		{
			this.Members.Clear();

			base.DettachModel();
		}

		public void Add( int point )
		{
			this.Model.Point += point;
		}
	}
}
