using System.Collections.ObjectModel;
using System.Windows.Media;
using SFLibs.Core.Basis;

namespace EarlyPusher.Models
{
    public class TeamData : ObservableObject
    {
        private string teamName;
        private Color teamColor;
        private int point;
        private ObservableCollection<MemberData> members = new ObservableCollection<MemberData>();

        public string TeamName
        {
            get { return this.teamName; }
            set { SetProperty(ref this.teamName, value); }
        }

        public Color TeamColor
        {
            get { return this.teamColor; }
            set { SetProperty(ref this.teamColor, value); }
        }

        public int Point
        {
            get { return this.point; }
            set { SetProperty(ref this.point, value); }
        }

        public ObservableCollection<MemberData> Members
        {
            get { return this.members; }
        }
    }
}
