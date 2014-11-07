using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EarlyPusher.Manager
{
	public class DeviceKeyEventArgs : EventArgs
	{
		private Guid instanceID;
		private int key;

		public Guid InstanceID
		{
			get { return instanceID; }
		}

		public int Key
		{
			get { return key; }
		}

		public DeviceKeyEventArgs( Guid id, int key )
		{
			this.instanceID = id;
			this.key = key;
		}
	}
}
