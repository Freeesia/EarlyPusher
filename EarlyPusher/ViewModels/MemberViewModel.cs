using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EarlyPusher.Models;
using StFrLibs.Core.Basis;

namespace EarlyPusher.ViewModels
{
	public class MemberViewModel : ViewModelBase<MemberData>
	{
		public TeamViewModel Parent { get; }

		private bool answerable;

		public bool Answerable
		{
			get { return this.answerable; }
			set { SetProperty( ref this.answerable, value ); }
		}
		
		public MemberViewModel( TeamViewModel parent, MemberData model ) : base( model )
		{
			this.Parent = parent;
		}
	}
}
