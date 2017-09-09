using System;
using SFLibs.Core.Basis;

namespace EarlyPusher.Models
{
    public class MemberData : ObservableObject
    {
        private Guid deviceGuid;
        private int key;
        private string name;

        public Guid DeviceGuid
        {
            get { return deviceGuid; }
            set { SetProperty(ref deviceGuid, value); }
        }

        public int Key
        {
            get { return key; }
            set { SetProperty(ref key, value); }
        }

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }
    }
}
