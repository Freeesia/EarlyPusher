using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SFLibs.Core.Interfaces;

namespace SFLibs.Core.Basis
{
	public class ObservableFixKeyedCollection<Tkey, Titem> : KeyedList<Tkey, Titem>, INotifyCollectionChanged, INotifyPropertyChanged, INotifyPropertyChanging
	{
		private const string IndexerName = "Item[]";

		public event NotifyCollectionChangedEventHandler CollectionChanged;
		public event PropertyChangingEventHandler PropertyChanging;
		public event PropertyChangedEventHandler PropertyChanged;

		public ObservableFixKeyedCollection( Func<Titem, Tkey> getKeyFunc )
			: base( getKeyFunc )
		{
		}

		protected override void InsertItem( int index, Titem item )
		{
			base.InsertItem( index, item );

			NotifyPropertyChanged( () => this.Count );
			NotifyPropertyChanged( IndexerName );
			OnCollectionChanged( NotifyCollectionChangedAction.Add, item, index );
		}

		protected override void RemoveItem( int index )
		{
			var item = this[index];
			base.RemoveItem( index );

			NotifyPropertyChanged( () => this.Count );
			NotifyPropertyChanged( IndexerName );
			OnCollectionChanged( NotifyCollectionChangedAction.Remove, item, index );
		}

		protected override void SetItem( int index, Titem item )
		{
			var originalItem = this[index];
			base.SetItem( index, item );

			NotifyPropertyChanged( IndexerName );
			OnCollectionChanged( NotifyCollectionChangedAction.Replace, originalItem, item, index );
		}

		protected virtual void MoveItem( int oldIndex, int newIndex )
		{
			var removedItem = this[oldIndex];

			base.RemoveItem( oldIndex );
			base.InsertItem( newIndex, removedItem );

			NotifyPropertyChanged( IndexerName );
			OnCollectionChanged( NotifyCollectionChangedAction.Move, removedItem, newIndex, oldIndex );
		}

		protected override void ClearItems()
		{
			base.ClearItems();

			NotifyPropertyChanged( () => this.Count );
			NotifyPropertyChanged( IndexerName );
			OnCollectionReset();
		}

		#region コレクション変更

		/// <summary>
		/// Helper to raise CollectionChanged event to any listeners
		/// </summary>
		private void OnCollectionChanged( NotifyCollectionChangedAction action, object item, int index )
		{
			OnCollectionChanged( new NotifyCollectionChangedEventArgs( action, item, index ) );
		}

		/// <summary>
		/// Helper to raise CollectionChanged event to any listeners
		/// </summary>
		private void OnCollectionChanged( NotifyCollectionChangedAction action, object item, int index, int oldIndex )
		{
			OnCollectionChanged( new NotifyCollectionChangedEventArgs( action, item, index, oldIndex ) );
		}

		/// <summary>
		/// Helper to raise CollectionChanged event to any listeners
		/// </summary>
		private void OnCollectionChanged( NotifyCollectionChangedAction action, object oldItem, object newItem, int index )
		{
			OnCollectionChanged( new NotifyCollectionChangedEventArgs( action, newItem, oldItem, index ) );
		}

		/// <summary>
		/// Helper to raise CollectionChanged event with action == Reset to any listeners
		/// </summary>
		private void OnCollectionReset()
		{
			OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Reset ) );
		}

		protected virtual void OnCollectionChanged( NotifyCollectionChangedEventArgs e )
		{
			var d = this.CollectionChanged;
			if( d != null )
			{
				d( this, e );
			}
		}

		#endregion

		#region プロパティ変更

		public void NotifyPropertyChanged<MemberType>( Expression<Func<MemberType>> expression )
		{
			NotifyPropertyChanged( ( (MemberExpression)expression.Body ).Member.Name );
		}

		public virtual void NotifyPropertyChanged( string propertyName )
		{
			var d = this.PropertyChanged;
			if( d != null )
			{
				d( this, new PropertyChangedEventArgs( propertyName ) );
			}
		}

		public void NotifyPropertyChanging<MemberType>( Expression<Func<MemberType>> expression )
		{
			NotifyPropertyChanging( ( (MemberExpression)expression.Body ).Member.Name );
		}

		public virtual void NotifyPropertyChanging( string propertyName )
		{
			var d = this.PropertyChanging;
			if( d != null )
			{
				d( this, new PropertyChangingEventArgs( propertyName ) );
			}
		}

		#endregion
	}
}
