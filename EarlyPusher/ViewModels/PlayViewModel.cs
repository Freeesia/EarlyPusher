using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StFrLibs.Core.Basis;

namespace EarlyPusher.ViewModels
{
	public class PlayViewModel : ViewModelBase
	{
		public ObservableHashCollection<TeamVM> Teams
		{
			get;
			private set;
		}

		public PlayViewModel( ObservableHashCollection<TeamVM> teams )
		{
			this.Teams = teams;
		}
	}
}
