using MyLeasing.Common.Models;
using Prism.Navigation;
using System.Collections.ObjectModel;

namespace MyLeasing.Prism.ViewModels
{
    public class PropertiesPageViewModel : ViewModelBase
    {
        private OwnerResponse _owner;
        private ObservableCollection<PropertyResponse> _properties;
        
        public PropertiesPageViewModel(
            INavigationService navigationService) : base(navigationService)
        {
            Title = "Properties";
        }

        public ObservableCollection<PropertyResponse> Properties
        {
            get => _properties;
            set => SetProperty(ref _properties, value);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.ContainsKey("owner"))
            {
                _owner = parameters.GetValue<OwnerResponse>("owner");
                Title = $"Properties of: {_owner.FullName}";
                Properties = new ObservableCollection<PropertyResponse>(_owner.Properties);
            }
        }
    }
}
