using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using StFrLibs.Core.Basis;

namespace EarlyPusher.Modules.TimeShockTab.ViewModels
{
	public class ImageItemViewModel : ObservableObject
	{
		private BitmapImage image;
		private bool isVisible;

		public BitmapImage Image
		{
			get { return this.image; }
			set { SetProperty( ref this.image, value ); }
		}

		public bool IsVisible
		{
			get { return this.isVisible; }
			set { SetProperty( ref this.isVisible, value ); }
		}

		public ImageItemViewModel( string path )
		{
			this.Image = new BitmapImage( new Uri( path ) );
		}
	}
}
