using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EarlyPusher.Models;
using StFrLibs.Core.Basis;

namespace EarlyPusher.Modules.ChoiceTab.ViewModels
{
	public class TeamChoiceVM : ViewModelBase<TeamData>
	{
		private List<SelectableItemVM> keyList = new List<SelectableItemVM>();
		private int? selectedIndex = null;

		#region プロパティ

		public IReadOnlyList<SelectableItemVM> KeyList
		{
			get { return this.keyList; }
		}

		public int? SelectedIndex
		{
			get { return this.selectedIndex; }
			set { SetProperty( ref this.selectedIndex, value ); }
		}

		#endregion

		public TeamChoiceVM( TeamData data )
			: base( data )
		{
		}

		public override void AttachModel()
		{
			base.AttachModel();

			InitKeyList();
			this.Model.Members.CollectionChanged += Members_CollectionChanged;
		}

		public override void DettachModel()
		{
			base.DettachModel();

			InitKeyList();
			this.Model.Members.CollectionChanged -= Members_CollectionChanged;
		}

		private void Members_CollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
		{
			InitKeyList();
		}

		private void InitKeyList()
		{
			this.keyList.Clear();
			foreach( var member in this.Model.Members.Take( 4 ) )
			{
				this.keyList.Add( new SelectableItemVM() { Parent = this, Device = member.DeviceGuid, Key = member.Key } );
			}
		}

		public bool ExistSelectedItem( Guid device, int key )
		{
			if( this.SelectedIndex != null )
			{
				return false;
			}

			var item = this.KeyList.FirstOrDefault( i => i.Device == device && i.Key == key );
			if( item == null )
			{
				return false;
			}

			foreach( var i in this.KeyList )
			{
				i.IsSelected = false;
			}
			this.SelectedIndex = this.keyList.IndexOf( item );
			item.IsSelected = true;

			return true;
		}

		public void ResetSeletectedItem()
		{
			foreach( var i in this.KeyList )
			{
				i.IsSelected = false;
			}

			this.SelectedIndex = null;
		}
	}
}
