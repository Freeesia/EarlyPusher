using StFrLibs.Core.Basis;

namespace EarlyPusher.Models
{
	public class SubjectData : ObservableObject
	{
		private string name;
		private string path;

		/// <summary>
		/// セット名
		/// </summary>
		public string Name
		{
			get { return this.name; }
			set { SetProperty( ref this.name, value ); }
		}

		/// <summary>
		/// 問題フォルダのパス
		/// </summary>
		public string Path
		{
			get { return this.path; }
			set { SetProperty( ref this.path, value ); }
		}
	}
}
