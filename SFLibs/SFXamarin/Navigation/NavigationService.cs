using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SFLibs.UI.Navigation
{
    public class NavigationService
    {
        private static NavigationService instance;
        private Stack<Page> pageStack = new Stack<Page>();

        public static NavigationService Current => instance;

        public NavigationService(Page main)
        {
            Debug.WriteLine("init");
            this.pageStack.Push(main);
            instance = this;
        }

        public async Task PushModalAsync(Page to)
        {
            var from = this.pageStack.Peek();
            await from.Navigation.PushModalAsync(to);
            this.pageStack.Push(to);
            {
                if (from?.BindingContext is INavigationViewModel vm)
                {
                    vm.OnNavigatedFrom();
                }
            }
            {
                if (to.BindingContext is INavigationViewModel vm)
                {
                    vm.OnNavigatedTo(new NavigationArgs(from.BindingContext));
                }
            }
        }

        public async Task PopModalAsync()
        {
            var from = this.pageStack.Pop();
            var to = this.pageStack.Peek();
            await from.Navigation.PopModalAsync();
            {
                if (from?.BindingContext is INavigationViewModel vm)
                {
                    vm.OnNavigatedFrom();
                }
            }
            {
                if (to.BindingContext is INavigationViewModel vm)
                {
                    vm.OnNavigatedTo(new NavigationArgs(from.BindingContext));
                }
            }
        }
    }
}
