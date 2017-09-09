using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFLibs.Core.Basis
{
	public class ObservableVMCollection<TM, TVM> : ObservableFixKeyedCollection<TM, TVM>
		where TVM : ViewModelBase<TM>
		where TM : ObservableObject
	{
		public ObservableVMCollection()
			: base( GetModel )
		{
		}

		private static TM GetModel( TVM item )
		{
			return item.Model;
		}

		protected override void InsertItem( int index, TVM item )
		{
			item.AttachModel();
			base.InsertItem( index, item );
		}

		protected override void RemoveItem( int index )
		{
			var item = this[index];
			item.DettachModel();
			base.RemoveItem( index );
		}
	}
}
