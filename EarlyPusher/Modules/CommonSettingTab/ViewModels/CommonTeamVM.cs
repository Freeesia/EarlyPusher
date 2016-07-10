using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EarlyPusher.Models;
using StFrLibs.Core.Adapters;
using StFrLibs.Core.Basis;

namespace EarlyPusher.Modules.CommonSettingTab.ViewModels
{
	public class CommonTeamVM : ViewModelBase<TeamData>
	{
		private ObservableHashVMCollection<CommonMemberVM> members = new ObservableHashVMCollection<CommonMemberVM>();
		private ViewModelsAdapter<CommonMemberVM,MemberData> adapter;

		public ObservableHashVMCollection<CommonMemberVM> Members
		{
			get { return this.members; }
		}

		public CommonTeamVM( TeamData data )
			: base( data )
		{
			this.adapter = new ViewModelsAdapter<CommonMemberVM, MemberData>( CreateMemberVM );
		}

		private CommonMemberVM CreateMemberVM( MemberData data )
		{
			return new CommonMemberVM( this, data );
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
