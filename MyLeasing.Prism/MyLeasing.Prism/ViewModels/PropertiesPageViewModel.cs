using MyLeasing.Common.Helpers;
using MyLeasing.Common.Models;
using MyLeasing.Prism.Helpers;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Linq;

namespace MyLeasing.Prism.ViewModels
{
    public class PropertiesPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private OwnerResponse _owner;
        private ObservableCollection<PropertyItemViewModel> _properties;
        private DelegateCommand _addPropertyCommand;

        public PropertiesPageViewModel(
            INavigationService navigationService) : base(navigationService)
        {
            _navigationService = navigationService;
            Title = "Properties";
            LoadOwner();
        }

        public DelegateCommand AddPropertyCommand => _addPropertyCommand ?? (_addPropertyCommand = new DelegateCommand(AddPropertyAsync));

        public ObservableCollection<PropertyItemViewModel> Properties
        {
            get => _properties;
            set => SetProperty(ref _properties, value);
        }

        private void LoadOwner()
        {
            _owner = JsonConvert.DeserializeObject<OwnerResponse>(Settings.Owner);

            if (_owner.RoleId == 1)
            {
                Title = $"Properties of: {_owner.FullName}";
            }
            else
            {
                Title = "Available Properties";
            }

            Properties = new ObservableCollection<PropertyItemViewModel>(_owner.Properties.Select(p => new PropertyItemViewModel(_navigationService)
            {
                Address = p.Address,
                Contracts = p.Contracts,
                HasParkingLot = p.HasParkingLot,
                IsAvailable = p.IsAvailable,
                Id = p.Id,
                Neighborhood = p.Neighborhood,
                Price = p.Price,
                PropertyImages = p.PropertyImages,
                PropertyType = p.PropertyType,
                Remarks = p.Remarks,
                Rooms = p.Rooms,
                SquareMeters = p.SquareMeters,
                Stratum = p.Stratum
            }).ToList());
        }

        private async void AddPropertyAsync()
        {
            if (_owner.RoleId != 1)
            {
                await App.Current.MainPage.DisplayAlert(
                    Languages.Error, Languages.ErrorNoOwner, Languages.Accept);
                return;
            }

            await _navigationService.NavigateAsync("EditPropertyPage");
        }
    }
}
