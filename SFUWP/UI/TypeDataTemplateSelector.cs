using System;
using System.Collections.Generic;
using System.Linq;
using SFLibs.Core.Basis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace SFLibs.UWP.UI
{
    /// <summary>
    /// 型名をキーにDataTemplateを管理して返すDataTemplateSelector
    /// </summary>
    [ContentProperty(Name = "Templates")]
    public class TypeDataTemplateSelector : DataTemplateSelector
    {
        public static string GetTargetType(DependencyObject obj)
        {
            return (string)obj.GetValue(TargetTypeProperty);
        }

        public static void SetTargetType(DependencyObject obj, string value)
        {
            obj.SetValue(TargetTypeProperty, value);
        }

        // Using a DependencyProperty as the backing store for TargetType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetTypeProperty =
            DependencyProperty.RegisterAttached("TargetType", typeof(string), typeof(TypeDataTemplateSelector), new PropertyMetadata(null));



        /// <summary>
        /// 型名とDataTemplateのリスト
        /// </summary>
        public KeyedList<string, DataTemplate> Templates { get; set; } = new KeyedList<string, DataTemplate>(t => GetTargetType(t));

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item == null)
            {
                return null;
            }

            return this.Templates.TryGetItem(item.GetType().Name, out var template) ? template : null;
        }
    }
}
