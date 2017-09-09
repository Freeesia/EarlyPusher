using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFLibs.Core.Basis
{
	public class ObservableHashCollection<T> : ObservableFixKeyedCollection<int, T>
	{
		public ObservableHashCollection()
			: base( GetHashKey )
		{
		}

		private static int GetHashKey( T item )
		{
			return item.GetHashCode();
		}
	}
}
