using StFrLibs.Core.Basis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EarlyPusher.Models
{
	public class KeyBindData : ObservableObject
	{
		private Guid deviceGuid;

		public Guid DeviceGuid
		{
			get { return deviceGuid; }
			set { SetProperty( ref deviceGuid, value ); }
		}

		private int key;

		public int Key
		{
			get { return key; }
			set { SetProperty( ref key, value ); }
		}
	}
}
