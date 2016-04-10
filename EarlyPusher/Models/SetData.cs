using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StFrLibs.Core.Basis;

namespace EarlyPusher.Models
{
	public class SetData : ObservableObject
	{
		private string name;
		private string path;
		private int basePoint;

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

		/// <summary>
		/// 基準ポイント
		/// </summary>
		public int BasePoint
		{
			get { return this.basePoint; }
			set { SetProperty( ref this.basePoint, value ); }
		}
	}
}
