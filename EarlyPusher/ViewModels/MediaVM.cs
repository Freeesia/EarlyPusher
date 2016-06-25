using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StFrLibs.Core.Basis;
using StFrLibs.Core.Commands;

namespace EarlyPusher.ViewModels
{
	public class MediaVM : ViewModelBase
	{
		private string path;
		private MediaElement media;
		private bool isPlaying;

		public event EventHandler MediaPlayed;
		public event EventHandler MediaStoped;

		public MediaElement Media
		{
			get { return this.media; }
			set { SetProperty( ref this.media, value ); }
		}

		public string FilePath
		{
			get { return path; }
			set { SetProperty( ref path, value ); }
		}

		public string FileName
		{
			get { return Path.GetFileName( this.FilePath ); }
		}

		public bool IsPlaying
		{
			get { return this.isPlaying; }
			set { SetProperty( ref this.isPlaying, value ); }
		}

		public bool HasVideo
		{
			get { return this.Media.HasVideo; }
		}

		public DelegateCommand PlayCommand { get; }
		public DelegateCommand PlayOrPauseCommand { get; }
		public DelegateCommand StopCommand { get; }

		public MediaVM()
		{
			this.PlayCommand = new DelegateCommand( o => Play() );
			this.PlayOrPauseCommand = new DelegateCommand( PlayOrPause );
			this.StopCommand = new DelegateCommand( p => Stop() );

			this.Media = new MediaElement();
			this.Media.LoadedBehavior = MediaState.Manual;
			this.Media.UnloadedBehavior = MediaState.Manual;

			this.Media.MediaOpened += Media_MediaOpened;
			this.Media.MediaEnded += Media_MediaEnded;
		}

		public void LoadFile()
		{
			if( !string.IsNullOrEmpty( this.FilePath ) && File.Exists( this.FilePath ) )
			{
				this.Media.Source = new Uri( this.FilePath );
			}
			else
			{
				this.Media.Source = null;
			}
		}

		private void Media_MediaOpened( object sender, RoutedEventArgs e )
		{
			NotifyPropertyChanged( () => this.HasVideo );
		}

		private void Media_MediaEnded( object sender, RoutedEventArgs e )
		{
			this.Stop();
		}

		private void PlayOrPause( object obj )
		{
			if( this.IsPlaying )
			{
				this.Pause();
			}
			else
			{
				this.Play();
			}
		}

		public void Play()
		{
			if( this.IsPlaying )
			{
				this.Stop();
			}

			this.IsPlaying = true;
			this.Media.Play();
			OnMediaPlayed();

		}

		public void Pause()
		{
			if( this.IsPlaying )
			{
				this.Media.Pause();
				this.IsPlaying = false;
			}
		}

		public void Stop()
		{
			this.Media.Stop();
			this.IsPlaying = false;
			OnMediaStoped();
		}

		private void OnMediaPlayed()
		{
			this.MediaPlayed?.Invoke( this, EventArgs.Empty );
		}

		private void OnMediaStoped()
		{
			var d = this.MediaStoped;
			if( d != null )
			{
				d( this, EventArgs.Empty );
			}
		}
	}
}
