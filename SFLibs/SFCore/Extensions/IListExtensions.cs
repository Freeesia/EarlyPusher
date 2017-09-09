using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFLibs.Core.Extensions
{
	public static class IListExtensions
	{
		public static void RemoveWhere<T>( this IList<T> list, Func<T, bool> where )
		{
			foreach( var item in list.Where( where ).ToArray() )
			{
				list.Remove( item );
			}
		}
	}
}
