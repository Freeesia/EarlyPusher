using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFLibs.Core.Basis
{
	public class ObservableKeyedCollection<Tkey, Titem> : ObservableFixKeyedCollection<Tkey, Titem> where Titem : ObservableObject
	{
		private Dictionary<Titem, Tkey> revDic = new Dictionary<Titem, Tkey>();

		public ObservableKeyedCollection( Func<Titem, Tkey> getKeyFunc )
			: base( getKeyFunc )
		{
		}

		protected override void InsertItem( int index, Titem item )
		{
			this.revDic.Add( item, this.GetKeyForItem( item ) );
			item.PropertyChanged += Item_PropertyChanged;
			base.InsertItem( index, item );
		}

		protected override void RemoveItem( int index )
		{
			var item = this[index];
			this.revDic.Remove( item );
			item.PropertyChanged -= Item_PropertyChanged;
			base.RemoveItem( index );
		}

		protected override void ClearItems()
		{
			base.ClearItems();
			this.revDic.Clear();
		}

		private void Item_PropertyChanged( object sender, System.ComponentModel.PropertyChangedEventArgs e )
		{
			var item = sender as Titem;
			var nowKey = this.GetKeyForItem( item );
			var oldKey = this.revDic[item];
			if( oldKey.Equals( nowKey ) )
			{
				this.revDic.Remove( item );
				this.revDic.Add( item, nowKey );
				this.Dictionary.Remove( oldKey );
				this.Dictionary.Add( nowKey, item );
			}
		}
	}
}
