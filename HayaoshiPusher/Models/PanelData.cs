using StFrLibs.Core.Basis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace HayaoshiPusher.Models
{
	public class PanelData : ObservableObject
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

		private string name;

		public string Name
		{
			get { return name; }
			set { SetProperty( ref name, value ); }
		}

	}
}
