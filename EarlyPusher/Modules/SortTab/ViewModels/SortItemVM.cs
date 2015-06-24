using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EarlyPusher.Models;
using StFrLibs.Core.Basis;

namespace EarlyPusher.Modules.SortTab.ViewModels
{
	public class SortItemVM : ObservableObject
	{
		private Choice? choice = null;

		public Choice? Choice
		{
			get { return this.choice; }
			set { SetProperty( ref this.choice, value ); }
		}
	}
}
