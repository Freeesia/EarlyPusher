using StFrLibs.Core.Basis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EarlyPusher.Models
{
	public class SettingData : ObservableObject
	{
		public const string FileName = "conf.xml";
		private ObservableHashCollection<TeamData> teamList;
		private string videoDir;
		private string answerSoundPath;

		public ObservableHashCollection<TeamData> TeamList
		{
			get { return teamList; }
		}

		public string VideoDir
		{
			get { return videoDir; }
			set { SetProperty( ref videoDir, value ); }
		}

		public string AnswerSoundPath
		{
			get { return answerSoundPath; }
			set { SetProperty( ref answerSoundPath, value ); }
		}

		public SettingData()
		{
			this.teamList = new ObservableHashCollection<TeamData>();
		}

	}
}
