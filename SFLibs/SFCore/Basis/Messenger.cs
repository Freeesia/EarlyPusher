using System;
using System.Collections.Generic;
using System.Text;

namespace SFLibs.Core.Basis
{
    public class Messenger
    {
        public event EventHandler Executed;

        public void Execute(object sender, EventArgs e)
        {
            this.Executed?.Invoke(sender, e);
        }
    }
}
