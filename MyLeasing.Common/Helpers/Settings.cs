using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace MyLeasing.Common.Helpers
{
    public static class Settings
    {
        private const string _property = "property";
        private static readonly string _settingsDefault = string.Empty;

        private static ISettings AppSettings => CrossSettings.Current;

        public static string Property
        {
            get => AppSettings.GetValueOrDefault(_property, _settingsDefault);
            set => AppSettings.AddOrUpdateValue(_property, value);
        }
    }
}
