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
		private ObservableHashCollection<TeamData> teamList = new ObservableHashCollection<TeamData>();
		private string earlyVideoDir;
		private string choiceVideoDir;
		private string answerSoundPath;

		public ObservableHashCollection<TeamData> TeamList
		{
			get { return teamList; }
		}

		public string EarlyVideoDir
		{
			get { return earlyVideoDir; }
			set { SetProperty( ref earlyVideoDir, value ); }
		}

		public string ChoiceVideoDir
		{
			get { return this.choiceVideoDir; }
			set { SetProperty( ref this.choiceVideoDir, value ); }
		}
		
		public string AnswerSoundPath
		{
			get { return answerSoundPath; }
			set { SetProperty( ref answerSoundPath, value ); }
		}
	}
}
