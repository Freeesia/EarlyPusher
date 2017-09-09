namespace SFLibs.UI.Navigation
{
    public class NavigationArgs
    {
        public object FromViewModel { get; }

        public NavigationArgs(object from)
        {
            this.FromViewModel = from;
        }
    }
}
