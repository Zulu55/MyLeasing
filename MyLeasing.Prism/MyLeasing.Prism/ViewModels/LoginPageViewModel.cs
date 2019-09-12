using MyLeasing.Common.Models;
using MyLeasing.Common.Services;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using System.Net.Http;
using System.Windows.Input;
using Xamarin.Forms;

namespace MyLeasing.Prism.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private string _password;
        private bool _isRunning;
        private bool _isEnabled;
        private DelegateCommand _loginCommand;

        public LoginPageViewModel(
            INavigationService navigationService,
            IApiService apiService) : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            Title = "Login";
            IsEnabled = true;

            //TODO: delete this lines
            Email = "jzuluaga55@hotmail.com";
            Password = "123456";

            OnFacebookLoginSuccessCmd = new Command<string>((authToken) =>
            {
                GetFacebookData(authToken);
            });

            OnFacebookLoginErrorCmd = new Command<string>((err) =>
            {
                App.Current.MainPage.DisplayAlert("Error", $"Authentication failed: {err}", "Accept");
            });

            OnFacebookLoginCancelCmd = new Command(() =>
            {
                App.Current.MainPage.DisplayAlert("Cancel", "Authentication cancelled by the user.", "Accept");
            });
        }

        public ICommand OnFacebookLoginSuccessCmd { get; }

        public ICommand OnFacebookLoginErrorCmd { get; }

        public ICommand OnFacebookLoginCancelCmd { get; }

        public DelegateCommand LoginCommand => _loginCommand ?? (_loginCommand = new DelegateCommand(Login));

        public string Email { get; set; }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        private async void Login()
        {
            if (string.IsNullOrEmpty(Email))
            {
                await App.Current.MainPage.DisplayAlert("Error", "You must enter an email.", "Accept");
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                await App.Current.MainPage.DisplayAlert("Error", "You must enter a password.", "Accept");
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            var url = App.Current.Resources["UrlAPI"].ToString();
            var connection = await _apiService.CheckConnectionAsync(url);
            if (!connection)
            {
                IsEnabled = true;
                IsRunning = false;
                await App.Current.MainPage.DisplayAlert("Error", "Check the internet connection.", "Accept");
                return;
            }

            var request = new TokenRequest
            {
                Password = Password,
                Username = Email
            };

            var response = await _apiService.GetTokenAsync(url, "Account", "/CreateToken", request);

            if (!response.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert("Error", "User or password incorrect.", "Accept");
                Password = string.Empty;
                return;
            }

            var token = response.Result;
            var response2 = await _apiService.GetOwnerByEmailAsync(url, "api", "/Owners/GetOwnerByEmail", "bearer", token.Token, Email);
            if (!response2.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert("Error", "Problem with user data, call support.", "Accept");
                return;
            }

            var owner = response2.Result;
            var parameters = new NavigationParameters
            {
                { "owner", owner }
            };

            await _navigationService.NavigateAsync("PropertiesPage", parameters);
            IsRunning = false;
            IsEnabled = true;
        }

        private async void GetFacebookData(string authToken)
        {
            var requestUrl = $"https://graph.facebook.com/v2.8/me/?fields=name," +
                $"picture.width(999),cover,age_range,devices,email,gender," +
                $"is_verified,birthday,languages,work,website,religion," +
                $"location,locale,link,first_name,last_name," +
                $"hometown&access_token={authToken}";
            var httpClient = new HttpClient();
            var userJson = await httpClient.GetStringAsync(requestUrl);
            var user = JsonConvert.DeserializeObject<FacebookResponse>(userJson);
            await App.Current.MainPage.DisplayAlert("Success", $"Name: {user.Name}, Email: {user.Email}, Id: {user.Id}", "Accept");
        }
    }
}
