using StFrLibs.Core.Basis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace EarlyPusher.Models
{
	public class KeyBindData : ObservableObject
	{
		private Guid deviceGuid;
		private int key;

		public Guid DeviceGuid
		{
			get { return deviceGuid; }
			set { SetProperty( ref deviceGuid, value ); }
		}

		public int Key
		{
			get { return key; }
			set { SetProperty( ref key, value ); }
		}

		private Color panelColor;

		public Color PanelColor
		{
			get { return panelColor; }
			set { SetProperty( ref panelColor, value ); }
		}

	}
}
