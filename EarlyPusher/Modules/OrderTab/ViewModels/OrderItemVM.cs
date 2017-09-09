using System.Windows.Media;
using EarlyPusher.Modules.OrderTab.Interfaces;

namespace EarlyPusher.Modules.OrderTab.ViewModels
{
    public class OrderItemVM : OrderItemVMBase, IBackColorHolder
    {
        private IBackColorHolder parent;
        private ImageSource image;

        public OrderItemVM(IBackColorHolder parent)
        {
            this.parent = parent;
        }

        public ImageSource Image
        {
            get { return this.image; }
            set { SetProperty(ref this.image, value); }
        }

        public Color BackColor
        {
            get { return this.parent.BackColor; }
        }
    }
}
