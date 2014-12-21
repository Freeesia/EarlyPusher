using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EarlyPusher.Controls
{
	public class ScaleFitControl : ContentControl
	{
		protected override Size ArrangeOverride( Size arrangeBounds )
		{
			int count = this.VisualChildrenCount;

			if( count > 0 )
			{
				UIElement child = (UIElement)( this.GetVisualChild( 0 ) );
				if( child != null )
				{
					if( !child.IsMeasureValid )
					{
						child.Measure( arrangeBounds );
					}

					if( !child.DesiredSize.IsEmpty )
					{
						var widthScale = arrangeBounds.Width / child.DesiredSize.Width;
						var heightScale = arrangeBounds.Height / child.DesiredSize.Height;

						var trans = new TransformGroup();
						if( widthScale > heightScale )
						{
							trans.Children.Add( new ScaleTransform( heightScale, heightScale ) );
							trans.Children.Add( new TranslateTransform( GetOffset( arrangeBounds.Width, child.DesiredSize.Width, heightScale ), 0 ) );
						}
						else
						{
							trans.Children.Add( new ScaleTransform( widthScale, widthScale ) );
							trans.Children.Add( new TranslateTransform( 0, GetOffset( arrangeBounds.Height, child.DesiredSize.Height, widthScale ) ) );
						}

						child.RenderTransform = trans;

						child.Arrange( new Rect( arrangeBounds ) );
					}
				}
			}
			return arrangeBounds;
		}

		private static double GetOffset( double parentSize, double childSize, double scale )
		{
			return ( parentSize - ( childSize * scale ) ) / 2.0;
		}
	}
}
