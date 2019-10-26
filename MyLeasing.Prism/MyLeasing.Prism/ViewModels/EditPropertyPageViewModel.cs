using MyLeasing.Common.Models;
using MyLeasing.Prism.Helpers;
using Prism.Navigation;
using Xamarin.Forms;

namespace MyLeasing.Prism.ViewModels
{
    public class EditPropertyPageViewModel : ViewModelBase
    {
        private PropertyResponse _property;
        private ImageSource _imageSource;
        private bool _isRunning;
        private bool _isEnabled;
        private bool _isEdit;

        public EditPropertyPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            IsEnabled = true;
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
                Title = Languages.AddProperty;
            }
        }
    }
}
