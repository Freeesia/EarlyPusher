using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using EarlyPusher.Models;
using StFrLibs.Core.Basis;

namespace EarlyPusher.Modules.OrderTab.ViewModels
{
	public class CurrectOrederItemVM : OrderItemVMBase
	{
		private ImageSource image;
		private bool isVisible = false;

		public ImageSource Image
		{
			get { return this.image; }
			set { SetProperty( ref this.image, value ); }
		}

		public bool IsVisible
		{
			get { return this.isVisible; }
			set { SetProperty( ref this.isVisible, value ); }
		}
	}
}
