using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using EarlyPusher.Models;
using EarlyPusher.Modules.SortTab.Interfaces;
using StFrLibs.Core.Basis;
using StFrLibs.Core.Extensions;

namespace EarlyPusher.Modules.SortTab.ViewModels
{
	public class TeamSortVM : ViewModelBase<TeamData>, IBackColorHolder
	{
		private ObservableCollection<SortItemVM> sortedList = new ObservableCollection<SortItemVM>();
		private bool isWinner;
		private int nextIndex = 0;
		private bool isCorrect = true;

		#region プロパティ

		public ObservableCollection<SortItemVM> SortedList
		{
			get { return this.sortedList; }
		}

		public bool IsWinner
		{
			get { return this.isWinner; }
			set { SetProperty( ref this.isWinner, value ); }
		}

		public Color BackColor
		{
			get { return this.Model.TeamColor; }
		}

		public bool IsCorrect
		{
			get { return this.isCorrect; }
			set { SetProperty( ref this.isCorrect, value ); }
		}
						
		#endregion

		public TeamSortVM( TeamData data )
			: base( data )
		{
			for( int i = 0; i < 4; i++ )
			{
				this.SortedList.Add( new SortItemVM( this ) );
			}
		}

		public void Clear()
		{
			this.nextIndex = 0;
			foreach( var item in this.SortedList )
			{
				item.Choice = null;
				item.IsVisible = false;
			}
			this.IsCorrect = true;
		}

		public bool SetKey( Guid device, int key )
		{
			if( this.nextIndex > 3 )
			{
				return false;
			}

			var data = this.Model.Members.FirstOrDefault( d => d.DeviceGuid == device && d.Key == key );
			if( data == null )
			{
				return false;
			}

			var choice =(Choice)this.Model.Members.IndexOf( data );

			if( this.SortedList.Any( i => i.Choice == choice ) )
			{
				return false;
			}

			this.SortedList[this.nextIndex].Choice = choice;
			this.nextIndex++;

			return true;
		}

		public void CheckCorrect( SortMediaVM media )
		{
			if( this.SortedList.Any( i => i.Choice == null ) )
			{
				return;
			}

			this.IsCorrect = this.SortedList.SequenceEqual( media.SortedList, new ComparerFunc<SortItemVM>( SortItemEqual, SortItemGetHash ) );
		}

		private int SortItemGetHash( SortItemVM arg )
		{
			return arg.Choice.GetHashCode();
		}

		private bool SortItemEqual( SortItemVM arg1, SortItemVM arg2 )
		{
			return arg1.Choice == arg2.Choice;
		}
	}
}
