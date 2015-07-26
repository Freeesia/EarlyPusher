using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EarlyPusher.Models;
using EarlyPusher.ViewModels;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using StFrLibs.Core.Basis;
using StFrLibs.Core.Commands;

namespace EarlyPusher.Modules.Setting2Tab.ViewModels
{
	public class OperateSetting2VM : OperateTabVMBase
	{
		private ObservableHashVMCollection<MediaSetting2VM> medias = new ObservableHashVMCollection<MediaSetting2VM>();
		private MediaSetting2VM selectedMedia;
		private string selectedCamera;

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

		public string SelectedCamera
		{
			get { return this.selectedCamera; }
			set { SetProperty( ref this.selectedCamera, value, SelectedCameraChanged ); }
		}

		public DelegateCommand SelectTimerImagePathCommand { get; private set; }
		public DelegateCommand SelectCorrectImagePathCommand { get; private set; }
		public DelegateCommand SelectMaskImagePathCommand { get; private set; }
		
		public OperateSetting2VM( MainVM parent )
			: base( parent )
		{
			this.SelectTimerImagePathCommand = new DelegateCommand( SelectTimerImage );
			this.SelectCorrectImagePathCommand = new DelegateCommand( SelectCorrectImage );
			this.SelectMaskImagePathCommand = new DelegateCommand( SelectMaskImage );
		}

		private void SelectTimerImage( object obj )
		{
			VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
			if( string.IsNullOrEmpty( this.Parent.Data.TimerImagePath ) )
			{
				dlg.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;
			}
			else
			{
				dlg.SelectedPath = this.Parent.Data.TimerImagePath;
			}
			if( dlg.ShowDialog() == true )
			{
				this.Parent.Data.TimerImagePath = dlg.SelectedPath;
			}
		}

		private void SelectCorrectImage( object obj )
		{
			VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
			if( string.IsNullOrEmpty( this.Parent.Data.CorrectImagePath ) )
			{
				dlg.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;
			}
			else
			{
				dlg.SelectedPath = this.Parent.Data.CorrectImagePath;
			}
			if( dlg.ShowDialog() == true )
			{
				this.Parent.Data.CorrectImagePath = dlg.SelectedPath;
			}
		}

		private void SelectMaskImage( object obj )
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
			if( dlg.ShowDialog() == true )
			{
				this.Parent.Data.MaskImagePath = dlg.FileName;
			}
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

			this.SelectedCamera = this.Parent.Data.CameraDevice;
		}

		public override void SaveData()
		{
			this.Parent.Data.ChoiceOrderMediaList.Clear();
			foreach( var media in this.Medias )
			{
				this.Parent.Data.ChoiceOrderMediaList.Add( media.Model );
			}
		}

		private void SelectedCameraChanged()
		{
			this.Parent.Data.CameraDevice = this.SelectedCamera;
		}
	}
}
