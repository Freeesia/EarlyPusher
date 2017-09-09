using System;

namespace SFLibs.UI.Data
{
    public interface IEventArgsConverter
    {
        object[] Convert(object sender, EventArgs args);
    }
}
