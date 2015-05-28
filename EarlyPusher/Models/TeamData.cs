using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using StFrLibs.Core.Basis;

namespace EarlyPusher.Models
{
	public class TeamData : ObservableObject
	{
		private string teamName;
		private Color teamColor;
		private ObservableHashCollection<MemberData> members = new ObservableHashCollection<MemberData>();

		public TeamData()
		{
			this.Members.CollectionChanged += Members_CollectionChanged;
		}

		private void Members_CollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
		{
			if( e.OldItems != null )
			{
				foreach( MemberData mamber in e.OldItems )
				{
					mamber.Parent = null;
				}
			}
			if( e.NewItems != null )
			{
				foreach( MemberData mamber in e.NewItems )
				{
					mamber.Parent = this;
				}
			}
		}

		public string TeamName
		{
			get { return this.teamName; }
			set { SetProperty( ref this.teamName, value ); }
		}

		public Color TeamColor
		{
			get { return this.teamColor; }
			set { SetProperty( ref this.teamColor, value ); }
		}

		public ObservableHashCollection<MemberData> Members
		{
			get { return this.members; }
		}
	}
}
