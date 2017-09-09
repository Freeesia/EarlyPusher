using System;
using Microsoft.Xaml.Interactivity;
using SFLibs.Core.Basis;
using Windows.UI.Xaml;

namespace SFLibs.UWP.Behavior
{
    public sealed class MessengerTriggerBehavior : Trigger
    {
        public Messenger Messenger
        {
            get { return (Messenger)GetValue(MessengerProperty); }
            set { SetValue(MessengerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Messenger.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessengerProperty =
            DependencyProperty.Register("Messenger", typeof(Messenger), typeof(MessengerTriggerBehavior), new PropertyMetadata(null, MessengerChanged));

        private static void MessengerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = (MessengerTriggerBehavior)d;
            if (e.OldValue is Messenger oldMessenger)
            {
                oldMessenger.Executed -= behavior.Messenger_Executed;
            }
            if (e.NewValue is Messenger newMessenger)
            {
                newMessenger.Executed += behavior.Messenger_Executed;
            }
        }

        private void Messenger_Executed(object sender, EventArgs e)
        {
            Interaction.ExecuteActions(this.AssociatedObject, this.Actions, e);
        }
    }
}
