using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StFrLibs.Core.Basis;

namespace EarlyPusher.ViewModels
{
	public class VideoVM : ViewModelBase
	{
		private string path;

		public string FilePath
		{
			get { return path; }
			set { SetProperty( ref path, value ); }
		}

		public string FileName
		{
			get { return Path.GetFileName( path ); }
		}

	}
}
