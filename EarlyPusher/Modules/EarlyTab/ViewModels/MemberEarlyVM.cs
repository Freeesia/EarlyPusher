﻿using EarlyPusher.Models;
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
		private int rank = 100;
		private bool canAnswer = true;

		public MemberEarlyVM( TeamEarlyVM parent, MemberData data )
			: base( data )
		{
			this.parent = parent;
		}

		public TeamEarlyVM Parent
		{
			get { return this.parent; }
		}

		public int Rank
		{
			get { return this.rank; }
			set
			{
				SetProperty( ref this.rank, value );
				NotifyPropertyChanged( () => this.RankStr );
			}
		}

		public string RankStr
		{
			get { return this.rank < 100 ? this.rank.ToString( "D2" ) : string.Empty; }
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