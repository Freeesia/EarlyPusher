﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EarlyPusher.Models;
using StFrLibs.Core.Adapters;
using StFrLibs.Core.Basis;

namespace EarlyPusher.Modules.CommonSettingTab.ViewModels
{
	public class TeamVM : ViewModelBase<TeamData>
	{
		private ObservableHashVMCollection<MemberVM> members = new ObservableHashVMCollection<MemberVM>();
		private ViewModelsAdapter<MemberVM,MemberData> adapter;

		public ObservableHashVMCollection<MemberVM> Members
		{
			get { return this.members; }
		}

		public TeamVM( TeamData data )
			: base( data )
		{
			this.adapter = new ViewModelsAdapter<MemberVM, MemberData>( CreateMemberVM );
		}

		private MemberVM CreateMemberVM( MemberData data )
		{
			return new MemberVM( this, data );
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