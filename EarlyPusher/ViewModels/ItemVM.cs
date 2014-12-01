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
	public class ItemVM : ViewModelBase, ISelectable
	{
		private KeyBindData data;
		private bool isSelected;
		private string rank = string.Empty;
		private Color color;

		public KeyBindData Data
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

		public Color PanelColor
		{
			get { return this.color; }
			set { SetProperty( ref this.color, value ); }
		}
	}
}
