using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFLibs.Core.Basis
{
	public class ObservableHashVMCollection<T> : ObservableHashCollection<T> where T : ViewModelBase
	{
		public ObservableHashVMCollection()
			: base()
		{
		}

		protected override void InsertItem( int index, T item )
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
