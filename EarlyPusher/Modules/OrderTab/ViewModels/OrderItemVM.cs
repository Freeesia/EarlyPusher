using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using EarlyPusher.Models;
using EarlyPusher.Modules.OrderTab.Interfaces;
using StFrLibs.Core.Basis;

namespace EarlyPusher.Modules.OrderTab.ViewModels
{
	public class OrderItemVM : ObservableObject, IBackColorHolder
	{
		private Choice? choice = null;
		private bool isVisible = false;

		private IBackColorHolder parent;

		public OrderItemVM( IBackColorHolder parent )
		{
			this.parent = parent;
		}

		public Choice? Choice
		{
			get { return this.choice; }
			set { SetProperty( ref this.choice, value ); }
		}

		public bool IsVisible
		{
			get { return this.isVisible; }
			set { SetProperty( ref this.isVisible, value ); }
		}

		public Color BackColor
		{
			get { return this.parent.BackColor; }
		}
	}
}
