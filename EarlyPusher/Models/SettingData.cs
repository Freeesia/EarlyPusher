using StFrLibs.Core.Basis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EarlyPusher.Models
{
	public class SettingData : ObservableObject
	{
		public const string FileName = "conf.xml";

		private ObservableKeyedCollection<string, KeyBindData> keyBindCollection;

		public ObservableKeyedCollection<string, KeyBindData> KeyBindCollection
		{
			get { return keyBindCollection; }
		}

		public SettingData()
		{
			this.keyBindCollection = new ObservableKeyedCollection<string, KeyBindData>( k => k.CollectionKey );
		}

	}
}
