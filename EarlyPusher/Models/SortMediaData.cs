using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StFrLibs.Core.Basis;

namespace EarlyPusher.Models
{
	public class SortMediaData : ObservableObject
	{
		private List<Choice> sortedList = new List<Choice>( 4 );
		private string mediaPath;
		private string choiceAImagePath;
		private string choiceBImagePath;
		private string choiceCImagePath;
		private string choiceDImagePath;

		public List<Choice> SortedList
		{
			get { return this.sortedList; }
		}

		public string MediaPath
		{
			get { return this.mediaPath; }
			set { SetProperty( ref this.mediaPath, value ); }
		}
		
		public string ChoiceAImagePath
		{
			get { return this.choiceAImagePath; }
			set { SetProperty( ref this.choiceAImagePath, value ); }
		}

		public string ChoiceBImagePath
		{
			get { return this.choiceBImagePath; }
			set { SetProperty( ref this.choiceBImagePath, value ); }
		}

		public string ChoiceCImagePath
		{
			get { return this.choiceCImagePath; }
			set { SetProperty( ref this.choiceCImagePath, value ); }
		}

		public string ChoiceDImagePath
		{
			get { return this.choiceDImagePath; }
			set { SetProperty( ref this.choiceDImagePath, value ); }
		}
	}
}
