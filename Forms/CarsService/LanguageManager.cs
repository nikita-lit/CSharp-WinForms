using System.Globalization;
using System.Resources;

namespace WinForms.CarsService
{
    public static class LanguageManager
    {
        private static ResourceManager _resourceManager = new("WinForms.Localization.CarService", typeof(LanguageManager).Assembly);

        public enum Language
        {
            English,
            Estonian,
            Russian
        }

        private static Language _language = Language.English;

        public static Language CurrentLanguage 
        { 
            get { return _language; }
            set
            {
                _language = value;

                CultureInfo culture = null;
                switch(_language)
                {
                    case Language.English:
                        culture = new CultureInfo("en");
                        break;
                    case Language.Estonian:
                        culture = new CultureInfo("et");
                        break;
                    case Language.Russian:
                        culture = new CultureInfo("ru");
                        break;
                }

                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;

                LanguageChanged?.Invoke();
            } 
        }

        public static event Action LanguageChanged;

        public static string Get(string id)
        {
            try
            {
                return _resourceManager.GetString(id, CultureInfo.CurrentUICulture) ?? id;
            }
            catch
            {
                return id;
            }
        }
    }
}
