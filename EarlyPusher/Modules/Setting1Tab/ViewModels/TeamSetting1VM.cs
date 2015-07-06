using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EarlyPusher.Models;
using StFrLibs.Core.Adapters;
using StFrLibs.Core.Basis;

namespace EarlyPusher.Modules.Setting1Tab.ViewModels
{
	public class TeamSetting1VM : ViewModelBase<TeamData>
	{
		private ObservableHashVMCollection<MemberSetting1VM> members = new ObservableHashVMCollection<MemberSetting1VM>();
		private ViewModelsAdapter<MemberSetting1VM,MemberData> adapter;

		public ObservableHashVMCollection<MemberSetting1VM> Members
		{
			get { return this.members; }
		}

		public TeamSetting1VM( TeamData data )
			: base( data )
		{
			this.adapter = new ViewModelsAdapter<MemberSetting1VM, MemberData>( CreateMemberVM );
		}

		private MemberSetting1VM CreateMemberVM( MemberData data )
		{
			return new MemberSetting1VM( this, data );
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
