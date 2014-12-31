using EarlyPusher.Models;
using StFrLibs.Core.Basis;
using StFrLibs.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace EarlyPusher.ViewModels
{
	public class ItemVM : ViewModelBase
	{
		private PanelData data;
		private string rank = string.Empty;
		private bool canAnswer = true;

		public PanelData Data
		{
			get { return this.data; }
			set { SetProperty( ref this.data, value ); }
		}

		public string Rank
		{
			get { return this.rank; }
			set { SetProperty( ref this.rank, value ); }
		}

		public bool CanAnswer
		{
			get { return this.canAnswer; }
			set 
			{ 
				SetProperty( ref this.canAnswer, value );
				NotifyPropertyChanged( "Opacity" );
			}
		}

		public double Opacity
		{
			get
			{
				return this.canAnswer ? 1.0 : 0.5;
			}
		}

	}
}
