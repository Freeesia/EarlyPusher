﻿using System;
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
	public class OrderItemVM : OrderItemVMBase, IBackColorHolder
	{
		private IBackColorHolder parent;
		private ImageSource image;

		public OrderItemVM( IBackColorHolder parent )
		{
			this.parent = parent;
		}

		public ImageSource Image
		{
			get { return this.image; }
			set { SetProperty( ref this.image, value ); }
		}

		public Color BackColor
		{
			get { return this.parent.BackColor; }
		}
	}
}