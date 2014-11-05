using EarlyPusher.Models;
using SlimDX.DirectInput;
using StFrLibs.Core.Basis;
using StFrLibs.Core.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace EarlyPusher
{
	public class VM : ViewModelBase
	{
		private SettingData data;
		private DirectInput input;

		private ObservableFixKeyedCollection<Guid, Device> devices = new ObservableFixKeyedCollection<Guid, Device>( d => d.Information.InstanceGuid );
		private object devicesLock = new object();

		private Timer inputLoop;

		private double fps = 0.0;

		private Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

		public DelegateCommand SearchCommand { get; private set; }

		private StringBuilder logBuilder;

		public string Log
		{
			get { return this.logBuilder.ToString(); }
		}

		public ObservableFixKeyedCollection<Guid, Device> Devices
		{
			get { return this.devices; }
		}

		public string FPS
		{
			get { return fps.ToString( "f2" ); }
		}

		private long updateTime;

		public long UpdateTime
		{
			get { return updateTime; }
			set { SetProperty( ref updateTime, value ); }
		}


		public VM()
		{
			this.SearchCommand = new DelegateCommand( SearchDevice, null );
			this.logBuilder = new StringBuilder();

			this.input = new DirectInput();
			this.inputLoop = new Timer( GetInput, null, 0, 2 );
		}

		private void LoadData()
		{
			if( File.Exists( SettingData.FileName ) )
			{
				using( FileStream file = new FileStream( SettingData.FileName, FileMode.Open ) )
				{
					XmlSerializer xml = new XmlSerializer( typeof( SettingData ) );
					this.data = xml.Deserialize( file ) as SettingData;
				}
			}
			else
			{
				this.data = new SettingData();
			}
		}

		/// <summary>
		/// 設定を保存します。
		/// </summary>
		public void SaveData()
		{
			using( Stream file = new FileStream( SettingData.FileName, FileMode.Create ) )
			{
				XmlSerializer xml = new XmlSerializer( typeof( SettingData ) );
				xml.Serialize( file, this.data );
			}
		}

		private void SearchDevice( object arg )
		{
			foreach( DeviceInstance di in input.GetDevices() )
			{
				WriteLogLine( "IGuid : " + di.InstanceGuid + ", PGuid : " + di.ProductGuid + ", Type : " + di.Type + ", PName : " + di.ProductName );

				Device device = null;
				if( !this.devices.Contains( di.InstanceGuid ) )
				{
					switch( di.Type )
					{
						case DeviceType.Gamepad:
							device = new Joystick( input, di.InstanceGuid );
							break;
						case DeviceType.Joystick:
							device = new Joystick( input, di.InstanceGuid );
							break;
						case DeviceType.Keyboard:
							device = new Keyboard( input );
							break;
						default:
							break;
					}
				}

				if( device != null )
				{
					device.Acquire();
					lock( this.devicesLock )
					{
						devices.Add( device );
					}
				}
			}
		}

		private void GetInput( object obj )
		{
			var timer = Stopwatch.StartNew();

			StringBuilder btn = new StringBuilder();

			lock( this.devicesLock )
			{
				foreach( Joystick joy in this.devices.OfType<Joystick>() )
				{
					JoystickState state = new JoystickState();

					if( joy.Poll().IsFailure )
					{
						joy.Unacquire();
						joy.Dispose();
						WriteLogLine( "joy.Poll().IsFailure" );
						continue;
					}

					if( joy.GetCurrentState( ref state ).IsFailure )
					{
						WriteLogLine( "joy.GetCurrentState( ref state ).IsFailure" );
						continue;
					}

					bool[] buttons = state.GetButtons();
					for( int i = 0; i < buttons.Length; i++ )
					{
						if( buttons[i] )
						{
							btn.Append( i + " " );
						}
					}
				}

				this.DispatcherInvoke( () => 
				{
					var dev = this.devices.FirstOrDefault( d => d.Disposed );
					while( dev != null )
					{
						this.devices.Remove( dev );
						dev = this.devices.FirstOrDefault( d => d.Disposed );
					}
				} );


				foreach( Keyboard key in this.devices.OfType<Keyboard>() )
				{
					KeyboardState state = new KeyboardState();

					if( key.Poll().IsFailure )
					{
						continue;
					}

					if( key.GetCurrentState( ref state ).IsFailure )
					{
						continue;
					}

					foreach( var item in state.PressedKeys )
					{
						btn.Append( item.ToString() + " " );
					}
				}
			}

			if( btn.Length > 0 )
			{
				WriteLogLine( btn.ToString() );
			}

			timer.Stop();
			this.UpdateTime = timer.ElapsedMilliseconds;

		}

		private void WriteLogLine( string str )
		{
			this.logBuilder.AppendLine( str );
			this.NotifyPropertyChanged( "Log" );
		}

		public void DispatcherInvoke( Action action )
		{
			if( !this.dispatcher.CheckAccess() )
			{
				this.dispatcher.BeginInvoke( action );
			}
			else
			{
				action();
			}
		}
	}
}
