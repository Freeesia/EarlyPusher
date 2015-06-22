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
		private Choice? selectedChoice;

		#region プロパティ

		public IReadOnlyList<SelectableItemVM> KeyList
		{
			get { return this.keyList; }
		}

		public Choice? SelectedChoice
		{
			get { return this.selectedChoice; }
			set { SetProperty( ref this.selectedChoice, value ); }
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
			if( this.SelectedChoice != null )
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
			this.SelectedChoice = (Choice)this.keyList.IndexOf( item );
			item.IsSelected = true;

			return true;
		}

		public void ResetSeletectedItem()
		{
			foreach( var i in this.KeyList )
			{
				i.IsSelected = false;
			}

			this.SelectedChoice = null;
		}
	}
}
