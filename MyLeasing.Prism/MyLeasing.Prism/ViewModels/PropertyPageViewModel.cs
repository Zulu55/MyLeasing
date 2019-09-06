using MyLeasing.Common.Models;
using Prism.Navigation;

namespace MyLeasing.Prism.ViewModels
{
    public class PropertyPageViewModel : ViewModelBase
    {
        private PropertyResponse _property;

        public PropertyPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "Property";
        }

        public PropertyResponse Property
        {
            get => _property;
            set => SetProperty(ref _property, value);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.ContainsKey("property")) ;
            {
                Property = parameters.GetValue<PropertyResponse>("property");
                Title = $"Property: {Property.Neighborhood}";
            }
        }
    }
}
