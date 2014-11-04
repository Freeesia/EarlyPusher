using SlimDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace EarlyPusher
{
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window
	{
		private DirectInput input;

		private List<Device> devices = new List<Device>();

		private Timer inputLoop;

		public MainWindow()
		{
			InitializeComponent();

			this.input = new DirectInput();
			this.inputLoop = new Timer( GetInput, null, 0, 10 );
		}

		private void GetInput( object obj )
		{
			foreach( Joystick joy in this.devices.OfType<Joystick>() )
			{
				JoystickState state = new JoystickState();

				if( joy.Poll().IsFailure )
				{
					continue;
				}

				if( joy.GetCurrentState( ref state ).IsFailure )
				{
					continue;
				}

				StringBuilder btn = new StringBuilder();
				bool[] buttons = state.GetButtons();
				for( int i = 0; i < buttons.Length; i++ )
				{
					if( buttons[i] )
					{
						btn.Append( i + " " );
					}
				}
				if( btn.Length > 0 )
				{
					LogWrite( btn.ToString() );
				}
			}

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

				StringBuilder btn = new StringBuilder();
				foreach( var item in state.PressedKeys )
				{
					btn.Append( item.ToString() + " " );
				}
				if( btn.Length > 0 )
				{
					LogWrite( btn.ToString() );
				}
			}

		}

		private void Button_Click( object sender, RoutedEventArgs e )
		{
			foreach( DeviceInstance di in input.GetDevices() )
			{
				LogWrite( "Type : " + di.Type + ", Guid : " + di.InstanceGuid + ", Name : " + di.InstanceName );

				Device device = null;
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

				if( device != null )
				{
					device.Acquire();
					devices.Add( device );
				}
			}
		}

		private void LogWrite( string str )
		{
			this.Dispatcher.BeginInvoke( new Action( () =>
			{
				this.log.Text += str + Environment.NewLine;
			} ) );
		}
	}
}
