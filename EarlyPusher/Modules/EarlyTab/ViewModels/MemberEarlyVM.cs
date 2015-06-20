using EarlyPusher.Models;
using StFrLibs.Core.Basis;
using StFrLibs.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace EarlyPusher.Modules.EarlyTab.ViewModels
{
	public class MemberEarlyVM : ViewModelBase<MemberData>
	{
		private TeamEarlyVM parent;
		private string rankStr = string.Empty;
		private int sort = -1;
		private int rank = 100;
		private bool canAnswer = true;

		public MemberEarlyVM( TeamEarlyVM parent, MemberData data )
			: base( data )
		{
			this.parent = parent;

			var t = this.parent;
			var s = t.Parent;

			var mID = t.Members.IndexOf( this );
			var tID = s.Teams.IndexOf( t );

			this.sort = tID * 10 + mID;

		}

		public TeamEarlyVM Parent
		{
			get { return this.parent; }
		}

		public int Sort
		{
			get
			{
				return this.sort;
			}
		}

		public int Rank
		{
			get { return this.rank; }
			set
			{
				SetProperty( ref this.rank, value );
				NotifyPropertyChanged( () => this.RankStr );
				NotifyPropertyChanged( () => this.RankPlayStr );
			}
		}

		public string RankStr
		{
			get { return this.rank < 100 ? this.rank.ToString( "D2" ) : string.Empty; }
		}

		public string RankPlayStr
		{
			get { return this.rank < 100 ? this.rank.ToString() : string.Empty; }
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
