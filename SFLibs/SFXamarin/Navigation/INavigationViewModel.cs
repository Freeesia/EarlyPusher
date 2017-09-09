namespace SFLibs.UI.Navigation
{
    public interface INavigationViewModel
    {
        void OnNavigatedTo(NavigationArgs args);
        void OnNavigatedFrom();
    }
}
