using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EarlyPusher.Models;
using StFrLibs.Core.Basis;

namespace EarlyPusher.Modules.CommonSettingTab.ViewModels
{
	public class MemberVM : ViewModelBase<MemberData>
	{
		private TeamVM parent;

		private bool isKeyLock = true;

		public bool IsKeyLock
		{
			get { return this.isKeyLock; }
			set { SetProperty( ref this.isKeyLock, value ); }
		}
		
		public MemberVM( TeamVM parent, MemberData data )
			: base( data )
		{
			this.parent = parent;
		}

		public TeamVM Parent
		{
			get { return this.parent; }
		}

	}
}
