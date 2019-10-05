using Prism.Navigation;

namespace MyLeasing.Prism.ViewModels
{
    public class MapPageViewModel : ViewModelBase
    {
        public MapPageViewModel(
            INavigationService navigationService) : base(navigationService)
        {
            Title = "Map";
        }
    }
}
