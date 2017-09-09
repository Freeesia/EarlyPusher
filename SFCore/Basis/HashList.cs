using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFLibs.Core.Basis
{
	public class HashList<T> : KeyedList<T, T>
	{
		public HashList()
			: base( i => i )
		{
		}
	}
}
