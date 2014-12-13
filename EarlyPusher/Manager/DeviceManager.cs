using EarlyPusher.Models;
using SlimDX.DirectInput;
using StFrLibs.Core.Basis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using EarlyPusher.Extensions;

namespace EarlyPusher.Manager
{
	public class DeviceManager : ObservableObject, IDisposable
	{
		private ObservableFixKeyedCollection<Guid, Device> devices = new ObservableFixKeyedCollection<Guid, Device>( d => d.Information.InstanceGuid );
		private DirectInput input;
		private object devicesLock = new object();
		private Timer inputLoop;

		private HashSet<Tuple<Guid, int>> pushingKeys;

		private Dispatcher dispatcher;

		private long updateTime;

		#region プロパティ

		public long UpdateTime
		{
			get { return updateTime; }
			set { SetProperty( ref updateTime, value ); }
		}

		public ObservableFixKeyedCollection<Guid, Device> Devices
		{
			get { return this.devices; }
		}

		#endregion

		#region イベント

		public EventHandler<DeviceKeyEventArgs> KeyPushed;

		public EventHandler<DeviceKeyEventArgs> KeyReleased;

		#endregion

		public DeviceManager()
		{
			this.pushingKeys = new HashSet<Tuple<Guid, int>>();
			this.dispatcher = Dispatcher.CurrentDispatcher;

			this.input = new DirectInput();
			this.inputLoop = new Timer( GetInput, null, 0, 10 );
		}

		public void SearchDevice()
		{
			foreach( DeviceInstance di in input.GetDevices() )
			{
				Debug.WriteLine( "IGuid : " + di.InstanceGuid + ", PGuid : " + di.ProductGuid + ", Type : " + di.Type + ", PName : " + di.ProductName );

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
							//device = new Keyboard( input );
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

			lock( this.devicesLock )
			{
				var newPush = new List<Tuple<Guid, int>>();

				foreach( Device d in this.devices )
				{

					if( d.Poll().IsFailure )
					{
						d.Unacquire();
						d.Dispose();
						Debug.WriteLine( "joy.Poll().IsFailure" );
						continue;
					}

					if( d is Joystick )
					{
						var joy = d as Joystick;
						JoystickState state = new JoystickState();

						if( joy.GetCurrentState( ref state ).IsFailure )
						{
							Debug.WriteLine( "joy.GetCurrentState( ref state ).IsFailure" );
							continue;
						}
						bool[] buttons = state.GetButtons();
						for( int i = 0; i < buttons.Length; i++ )
						{
							Tuple<Guid,int> key = new Tuple<Guid,int>(joy.Information.InstanceGuid, i+1);
							if( buttons[i] && !this.pushingKeys.Contains( key ) )
							{
								newPush.Add(key);
							}
							else if( !buttons[i] && this.pushingKeys.Contains( key ) )
							{
								this.pushingKeys.Remove( key );
								Released( key );
							}
						}
					}
					else if( d is Keyboard )
					{
						var board = d as Keyboard;
						KeyboardState state = new KeyboardState();

						if( board.GetCurrentState( ref state ).IsFailure )
						{
							continue;
						}

						foreach( var item in state.AllKeys )
						{
							Tuple<Guid, int> key = new Tuple<Guid, int>( board.Information.InstanceGuid, ( int )item );
							if( state.IsPressed(item) && !this.pushingKeys.Contains( key ) )
							{
								newPush.Add(key);
							}
							else if( state.IsReleased( item ) && this.pushingKeys.Contains( key ) )
							{
								this.pushingKeys.Remove( key );
								Released( key );
							}
						}
					}

					newPush.Shuffle();
					if (newPush.Count > 0)
					{
						Debug.WriteLine("同時押し数 : " + newPush.Count);
					}
					foreach (var key in newPush)
					{
						this.pushingKeys.Add(key);
						Pushed(key);
					}

				}

				var dev = this.devices.FirstOrDefault( d => d.Disposed );
				while( dev != null )
				{
					this.devices.Remove( dev );
					dev = this.devices.FirstOrDefault( d => d.Disposed );
				}
			}

			timer.Stop();
			this.UpdateTime = timer.ElapsedTicks;
		}

		private void Pushed( Tuple<Guid, int> key )
		{
			var d = KeyPushed;
			if( d != null )
			{
				this.dispatcher.BeginInvoke( d, this, new DeviceKeyEventArgs( key.Item1, key.Item2 ) );
			}
		}

		private void Released( Tuple<Guid, int> key )
		{
			var d = KeyReleased;
			if( d != null )
			{
				this.dispatcher.BeginInvoke( d, this, new DeviceKeyEventArgs( key.Item1, key.Item2 ) );
			}
		}

		public void Dispose()
		{
			this.inputLoop.Dispose();
			lock( this.devicesLock )
			{
				var d = this.Devices.FirstOrDefault();
				while( d != null )
				{
					d.Unacquire();
					d.Dispose();
					this.Devices.Remove( d );
				}
			}
			this.pushingKeys.Clear();
			this.input.Dispose();
		}
	}
}
