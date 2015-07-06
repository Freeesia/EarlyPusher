using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EarlyPusher.Models;
using StFrLibs.Core.Basis;

namespace EarlyPusher.Modules.Setting1Tab.ViewModels
{
	public class MemberSetting1VM : ViewModelBase<MemberData>
	{
		private TeamSetting1VM parent;

		private bool isKeyLock = true;

		public bool IsKeyLock
		{
			get { return this.isKeyLock; }
			set { SetProperty( ref this.isKeyLock, value ); }
		}
		
		public MemberSetting1VM( TeamSetting1VM parent, MemberData data )
			: base( data )
		{
			this.parent = parent;
		}

		public TeamSetting1VM Parent
		{
			get { return this.parent; }
		}

	}
}
