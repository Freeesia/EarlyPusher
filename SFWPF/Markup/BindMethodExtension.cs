using System;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using SFLibs.UI.Data;

namespace SFLibs.UI.Markup
{
    public class BindMethodExtension : MarkupExtension
    {
        public string Path { get; set; }

        public IEventArgsConverter Converter { get; set; }

        public BindMethodExtension(string path)
        {
            this.Path = path;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget targetProvider)
            {
                if (targetProvider.TargetObject is FrameworkElement target)
                {
                    var methodInfo = GetType().GetMethod(nameof(ProxyHandler), BindingFlags.NonPublic | BindingFlags.Instance);
                    if (targetProvider.TargetProperty is MethodInfo targetEventAddMethod)
                    {
                        return Delegate.CreateDelegate(targetEventAddMethod.GetParameters()[1].ParameterType, this, methodInfo);
                    }
                    else if (targetProvider.TargetProperty is EventInfo targetEventInfo)
                    {
                        return Delegate.CreateDelegate(targetEventInfo.EventHandlerType, this, methodInfo);
                    }
                }
            }
            return null;
        }

        private void ProxyHandler(object sender, EventArgs e)
        {
            if (!(sender is FrameworkElement target) || target.DataContext == null)
            {
                return;
            }
            var dataContext = target.DataContext;

            var methodInfo = dataContext.GetType().GetMethod(this.Path, BindingFlags.Public | BindingFlags.Instance);

            var args = this.Converter?.Convert(sender, e) ?? new object[] { };

            methodInfo.Invoke(dataContext, args);
        }
    }
}
