using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EarlyPusher.Models;
using StFrLibs.Core.Basis;

namespace EarlyPusher.Modules.OrderTab.ViewModels
{
	public abstract class OrderItemVMBase : ObservableObject
	{
		private Choice? choice;

		public Choice? Choice
		{
			get { return this.choice; }
			set { SetProperty( ref this.choice, value ); }
		}
	}
}
