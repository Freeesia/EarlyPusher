using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StFrLibs.Core.Basis;

namespace EarlyPusher.Models
{
	[Serializable]
	public class EarlyData : ObservableObject
	{
		private string correctPath;
		private string incorrectPath;
		private string pushPath;

		public ObservableCollection<SetData> Sets { get; } = new ObservableCollection<SetData>();

		/// <summary>
		/// プッシュ音
		/// </summary>
		public string PushPath
		{
			get { return this.pushPath; }
			set { SetProperty( ref this.pushPath, value ); }
		}

		/// <summary>
		/// 正解音
		/// </summary>
		public string CorrectPath
		{
			get { return this.correctPath; }
			set { SetProperty( ref this.correctPath, value ); }
		}

		/// <summary>
		/// 不正解音
		/// </summary>
		public string IncorrectPath
		{
			get { return this.incorrectPath; }
			set { SetProperty( ref this.incorrectPath, value ); }
		}
	}
}
