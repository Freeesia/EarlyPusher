using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StFrLibs.Core.Extensions;
using EarlyPusher.Models;
using EarlyPusher.ViewModels;
using System.Windows.Media;
using EarlyPusher.Modules.SortTab.Interfaces;

namespace EarlyPusher.Modules.SortTab.ViewModels
{
	public class SortMediaVM : MediaVM, IBackColorHolder
	{
		private ObservableCollection<SortItemVM> sortedList = new ObservableCollection<SortItemVM>();

		public ObservableCollection<SortItemVM> SortedList
		{
			get { return this.sortedList; }
		}

		public Color BackColor
		{
			get { return Colors.White; }
		}

		public SortMediaVM()
			: base()
		{
			for( int i = 0; i < 4; i++ )
			{
				this.SortedList.Add( new SortItemVM( this ) );
			}

			this.PropertyChanged += SortMediaVM_PropertyChanged;
		}

		public void Clear()
		{
			this.SortedList.ForEach( i => i.IsVisible = false );
		}

		private void SortMediaVM_PropertyChanged( object sender, System.ComponentModel.PropertyChangedEventArgs e )
		{
			if( e.PropertyName == "FilePath" && !string.IsNullOrEmpty( this.FilePath ) )
			{
				var fileName = Path.GetFileNameWithoutExtension( this.FilePath );
				var lstIndex = fileName.LastIndexOf( '_' );
				if( lstIndex == -1 )
				{
					return;
				}

				var sortStr = fileName.Substring( lstIndex + 1 );
				if( string.IsNullOrEmpty(sortStr) )
				{
					return;
				}

				for( int i = 0; i < 4; i++ )
				{
					Choice c;
					if( !Enum.TryParse<Choice>( sortStr[i].ToString(), out c ) )
					{
						return;
					}
					this.SortedList[i].Choice = c;
				}

			}
		}
	}
}
