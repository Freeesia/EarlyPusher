using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using EarlyPusher.Basis;
using EarlyPusher.ViewModels;

namespace EarlyPusher.Views
{
	public class PlayViewSelecter : DataTemplateSelector
	{
		public override DataTemplate SelectTemplate( object item, DependencyObject container )
		{
			if( item is SettingVM )
			{
				var vm = item as SettingVM;
				switch( vm.Mode )
				{
					case PlayMode.Choice4:
						return ( (FrameworkElement)container ).FindResource( "choice4ModeTemplate" ) as DataTemplate;
					default:
						return ( (FrameworkElement)container ).FindResource( "earlyModeTemplate" ) as DataTemplate;
				}
			}

			return null;
		}
	}
}
