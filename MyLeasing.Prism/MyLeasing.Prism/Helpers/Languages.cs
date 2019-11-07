using MyLeasing.Prism.Interfaces;
using MyLeasing.Prism.Resources;
using Xamarin.Forms;

namespace MyLeasing.Prism.Helpers
{
    public static class Languages
    {
        static Languages()
        {
            var ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            Resource.Culture = ci;
            DependencyService.Get<ILocalize>().SetLocale(ci);
        }

        public static string Accept => Resource.Accept;

        public static string Email => Resource.Email;

        public static string EmailError => Resource.EmailError;

        public static string EmailPlaceHolder => Resource.EmailPlaceHolder;

        public static string Error => Resource.Error;

        public static string Forgot => Resource.Forgot;

        public static string Login => Resource.Login;

        public static string LoginError => Resource.LoginError;

        public static string Password => Resource.Password;

        public static string PasswordError => Resource.PasswordError;

        public static string PasswordPlaceHolder => Resource.PasswordPlaceHolder;

        public static string Register => Resource.Register;

        public static string Rememberme => Resource.Rememberme;

        public static string Loging => Resource.Loging;
        
        public static string ErrorNoOwner => Resource.ErrorNoOwner;
        
        public static string NewProperty => Resource.NewProperty;

        public static string Delete => Resource.Delete;

        public static string EditProperty => Resource.EditProperty;

        public static string ChangeImage => Resource.ChangeImage;

        public static string Neighborhood => Resource.Neighborhood;

        public static string NeighborhoodError => Resource.NeighborhoodError;

        public static string NeighborhoodPlaceHolder => Resource.NeighborhoodPlaceHolder;

        public static string Price => Resource.Price;

        public static string PriceError => Resource.PriceError;

        public static string PricePlaceHolder => Resource.PricePlaceHolder;

        public static string SquareMeters => Resource.SquareMeters;

        public static string SquareMetersError => Resource.SquareMetersError;

        public static string SquareMetersPlaceHolder => Resource.SquareMetersPlaceHolder;

        public static string Rooms => Resource.Rooms;

        public static string RoomsError => Resource.RoomsError;

        public static string RoomsPlaceHolder => Resource.RoomsPlaceHolder;

        public static string Stratum => Resource.Stratum;

        public static string StratumError => Resource.StratumError;

        public static string StratumPlaceHolder => Resource.StratumPlaceHolder;

        public static string PropertyType => Resource.PropertyType;

        public static string PropertyTypeError => Resource.PropertyTypeError;

        public static string PropertyTypePlaceHolder => Resource.PropertyTypePlaceHolder;

        public static string HasParkingLot => Resource.HasParkingLot;

        public static string IsAvailable => Resource.IsAvailable;

        public static string Remarks => Resource.Remarks;

        public static string Saving => Resource.Saving;

        public static string Save => Resource.Save;

        public static string Address => Resource.Address;

        public static string AddressError => Resource.AddressError;

        public static string AddressPlaceHolder => Resource.AddressPlaceHolder;
    }
}
