using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using EarlyPusher.Modules.SortTab.Views;
using EarlyPusher.ViewModels;

namespace EarlyPusher.Modules.SortTab.ViewModels
{
	public class OperateSortVM : OperateTabVMBase
	{
		public override UIElement PlayView
		{
			get { return new PlaySortView(); }
		}

		public OperateSortVM( MainVM parent )
			: base( parent )
		{
		}
	}
}
