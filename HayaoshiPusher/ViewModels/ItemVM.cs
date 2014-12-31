using HayaoshiPusher.Models;
using StFrLibs.Core.Basis;
using StFrLibs.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HayaoshiPusher.ViewModels
{
	public class ItemVM : ViewModelBase, ISelectable
	{
		private PanelData data;
		private bool isSelected;
		private string rank = string.Empty;
		private bool canAnswer = true;

		public PanelData Data
		{
			get { return this.data; }
			set { SetProperty( ref this.data, value ); }
		}

		public bool IsSelected
		{
			get { return this.isSelected; }
			set { SetProperty( ref this.isSelected, value ); }
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
