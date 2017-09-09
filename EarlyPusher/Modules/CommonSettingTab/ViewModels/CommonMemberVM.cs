using EarlyPusher.Models;
using SFLibs.Core.Basis;

namespace EarlyPusher.Modules.CommonSettingTab.ViewModels
{
    public class CommonMemberVM : ViewModelBase<MemberData>
    {
        private bool isKeyLock = true;

        public bool IsKeyLock
        {
            get { return this.isKeyLock; }
            set { SetProperty(ref this.isKeyLock, value); }
        }

        public CommonTeamVM Parent { get; }

        public CommonMemberVM(CommonTeamVM parent, MemberData data)
            : base(data)
        {
            this.Parent = parent;
        }
    }
}
