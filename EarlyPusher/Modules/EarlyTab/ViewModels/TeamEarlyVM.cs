using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EarlyPusher.Models;
using StFrLibs.Core.Adapters;
using StFrLibs.Core.Basis;

namespace EarlyPusher.Modules.EarlyTab.ViewModels
{
	public class TeamEarlyVM : ViewModelBase<TeamData>
	{
		private ObservableHashVMCollection<MemberEarlyVM> members = new ObservableHashVMCollection<MemberEarlyVM>();
		private ViewModelsAdapter<MemberEarlyVM,MemberData> adapter;

		public ObservableHashVMCollection<MemberEarlyVM> Members
		{
			get { return this.members; }
		}

		public TeamEarlyVM( TeamData data )
			: base( data )
		{
			this.adapter = new ViewModelsAdapter<MemberEarlyVM, MemberData>( CreateMemberVM );
		}

		private MemberEarlyVM CreateMemberVM( MemberData data )
		{
			return new MemberEarlyVM( this, data );
		}

		public override void AttachModel()
		{
			this.adapter.Adapt( this.members, this.Model.Members );

			base.AttachModel();
		}

		public override void DettachModel()
		{
			this.members.Clear();

			base.DettachModel();
		}
	}
}
