using System;
using SFLibs.Core.Basis;

namespace EarlyPusher.Modules.ChoiceTab.ViewModels
{
    public class SelectableItemVM : ObservableObject
    {
        private bool isSelected;

        public TeamChoiceVM Parent { get; set; }
        public Guid Device { get; set; }
        public int Key { get; set; }

        public bool IsSelected
        {
            get { return this.isSelected; }
            set { SetProperty(ref this.isSelected, value); }
        }
    }
}
