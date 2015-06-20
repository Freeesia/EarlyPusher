using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Serialization;
using StFrLibs.Core.Basis;

namespace EarlyPusher.Models
{
	public class TeamData : ObservableObject
	{
		private string teamName;
		private Color teamColor;
		private ObservableHashCollection<MemberData> members = new ObservableHashCollection<MemberData>();
		
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
