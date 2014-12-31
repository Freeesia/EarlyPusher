using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using StFrLibs.Core.Basis;
using StFrLibs.Core.Commands;

namespace EarlyPusher.ViewModels
{
	public class SoundVM : ViewModelBase, IDisposable
	{
		private MediaPlayer sound;
		private bool isPlaying = false;
		private bool isPause = false;
		private string path;

		public MediaPlayer Sound
		{
			get { return sound; }
		}

		public string Path
		{
			get { return path; }
			set { SetProperty( ref this.path, value, PathSetted ); }
		}

		public DelegateCommand PlayCommand { get; private set; }
		public DelegateCommand PauseCommand { get; private set; }
		public DelegateCommand StopCommand { get; private set; }

		public SoundVM()
		{
			this.PlayCommand = new DelegateCommand( Play, CanPlay );
			this.PauseCommand = new DelegateCommand( Pause, CanPause );
			this.StopCommand = new DelegateCommand( Stop, CanStop );
		}

		private void PathSetted( bool obj )
		{
			if( obj )
			{
				if( File.Exists( this.Path ) )
				{
					this.sound = OpenSound( this.Path );
					this.PlayCommand.RaiseCanExecuteChanged();
				}
				else if( this.sound != null )
				{
					this.sound.Stop();
					this.sound.Close();
					this.sound = null;
				}
			}
		}

		private MediaPlayer OpenSound( string path )
		{
			var player = new MediaPlayer();
			player.Open( new Uri( path ) );
			player.MediaEnded += player_MediaEnded;
			return player;
		}

		private void UpdateCommand()
		{
			this.PlayCommand.RaiseCanExecuteChanged();
			this.PauseCommand.RaiseCanExecuteChanged();
			this.StopCommand.RaiseCanExecuteChanged();
		}

		#region コマンド処理

		private bool CanPlay( object obj )
		{
			return this.Sound != null;
		}

		private void Play( object obj )
		{
			if( this.isPlaying )
			{
				this.Sound.Stop();
			}
			this.Sound.Play();
			this.isPlaying = true;
			UpdateCommand();
		}

		private bool CanPause( object obj )
		{
			return this.Sound != null && isPlaying;
		}

		private void Pause( object obj )
		{
			if( !this.isPause )
			{
				this.Sound.Pause();
			}
			else
			{
				this.Sound.Play();
			}
			this.isPause = !this.isPause;
			UpdateCommand();
		}

		private bool CanStop( object obj )
		{
			return this.Sound != null && isPlaying;
		}

		private void Stop( object obj )
		{
			this.Sound.Stop();
			this.isPlaying = false;
			UpdateCommand();
		}

		#endregion

		private void player_MediaEnded( object sender, EventArgs e )
		{
			this.Stop( null );
		}

		public void Dispose()
		{
			if( this.sound != null )
			{
				this.sound.Stop();
				this.sound.Close();
			}
		}
	}
}
