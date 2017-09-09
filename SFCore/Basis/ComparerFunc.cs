using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFLibs.Core.Basis
{
	public class ComparerFunc<T> : IEqualityComparer<T>
	{
		private Func<T,T,bool> equalFunc;
		private Func<T,int> getHashFunc;

		public ComparerFunc( Func<T, T, bool> equalFunc, Func<T, int> getHashFunc )
		{
			this.equalFunc = equalFunc;
			this.getHashFunc = getHashFunc;
		}

		public bool Equals( T x, T y )
		{
			return this.equalFunc( x, y );
		}

		public int GetHashCode( T obj )
		{
			return this.getHashFunc( obj );
		}
	}
}
