using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFLibs.Core.Extensions
{
	public static class IEnumerableExtension
	{
		public static void ForEach<T>( this IEnumerable<T> list, Action<T> action )
		{
			foreach( var item in list )
			{
				action( item );
			}
		}
	}
}
