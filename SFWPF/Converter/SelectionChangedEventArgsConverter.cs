using System;
using System.Windows.Controls;
using SFLibs.UI.Data;

namespace SFLibs.UI.Converter
{
    public class SelectionChangedEventArgsConverter : IEventArgsConverter
    {
        public object[] Convert(object sender, EventArgs args)
        {
            if (args is SelectionChangedEventArgs a && a.AddedItems.Count > 0)
            {
                return new[] { a.AddedItems[0] };
            }
            return null;
        }
    }
}
