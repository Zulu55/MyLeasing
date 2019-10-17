using MyLeasing.Common.Helpers;
using MyLeasing.Common.Models;
using MyLeasing.Common.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace MyLeasing.Prism.Views
{
    public partial class MapPage : ContentPage
    {
        private readonly IGeolocatorService _geolocatorService;
        private readonly IApiService _apiService;

        public MapPage(
            IGeolocatorService geolocatorService,
            IApiService apiService)
        {
            InitializeComponent();
            _geolocatorService = geolocatorService;
            _apiService = apiService;
            ShowPropertiesAsync();
            MoveMapToCurrentPositionAsync();
        }

        private async void ShowPropertiesAsync()
        {
            var url = App.Current.Resources["UrlAPI"].ToString();
            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);

            var response = await _apiService.GetListAsync<PropertyResponse>(
                url, 
                "api",
                "/Owners/GetAvailbleProperties", 
                "bearer", 
                token.Token);

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert("Error", response.Message, "Accept");
                return;
            }

            var properties = (List<PropertyResponse>)response.Result;

            foreach (var property in properties)
            {
                MyMap.Pins.Add(new Pin
                {
                    Address = property.Address,
                    Label = property.Neighborhood,
                    Position = new Position(property.Latitude, property.Longitude),
                    Type = PinType.Place
                });
            }
        }

        private async void MoveMapToCurrentPositionAsync()
        {
            await _geolocatorService.GetLocationAsync();
            var position = new Position(
                _geolocatorService.Latitude,
                _geolocatorService.Longitude);
            MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                position,
                Distance.FromKilometers(.5)));
        }
    }
}
