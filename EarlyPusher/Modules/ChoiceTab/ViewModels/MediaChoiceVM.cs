using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EarlyPusher.ViewModels;
using StFrLibs.Core.Commands;

namespace EarlyPusher.Modules.ChoiceTab.ViewModels
{
	public class MediaChoiceVM : MediaVM
	{
		private string qFilePath;
		private string aFilePath;

		public DelegateCommand PlayAnswerCommand { get; private set; }
		public DelegateCommand PlayQuestionCommand { get; private set; }

		public MediaChoiceVM( string dir )
		{
			foreach( var path in Directory.EnumerateFiles( dir ) )
			{
				if( Path.GetFileNameWithoutExtension( path ).Contains( "question" ) )
				{
					this.qFilePath = path;
				}
				else if( Path.GetFileNameWithoutExtension( path ).Contains( "answer" ) )
				{
					this.aFilePath = path;
				}
			}

			this.PlayAnswerCommand = new DelegateCommand( PlayAnswer );
			this.PlayQuestionCommand = new DelegateCommand( PlayQuestion );
		}

		private void PlayAnswer( object obj )
		{
			Stop();
			this.FilePath = this.aFilePath;
			LoadFile();
			Play();
		}

		private void PlayQuestion( object obj )
		{
			Stop();
			this.FilePath = this.qFilePath;
			LoadFile();
			Play();
		}
	}
}
