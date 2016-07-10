using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EarlyPusher.Models;
using StFrLibs.Core.Basis;

namespace EarlyPusher.Modules.CommonSettingTab.ViewModels
{
	public class CommonMemberVM : ViewModelBase<MemberData>
	{
		private bool isKeyLock = true;

		public bool IsKeyLock
		{
			get { return this.isKeyLock; }
			set { SetProperty( ref this.isKeyLock, value ); }
		}

		public CommonTeamVM Parent { get; }

		public CommonMemberVM( CommonTeamVM parent, MemberData data )
			: base( data )
		{
			this.Parent = parent;
		}
	}
}
