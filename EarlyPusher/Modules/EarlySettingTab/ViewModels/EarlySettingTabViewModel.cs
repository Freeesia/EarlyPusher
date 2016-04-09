using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EarlyPusher.Modules.EarlySettingTab.Views;
using EarlyPusher.ViewModels;

namespace EarlyPusher.Modules.EarlySettingTab.ViewModels
{
	public class EarlySettingTabViewModel : OperateTabVMBase
	{


		public EarlySettingTabViewModel( MainVM parent ) : base( parent )
		{
			this.View = new EarlySettingTabView();
			this.Header = "早押し設定";
		}
	}
}
