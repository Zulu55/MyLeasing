using MyLeasing.Common.Helpers;
using MyLeasing.Common.Models;
using MyLeasing.Common.Services;
using MyLeasing.Prism.Helpers;
using Newtonsoft.Json;
using Prism.Navigation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace MyLeasing.Prism.ViewModels
{
    public class EditPropertyPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private PropertyResponse _property;
        private ImageSource _imageSource;
        private bool _isRunning;
        private bool _isEnabled;
        private bool _isEdit;
        private ObservableCollection<PropertyTypeResponse> _propertyTypes;
        private PropertyTypeResponse _propertyType;
        private ObservableCollection<Stratum> _stratums;
        private Stratum _stratum;

        public EditPropertyPageViewModel(
            INavigationService navigationService,
            IApiService apiService) : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            IsEnabled = true;
        }

        public ObservableCollection<PropertyTypeResponse> PropertyTypes
        {
            get => _propertyTypes;
            set => SetProperty(ref _propertyTypes, value);
        }

        public PropertyTypeResponse PropertyType
        {
            get => _propertyType;
            set => SetProperty(ref _propertyType, value);
        }

        public ObservableCollection<Stratum> Stratums
        {
            get => _stratums;
            set => SetProperty(ref _stratums, value);
        }

        public Stratum Stratum
        {
            get => _stratum;
            set => SetProperty(ref _stratum, value);
        }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public bool IsEdit
        {
            get => _isEdit;
            set => SetProperty(ref _isEdit, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        public PropertyResponse Property
        {
            get => _property;
            set => SetProperty(ref _property, value);
        }

        public ImageSource ImageSource
        {
            get => _imageSource;
            set => SetProperty(ref _imageSource, value);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.ContainsKey("property"))
            {
                Property = parameters.GetValue<PropertyResponse>("property");
                ImageSource = Property.FirstImage;
                IsEdit = true;
                Title = Languages.EditProperty;
            }
            else
            {
                Property = new PropertyResponse { IsAvailable = true };
                ImageSource = "noImage";
                IsEdit = false;
                Title = Languages.NewProperty;
            }

            LoadPropertyTypesAsync();
            LoadStratums();
        }

        private void LoadStratums()
        {
            Stratums = new ObservableCollection<Stratum>();
            for (int i = 1; i <= 6; i++)
            {
                Stratums.Add(new Stratum { Id = i, Name = $"{i}" });
            }

            Stratum = Stratums.FirstOrDefault(s => s.Id == Property.Stratum);
        }

        private async void LoadPropertyTypesAsync()
        {
            var url = App.Current.Resources["UrlAPI"].ToString();
            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);

            var response = await _apiService.GetListAsync<PropertyTypeResponse>(
                url, "/api", "/PropertyTypes", "bearer", token.Token);

            IsRunning = false;
            IsEnabled = true;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(Languages.Error, response.Message, Languages.Accept);
                return;
            }

            var propertyTypes = (List<PropertyTypeResponse>)response.Result;
            PropertyTypes = new ObservableCollection<PropertyTypeResponse>(propertyTypes);

            if (!string.IsNullOrEmpty(Property.PropertyType))
            {
                PropertyType = PropertyTypes.FirstOrDefault(pt => pt.Name == Property.PropertyType);
            }
        }
    }
}
