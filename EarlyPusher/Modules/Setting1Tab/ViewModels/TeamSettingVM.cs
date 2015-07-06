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
	public class TeamSettingVM : ViewModelBase<TeamData>
	{
		private ObservableHashVMCollection<MemberSettingVM> members = new ObservableHashVMCollection<MemberSettingVM>();
		private ViewModelsAdapter<MemberSettingVM,MemberData> adapter;

		public ObservableHashVMCollection<MemberSettingVM> Members
		{
			get { return this.members; }
		}

		public TeamSettingVM( TeamData data )
			: base( data )
		{
			this.adapter = new ViewModelsAdapter<MemberSettingVM, MemberData>( CreateMemberVM );
		}

		private MemberSettingVM CreateMemberVM( MemberData data )
		{
			return new MemberSettingVM( this, data );
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
