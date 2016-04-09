using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using EarlyPusher.Models;
using EarlyPusher.Modules.Setting2Tab.Views;
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
		private string timerImagePath;
		private string correctImagePath;
		private string maskImagePath;
		private string backImagePath;
		private string cameraDevice;
		private  string bgmPath;
		private  string correctSoundPath;

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

		public string TimerImagePath
		{
			get { return this.timerImagePath; }
			set { SetProperty( ref this.timerImagePath, value ); }
		}

		public string CorrectImagePath
		{
			get { return this.correctImagePath; }
			set { SetProperty( ref this.correctImagePath, value ); }
		}

		public string MaskImagePath
		{
			get { return this.maskImagePath; }
			set { SetProperty( ref this.maskImagePath, value ); }
		}

		public string BackImagePath
		{
			get { return this.backImagePath; }
			set { SetProperty( ref this.backImagePath, value ); }
		}

		public string CameraDevice
		{
			get { return this.cameraDevice; }
			set { SetProperty( ref this.cameraDevice, value ); }
		}

		public string BgmPath
		{
			get { return this.bgmPath; }
			set { SetProperty( ref this.bgmPath, value ); }
		}

		public string CorrectSoundPath
		{
			get { return this.correctSoundPath; }
			set { SetProperty( ref this.correctSoundPath, value ); }
		}

		public DelegateCommand SelectTimerImagePathCommand { get; private set; }
		public DelegateCommand SelectCorrectImagePathCommand { get; private set; }
		public DelegateCommand SelectMaskImagePathCommand { get; private set; }
		public DelegateCommand SelectBackImagePathCommand { get; private set; }
		public DelegateCommand SelectBgmPathCommand { get; private set; }
		public DelegateCommand SelectCorrectSoundPathCommand { get; private set; }

		public OperateSetting2VM( MainVM parent )
			: base( parent )
		{
			this.View = new OperateSetting2View();
			this.Header = "設定2";

			this.SelectTimerImagePathCommand = new DelegateCommand( SelectTimerImage );
			this.SelectCorrectImagePathCommand = new DelegateCommand( SelectCorrectImage );
			this.SelectMaskImagePathCommand = new DelegateCommand( SelectMaskImage );
			this.SelectBackImagePathCommand = new DelegateCommand( SelectBackImage );
			this.SelectBgmPathCommand = new DelegateCommand( SelectBgmPath );
			this.SelectCorrectSoundPathCommand = new DelegateCommand( SelectCorrectSoundPath );
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

		private void SelectBackImage( object obj )
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
			if( dlg.ShowDialog() == true )
			{
				this.Parent.Data.BackImagePath = dlg.FileName;
			}
		}

		private void SelectBgmPath( object obj )
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
			if( dlg.ShowDialog() == true )
			{
				this.Parent.Data.TimeshockBgmPath = dlg.FileName;
			}
		}

		private void SelectCorrectSoundPath( object obj )
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
			if( dlg.ShowDialog() == true )
			{
				this.Parent.Data.TimeshockCorrectSoundPath = dlg.FileName;
			}
		}

		public override void LoadData()
		{
			this.Parent.Data.PropertyChanged -= Data_PropertyChanged;

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

				this.TimerImagePath = this.Parent.Data.TimerImagePath;
				this.CorrectImagePath = this.Parent.Data.CorrectImagePath;
				this.MaskImagePath = this.Parent.Data.MaskImagePath;
				this.BackImagePath = this.Parent.Data.BackImagePath;
				this.CameraDevice = this.Parent.Data.CameraDevice;
				this.BgmPath = this.Parent.Data.TimeshockBgmPath;
				this.CorrectSoundPath = this.Parent.Data.TimeshockCorrectSoundPath;
			}

			this.Parent.Data.PropertyChanged += Data_PropertyChanged;
		}

		private void Data_PropertyChanged( object sender, System.ComponentModel.PropertyChangedEventArgs e )
		{
			LoadData();
		}

		public override void SaveData()
		{
			this.Parent.Data.CameraDevice = this.CameraDevice;
			this.Parent.Data.ChoiceOrderMediaList.Clear();
			foreach( var media in this.Medias )
			{
				this.Parent.Data.ChoiceOrderMediaList.Add( media.Model );
			}
		}
	}
}
