using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EarlyPusher.Models;
using StFrLibs.Core.Adapters;
using StFrLibs.Core.Basis;

namespace EarlyPusher.Modules.BinkanOperateTab.ViewModels
{
	public class TeamViewModel : ViewModelBase<TeamData>
	{
		private bool answerable = false;
		private bool pushPermission = true;

		/// <summary>
		/// 解答権
		/// </summary>
		public bool Answerable
		{
			get { return this.answerable; }
			set { SetProperty( ref this.answerable, value ); }
		}
		
		/// <summary>
		/// プッシュ権
		/// </summary>
		public bool Pushable
		{
			get { return this.pushPermission; }
			set { SetProperty( ref this.pushPermission, value ); }
		}

		public TeamViewModel( TeamData model ) : base( model )
		{
		}

		public void Add( int point )
		{
			this.Model.Point += point;
		}
	}
}
