using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using EarlyPusher.Modules.TimeShockTab.Views;
using EarlyPusher.ViewModels;
using StFrLibs.Core.Commands;
using StFrLibs.Core.Extensions;

namespace EarlyPusher.Modules.TimeShockTab.ViewModels
{
	public class OperateTimeShockVM : OperateTabVMBase
	{
		private ObservableCollection<ImageItemViewModel> correctImageItems = new ObservableCollection<ImageItemViewModel>();
		private ObservableCollection<ImageItemViewModel> timerImageItems = new ObservableCollection<ImageItemViewModel>();
		private BitmapImage maskImage;
		private BitmapImage backImage;
		private MediaVM bgm = new MediaVM();
		private MediaVM correctSound = new MediaVM();
		private int time = 0;
		private Timer timer;

		public ObservableCollection<ImageItemViewModel> CorrectImageItems
		{
			get { return this.correctImageItems; }
		}

		public ObservableCollection<ImageItemViewModel> TimerImageItems
		{
			get { return this.timerImageItems; }
		}

		public BitmapImage MaskImage
		{
			get { return this.maskImage; }
			set { SetProperty( ref this.maskImage, value ); }
		}

		public BitmapImage BackImage
		{
			get { return this.backImage; }
			set { SetProperty( ref this.backImage, value ); }
		}

		public DelegateCommand StartCommand { get; private set; }
		public DelegateCommand StopCommand { get; private set; }

		public int Time
		{
			get { return this.time; }
			set { SetProperty( ref this.time, value ); }
		}

		public OperateTimeShockVM( MainVM parent )
			: base( parent )
		{
			this.View = new OperateTimeShockView();
			this.Header = "タイムショック";

			this.PlayView = new PlayTimeShockView();

			this.StartCommand = new DelegateCommand( Start, CanStart );
			this.StopCommand = new DelegateCommand( Stop );

			this.CorrectImageItems.CollectionChanged += CorrectImageItems_CollectionChanged;
		}

		private void CorrectImageItems_CollectionChanged( object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e )
		{
			if( e.OldItems != null )
			{
				foreach( ImageItemViewModel item in e.OldItems )
				{
					item.PropertyChanged -= CorrectItem_PropertyChanged;
				}
			}

			if( e.NewItems != null )
			{
				foreach( ImageItemViewModel item in e.NewItems )
				{
					item.PropertyChanged += CorrectItem_PropertyChanged;
				}
			}
		}

		private void CorrectItem_PropertyChanged( object sender, System.ComponentModel.PropertyChangedEventArgs e )
		{
			var item = sender as ImageItemViewModel;
			if( e.PropertyName == "IsVisible" && item.IsVisible )
			{
				this.correctSound.Play();
			}
		}

		private bool CanStart( object obj )
		{
			return this.CorrectImageItems.Any() && this.TimerImageItems.Any();
		}

		private void Start( object obj )
		{
			if( this.timer == null )
			{
				this.timer = new Timer( IncrementTime, null, 0, 1000 );
				this.bgm.Play();
			}
		}

		private void Stop( object obj )
		{
			if( this.timer != null )
			{
				this.timer.Dispose();
				this.timer = null;
			}

			this.bgm.Stop();
			this.TimerImageItems.ForEach( i => i.IsVisible = false );
			this.CorrectImageItems.ForEach( i => i.IsVisible = false );
		}

		private void IncrementTime( object state )
		{
			this.TimerImageItems[this.Time].IsVisible = true;

			this.Time++;
			if( this.Time >= this.TimerImageItems.Count )
			{
				this.timer.Dispose();
				this.timer = null;
				this.bgm.Media.Dispatcher.BeginInvoke( new Action( () => this.bgm.Stop() ) );
			}
		}

		public override void LoadData()
		{
			this.Parent.Data.PropertyChanged -= Data_PropertyChanged;

			if( !string.IsNullOrEmpty( this.Parent.Data.CorrectImagePath ) && Directory.Exists( this.Parent.Data.CorrectImagePath ) )
			{
				this.CorrectImageItems.Clear();
				foreach( string path in Directory.EnumerateFiles( this.Parent.Data.CorrectImagePath, "*", SearchOption.AllDirectories ) )
				{
					this.CorrectImageItems.Add( new ImageItemViewModel( path ) );
				}
			}

			if( !string.IsNullOrEmpty( this.Parent.Data.TimerImagePath ) && Directory.Exists( this.Parent.Data.TimerImagePath ) )
			{
				this.TimerImageItems.Clear();
				foreach( string path in Directory.EnumerateFiles( this.Parent.Data.TimerImagePath, "*", SearchOption.AllDirectories ) )
				{
					this.TimerImageItems.Add( new ImageItemViewModel( path ) );
				}
			}

			if( !string.IsNullOrEmpty( this.Parent.Data.MaskImagePath ) && File.Exists( this.Parent.Data.MaskImagePath ) )
			{
				this.MaskImage = new BitmapImage( new Uri( this.Parent.Data.MaskImagePath ) );
			}

			if( !string.IsNullOrEmpty( this.Parent.Data.BackImagePath ) && File.Exists( this.Parent.Data.BackImagePath ) )
			{
				this.BackImage = new BitmapImage( new Uri( this.Parent.Data.BackImagePath ) );
			}

			if( !string.IsNullOrEmpty( this.Parent.Data.TimeshockBgmPath ) && File.Exists( this.Parent.Data.TimeshockBgmPath ) )
			{
				this.bgm.FilePath = this.Parent.Data.TimeshockBgmPath;
				this.bgm.LoadFile();
			}

			if( !string.IsNullOrEmpty( this.Parent.Data.TimeshockCorrectSoundPath ) && File.Exists( this.Parent.Data.TimeshockCorrectSoundPath ) )
			{
				this.correctSound.FilePath = this.Parent.Data.TimeshockCorrectSoundPath;
				this.correctSound.LoadFile();
			}

			this.Parent.Data.PropertyChanged += Data_PropertyChanged;

			this.StartCommand.RaiseCanExecuteChanged();
		}

		private void Data_PropertyChanged( object sender, System.ComponentModel.PropertyChangedEventArgs e )
		{
			if( e.PropertyName == "CorrectImagePath" ||
				e.PropertyName == "TimerImagePath" ||
				e.PropertyName == "BackImagePath" ||
				e.PropertyName == "MaskImagePath" )
			{
				LoadData();
			}
		}
	}
}
