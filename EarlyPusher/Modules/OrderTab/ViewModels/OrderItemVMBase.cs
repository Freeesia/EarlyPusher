using EarlyPusher.Models;
using SFLibs.Core.Basis;

namespace EarlyPusher.Modules.OrderTab.ViewModels
{
    public abstract class OrderItemVMBase : ObservableObject
    {
        private Choice? choice;

        public Choice? Choice
        {
            get { return this.choice; }
            set { SetProperty(ref this.choice, value); }
        }
    }
}
