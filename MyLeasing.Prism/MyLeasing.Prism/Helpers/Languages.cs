using MyLeasing.Prism.Interfaces;
using MyLeasing.Prism.Resources;
using System;
using System.Collections.Generic;
using System.Text;
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

        public static string Error => Resource.Error;

        public static string EmailError => Resource.EmailError;

        public static string Email => Resource.Email;

        public static string EmailPlaceHolder => Resource.EmailPlaceHolder;

        public static string Password => Resource.Password;

        public static string PasswordPlaceHolder => Resource.PasswordPlaceHolder;

        public static string Rememberme => Resource.Rememberme;

        public static string ForgotPassword => Resource.ForgotPassword;

        public static string Login => Resource.Login;

        public static string Register => Resource.Register;

        public static string Loading => Resource.Loading;

        public static string ErrorNoOwner => Resource.ErrorNoOwner;

        public static string AddProperty => Resource.AddProperty;

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

        public static string Address => Resource.Address;

        public static string AddressError => Resource.AddressError;

        public static string AddressPlaceHolder => Resource.AddressPlaceHolder;

        public static string Save => Resource.Save;

        public static string Contracts => Resource.Contracts;

        public static string AddImage => Resource.AddImage;

        public static string DeleteImage => Resource.DeleteImage;

        public static string PictureSource => Resource.PictureSource;

        public static string Cancel => Resource.Cancel;

        public static string FromCamera => Resource.FromCamera;

        public static string FromGallery => Resource.FromGallery;

        public static string CreateEditPropertyConfirm => Resource.CreateEditPropertyConfirm;

        public static string Created => Resource.Created;

        public static string Edited => Resource.Edited;
    }
}
