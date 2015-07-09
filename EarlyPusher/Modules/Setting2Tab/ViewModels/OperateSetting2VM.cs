using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EarlyPusher.Models;
using EarlyPusher.ViewModels;
using StFrLibs.Core.Basis;

namespace EarlyPusher.Modules.Setting2Tab.ViewModels
{
	public class OperateSetting2VM : OperateTabVMBase
	{
		private ObservableHashVMCollection<MediaSetting2VM> medias = new ObservableHashVMCollection<MediaSetting2VM>();
		private MediaSetting2VM selectedMedia;

		/// <summary>
		/// メディアのリスト
		/// </summary>
		public ObservableHashVMCollection<MediaSetting2VM> Medias
		{
			get
			{
				return this.medias;
			}
		}

		/// <summary>
		/// 選択しているメディア
		/// </summary>
		public MediaSetting2VM SelectedMedia
		{
			get { return this.selectedMedia; }
			set { SetProperty( ref this.selectedMedia, value ); }
		}

		public OperateSetting2VM( MainVM parent )
			: base( parent )
		{

		}

		public override void LoadData()
		{
			if( !string.IsNullOrEmpty( this.Parent.Data.SortVideoDir ) && Directory.Exists( this.Parent.Data.SortVideoDir ) )
			{
				this.Medias.Clear();
				foreach( string path in Directory.EnumerateFiles( this.Parent.Data.SortVideoDir, "*", SearchOption.AllDirectories ) )
				{
					if( !this.Parent.Data.ChoiceOrderMediaList.Contains( path ) )
					{
						this.Parent.Data.ChoiceOrderMediaList.Add( new ChoiceOrderMediaData( path ) );
					}

					var media = new MediaSetting2VM( this.Parent.Data.ChoiceOrderMediaList[path] );
					this.Medias.Add( media );
				}
			}
		}

		public override void SaveData()
		{
			this.Parent.Data.ChoiceOrderMediaList.Clear();
			foreach( var media in this.Medias )
			{
				this.Parent.Data.ChoiceOrderMediaList.Add( media.Model );
			}
		}
	}
}
