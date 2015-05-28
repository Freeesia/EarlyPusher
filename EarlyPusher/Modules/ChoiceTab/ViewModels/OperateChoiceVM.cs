using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using EarlyPusher.Modules.EarlyTab.Views;
using EarlyPusher.ViewModels;

namespace EarlyPusher.Modules.ChoiceTab.ViewModels
{
	public class OperateChoiceVM : OperateTabVMBase
	{
		private PlayChoiceView view = new PlayChoiceView();

		public OperateChoiceVM( MainVM parent )
			: base( parent )
		{
		}

		public override UIElement PlayView
		{
			get { return this.view; }
		}
	}
}
