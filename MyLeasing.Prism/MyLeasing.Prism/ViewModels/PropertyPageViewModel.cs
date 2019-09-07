using MyLeasing.Common.Models;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Linq;

namespace MyLeasing.Prism.ViewModels
{
    public class PropertyPageViewModel : ViewModelBase
    {
        private PropertyResponse _property;
        private ObservableCollection<RotatorModel> _imageCollection;

        public PropertyPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "Property";
        }

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

            if (parameters.ContainsKey("property")) ;
            {
                Property = parameters.GetValue<PropertyResponse>("property");
                Title = $"Property: {Property.Neighborhood}";
                LoadImages();
            }
        }

        private void LoadImages()
        {
            ImageCollection = new ObservableCollection<RotatorModel>(Property.PropertyImages.Select(pi => new RotatorModel
            {
                Image = pi.ImageUrl
            }).ToList());
        }
    }
}
