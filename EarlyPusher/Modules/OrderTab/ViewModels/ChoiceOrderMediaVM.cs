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
using EarlyPusher.Modules.OrderTab.Interfaces;
using System.Windows.Media.Imaging;

namespace EarlyPusher.Modules.OrderTab.ViewModels
{
	public class ChoiceOrderMediaVM : MediaVM
	{
		private ObservableCollection<CurrectOrederItemVM> sortedList = new ObservableCollection<CurrectOrederItemVM>();
		private ChoiceOrderMediaData model;

		public ObservableCollection<CurrectOrederItemVM> SortedList
		{
			get { return this.sortedList; }
		}

		public ChoiceOrderMediaData Model
		{
			get { return this.model; }
		}

		public ChoiceOrderMediaVM( ChoiceOrderMediaData model ) : base()
		{
			this.model = model;

			for( int i = 0; i < 4; i++ )
			{
				var item = new CurrectOrederItemVM();
				item.Choice = this.model.ChoiceOrder[i];
				switch( item.Choice )
				{
					case Choice.A:
						if( !string.IsNullOrEmpty(this.model.ChoiceAImagePath) )
						{
							item.Image = new BitmapImage( new Uri( this.model.ChoiceAImagePath ) );
						}
						break;
					case Choice.B:
						if( !string.IsNullOrEmpty(this.model.ChoiceBImagePath) )
						{
							item.Image = new BitmapImage( new Uri( this.model.ChoiceBImagePath ) );
						}
						break;
					case Choice.C:
						if( !string.IsNullOrEmpty(this.model.ChoiceCImagePath) )
						{
							item.Image = new BitmapImage( new Uri( this.model.ChoiceCImagePath ) );
						}
						break;
					case Choice.D:
						if( !string.IsNullOrEmpty(this.model.ChoiceDImagePath) )
						{
							item.Image = new BitmapImage( new Uri( this.model.ChoiceDImagePath ) );
						}
						break;
				}

				this.SortedList.Add( item );
			}

			this.FilePath = this.model.MediaPath;
			this.FileName = Path.GetFileName( this.FilePath );
			this.LoadFile();
		}

		public void Clear()
		{
			this.SortedList.ForEach( i => i.IsVisible = false );
		}
	}
}
