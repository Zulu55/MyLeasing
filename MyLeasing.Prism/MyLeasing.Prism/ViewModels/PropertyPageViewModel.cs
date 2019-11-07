using MyLeasing.Common.Helpers;
using MyLeasing.Common.Models;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Linq;

namespace MyLeasing.Prism.ViewModels
{
    public class PropertyPageViewModel : ViewModelBase
    {
        private PropertyResponse _property;
        private ObservableCollection<RotatorModel> _imageCollection;
        private DelegateCommand _editPropertyCommand;
        private readonly INavigationService _navigationService;

        public PropertyPageViewModel(
            INavigationService navigationService) : base(navigationService)
        {
            Title = "Details";
            _navigationService = navigationService;
        }

        public DelegateCommand EditPropertyCommand => _editPropertyCommand ?? (_editPropertyCommand = new DelegateCommand(EditPropertyAsync));

        public PropertyResponse Property
        {
            get => _property;
            set => SetProperty(ref _property, value);
        }

        public ObservableCollection<RotatorModel> ImageCollection
        {
            get => _imageCollection;
            set => SetProperty(ref _imageCollection, value);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            Property = JsonConvert.DeserializeObject<PropertyResponse>(Settings.Property);
            LoadImages();
        }

        private void LoadImages()
        {
            ImageCollection = new ObservableCollection<RotatorModel>(Property.PropertyImages.Select(pi => new RotatorModel
            {
                Image = pi.ImageUrl
            }).ToList());
        }

        private async void EditPropertyAsync()
        {
            var parameters = new NavigationParameters
            {
                { "property", Property }
            };

            await _navigationService.NavigateAsync("EditPropertyPage", parameters);
        }
    }
}
