using System;
using System.Windows;
using System.Windows.Controls;

namespace SFLibs.UI.Behaviors
{
	public static class LogBehavior
	{
		public static string GetLog( DependencyObject obj )
		{
			return ( string )obj.GetValue( LogProperty );
		}

		public static void SetLog( DependencyObject obj, string value )
		{
			obj.SetValue( LogProperty, value );
		}

		// Using a DependencyProperty as the backing store for Log.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty LogProperty =
			DependencyProperty.RegisterAttached( "Log", typeof( string ), typeof( LogBehavior ), new PropertyMetadata( string.Empty, OnLogChanged ) );

		public static bool GetIsScrollEnd( DependencyObject obj )
		{
			return ( bool )obj.GetValue( IsScrollEndProperty );
		}

		public static void SetIsScrollEnd( DependencyObject obj, bool value )
		{
			obj.SetValue( IsScrollEndProperty, value );
		}

		// Using a DependencyProperty as the backing store for IsScrollEnd.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsScrollEndProperty =
			DependencyProperty.RegisterAttached( "IsScrollEnd", typeof( bool ), typeof( LogBehavior ), new PropertyMetadata( false, OnIsScrollEndChanged ) );

		private static void OnIsScrollEndChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			var text = d as TextBox;
			if( text != null )
			{
				if( ( bool )e.NewValue )
				{
					text.IsReadOnlyCaretVisible = true;
				}
				else
				{
					text.IsReadOnlyCaretVisible = false;
				}
			}
		}

		private static void OnLogChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			var log = e.NewValue as string;
			var text = d as TextBox;
			if( !string.IsNullOrEmpty( log ) )
			{
				text.AppendText( log );
				text.AppendText( Environment.NewLine );
				if( GetIsScrollEnd( d ) )
				{
					//var oldFocusedElement = FocusManager.GetFocusedElement( text );
					//text.Focus();
					text.CaretIndex = text.Text.Length;
					text.ScrollToEnd();
					//FocusManager.SetFocusedElement( text, oldFocusedElement );
				}
			}


		}

	}
}
