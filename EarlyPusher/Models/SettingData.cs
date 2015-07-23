using StFrLibs.Core.Basis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace EarlyPusher.Models
{
	public class SettingData : ObservableObject
	{
		public const string FileName = "conf.xml";
		private ObservableHashCollection<TeamData> teamList = new ObservableHashCollection<TeamData>();
		private ObservableKeyedCollection<string,ChoiceOrderMediaData> choiceOrderMediaList = new ObservableKeyedCollection<string, ChoiceOrderMediaData>( m => m.MediaPath );
		private string earlyVideoDir;
		private string choiceVideoDir;
		private string sortVideoDir;
		private string answerSoundPath;
		private string correctSoundPath;
		private string missSoundPath;
		private string checkSoundPath;


		public ObservableHashCollection<TeamData> TeamList
		{
			get { return teamList; }
		}

		public ObservableKeyedCollection<string, ChoiceOrderMediaData> ChoiceOrderMediaList
		{
			get { return choiceOrderMediaList; }
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

		public string SortVideoDir
		{
			get { return this.sortVideoDir; }
			set { SetProperty( ref this.sortVideoDir, value, SortVideoDirChanged ); }
		}
				
		public string AnswerSoundPath
		{
			get { return answerSoundPath; }
			set { SetProperty( ref answerSoundPath, value ); }
		}

		public string CorrectSoundPath
		{
			get { return this.correctSoundPath; }
			set { SetProperty( ref this.correctSoundPath, value ); }
		}

		public string MissSoundPath
		{
			get { return this.missSoundPath; }
			set { SetProperty( ref this.missSoundPath, value ); }
		}

		public string CheckSoundPath
		{
			get { return this.checkSoundPath; }
			set { SetProperty( ref this.checkSoundPath, value ); }
		}
		
		private void SortVideoDirChanged()
		{
			if( !string.IsNullOrEmpty( this.SortVideoDir ) && Directory.Exists( this.SortVideoDir ) )
			{
				foreach( string path in Directory.EnumerateFiles( this.SortVideoDir, "*", SearchOption.AllDirectories ) )
				{
					if( !this.ChoiceOrderMediaList.Contains( path ) )
					{
						this.ChoiceOrderMediaList.Add( new ChoiceOrderMediaData( path ) );
					}
				}
			}
		}
	}
}
