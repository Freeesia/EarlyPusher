using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EarlyPusher.Controls
{
	public class FixedWrapPanel : Panel
	{
		#region Public Properties

		private static bool IsWidthHeightValid( object value )
		{
			double v = (double)value;
			return ( Double.IsNaN( v ) ) || ( v >= 0.0d && !Double.IsPositiveInfinity( v ) );
		}

		/// <summary>
		/// DependencyProperty for <see cref="ItemWidth" /> property.
		/// </summary>
		public static readonly DependencyProperty ItemWidthProperty =
				DependencyProperty.Register(
						"ItemWidth",
						typeof( double ),
						typeof( FixedWrapPanel ),
						new FrameworkPropertyMetadata(
								Double.NaN,
								FrameworkPropertyMetadataOptions.AffectsMeasure ),
						new ValidateValueCallback( IsWidthHeightValid ) );

		/// <summary>
		/// アイテムの幅
		/// </summary>
		[TypeConverter( typeof( LengthConverter ) )]
		public double ItemWidth
		{
			get { return (double)GetValue( ItemWidthProperty ); }
			set { SetValue( ItemWidthProperty, value ); }
		}


		/// <summary>
		/// DependencyProperty for <see cref="ItemHeight" /> property.
		/// </summary>
		public static readonly DependencyProperty ItemHeightProperty =
				DependencyProperty.Register(
						"ItemHeight",
						typeof( double ),
						typeof( FixedWrapPanel ),
						new FrameworkPropertyMetadata(
								Double.NaN,
								FrameworkPropertyMetadataOptions.AffectsMeasure ),
						new ValidateValueCallback( IsWidthHeightValid ) );


		/// <summary>
		/// アイテムの高さ
		/// </summary>
		[TypeConverter( typeof( LengthConverter ) )]
		public double ItemHeight
		{
			get { return (double)GetValue( ItemHeightProperty ); }
			set { SetValue( ItemHeightProperty, value ); }
		}

		/// <summary>
		/// DependencyProperty for <see cref="Orientation" /> property.
		/// </summary>
		public static readonly DependencyProperty OrientationProperty =
				StackPanel.OrientationProperty.AddOwner(
						typeof( FixedWrapPanel ),
						new FrameworkPropertyMetadata(
								Orientation.Horizontal,
								FrameworkPropertyMetadataOptions.AffectsMeasure ) );

		/// <summary>
		/// 向き
		/// </summary>
		public Orientation Orientation
		{
			get { return (Orientation)GetValue( OrientationProperty ); }
			set { SetValue( OrientationProperty, value ); }
		}

		/// <summary>
		/// 改行する個数
		/// </summary>
		public int WrapCount
		{
			get { return (int)GetValue( WrapCountProperty ); }
			set { SetValue( WrapCountProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for WrapCount.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty WrapCountProperty = 
				DependencyProperty.Register(
						"WrapCount",
						typeof( int ),
						typeof( FixedWrapPanel ),
						new FrameworkPropertyMetadata(
							1,
							FrameworkPropertyMetadataOptions.AffectsMeasure ),
							new ValidateValueCallback( IsWrapCountValid ) );

		private static bool IsWrapCountValid( object value )
		{
			int v = (int)value;
			return v > 0;
		}


		#endregion

		#region Protected Methods

		private struct UVSize
		{
			internal UVSize( Orientation orientation, double width, double height )
			{
				U = V = 0d;
				_orientation = orientation;
				Width = width;
				Height = height;
			}

			internal UVSize( Orientation orientation )
			{
				U = V = 0d;
				_orientation = orientation;
			}

			internal double U;
			internal double V;
			private Orientation _orientation;

			internal double Width
			{
				get { return ( _orientation == Orientation.Horizontal ? U : V ); }
				set { if( _orientation == Orientation.Horizontal ) U = value; else V = value; }
			}
			internal double Height
			{
				get { return ( _orientation == Orientation.Horizontal ? V : U ); }
				set { if( _orientation == Orientation.Horizontal ) V = value; else U = value; }
			}
		}


		/// <summary>
		/// <see cref="FrameworkElement.MeasureOverride"/>
		/// </summary>
		protected override Size MeasureOverride( Size constraint )
		{
			UVSize curLineSize = new UVSize( Orientation );
			UVSize panelSize = new UVSize( Orientation );
			UVSize uvConstraint = new UVSize( Orientation, constraint.Width, constraint.Height );
			double itemWidth = ItemWidth;
			double itemHeight = ItemHeight;
			bool itemWidthSet = !Double.IsNaN( itemWidth );
			bool itemHeightSet = !Double.IsNaN( itemHeight );
			int lineCount = 0;

			Size childConstraint = new Size(
				( itemWidthSet ? itemWidth : constraint.Width ),
				( itemHeightSet ? itemHeight : constraint.Height ) );

			foreach( UIElement child in InternalChildren )
			{
				if( child == null ) continue;

				//Flow passes its own constrint to children
				child.Measure( childConstraint );

				//this is the size of the child in UV space
				UVSize sz = new UVSize(
					Orientation,
					( itemWidthSet ? itemWidth : child.DesiredSize.Width ),
					( itemHeightSet ? itemHeight : child.DesiredSize.Height ) );

				if( lineCount < WrapCount ) //同じ行に追加
				{
					curLineSize.U += sz.U;
					curLineSize.V = Math.Max( sz.V, curLineSize.V );
				}
				else //新しい行に移行
				{
					lineCount = 0;

					panelSize.U = Math.Max( curLineSize.U, panelSize.U );
					panelSize.V += curLineSize.V;
					curLineSize = sz;
				}

				lineCount++;
			}

			//the last line size, if any should be added
			panelSize.U = Math.Max( curLineSize.U, panelSize.U );
			panelSize.V += curLineSize.V;

			//go from UV space to W/H space
			return new Size( panelSize.Width, panelSize.Height );
		}

		/// <summary>
		/// <see cref="FrameworkElement.ArrangeOverride"/>
		/// </summary>
		protected override Size ArrangeOverride( Size finalSize )
		{
			int firstInLine = 0;
			double itemWidth = ItemWidth;
			double itemHeight = ItemHeight;
			double accumulatedV = 0;
			double itemU = ( IsHorizontal() ? itemWidth : itemHeight );
			UVSize curLineSize = new UVSize( Orientation );
			bool itemWidthSet = !Double.IsNaN( itemWidth );
			bool itemHeightSet = !Double.IsNaN( itemHeight );
			bool useItemU = ( IsHorizontal() ? itemWidthSet : itemHeightSet );
			int lineCount = 0;

			UIElementCollection children = InternalChildren;

			for( int i=0, count = children.Count; i < count; i++ )
			{
				UIElement child = children[i] as UIElement;
				if( child == null ) continue;

				UVSize sz = new UVSize(
					Orientation,
					( itemWidthSet ? itemWidth : child.DesiredSize.Width ),
					( itemHeightSet ? itemHeight : child.DesiredSize.Height ) );

				if( lineCount < WrapCount ) //同じ行に追加
				{
					curLineSize.U += sz.U;
					curLineSize.V = Math.Max( sz.V, curLineSize.V );
				}
				else //新しい行に移行
				{
					lineCount = 0;

					arrangeLine( accumulatedV, curLineSize.V, firstInLine, i, useItemU, itemU );

					accumulatedV += curLineSize.V;
					curLineSize = sz;

					firstInLine = i;
				}

				lineCount++;
			}

			//arrange the last line, if any
			if( firstInLine < children.Count )
			{
				arrangeLine( accumulatedV, curLineSize.V, firstInLine, children.Count, useItemU, itemU );
			}

			return finalSize;
		}

		private void arrangeLine( double v, double lineV, int start, int end, bool useItemU, double itemU )
		{
			double u = 0;
			bool isHorizontal = IsHorizontal();

			UIElementCollection children = InternalChildren;
			for( int i = start; i < end; i++ )
			{
				UIElement child = children[i] as UIElement;
				if( child != null )
				{
					UVSize childSize = new UVSize( Orientation, child.DesiredSize.Width, child.DesiredSize.Height );
					double layoutSlotU = ( useItemU ? itemU : childSize.U );
					child.Arrange( new Rect(
						( isHorizontal ? u : v ),
						( isHorizontal ? v : u ),
						( isHorizontal ? layoutSlotU : lineV ),
						( isHorizontal ? lineV : layoutSlotU ) ) );
					u += layoutSlotU;
				}
			}
		}

		private bool IsHorizontal()
		{
			return Orientation == Orientation.Horizontal;
		}

		#endregion Protected Methods
	}
}
