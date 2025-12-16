namespace WinForms.CarsService
{
    public static class LanguageManager
    {
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
                LanguageChanged?.Invoke();
            } 
        }

        public static event Action LanguageChanged;

        public static readonly Dictionary<Language, Dictionary<string, string>> Translations =
            new()
            {
                {
                    Language.English, new()
                    {
                        ["car_service"] = "Car Service",
                        ["owners"] = "Owners",
                        ["owner"] = "Owner",
                        ["cars"] = "Cars",
                        ["car"] = "Car",
                        ["services"] = "Services",
                        ["service"] = "Service",
                        ["add"] = "Add",
                        ["update"] = "Update",
                        ["delete"] = "Delete",
                        ["invalid"] = "Data is invalid!",
                        ["row_not_selected"] = "Row isn't selected!",
                        ["are_you_sure"] = "Are you sure?",
                        ["are_you_sure_owner"] = "Are you sure?\nDeleting an owner deletes their cars!",
                        ["warning"] = "Warning!",
                        ["language"] = "Language",
                        ["search_owner"] = "Search owner",
                        ["search"] = "Search",

                        ["id"] = "ID",
                        ["fullname"] = "Full Name",
                        ["phone"] = "Phone Number",
                        ["brand"] = "Brand",
                        ["model"] = "Model",
                        ["registrationnumber"] = "Registration Number",
                        ["name"] = "Name",
                        ["price"] = "Price",
                    }
                },
                {
                    Language.Estonian, new()
                    {
                        ["car_service"] = "Autoteenindus",
                        ["owners"] = "Omanikud",
                        ["owner"] = "Omanik",
                        ["cars"] = "Autod",
                        ["car"] = "Auto",
                        ["services"] = "Teenused",
                        ["service"] = "Teenus",
                        ["add"] = "Lisa",
                        ["update"] = "Uuenda",
                        ["delete"] = "Kustuta",
                        ["invalid"] = "Andmed on vigased!",
                        ["row_not_selected"] = "Rida pole valitud!",
                        ["are_you_sure"] = "Oled kindel?",
                        ["are_you_sure_owner"] = "Oled kindel?\nOmaniku kustutamine kustutab ka autod!",
                        ["warning"] = "Hoiatus!",
                        ["language"] = "Keel",
                        ["search_owner"] = "Otsi omanikku",
                        ["search"] = "Otsing",

                        ["id"] = "ID",
                        ["fullname"] = "Täisnimi",
                        ["phone"] = "Telefon",
                        ["brand"] = "Bränd",
                        ["model"] = "Mudel",
                        ["registrationnumber"] = "Registreerimisnumber",
                        ["name"] = "Nimi",
                        ["price"] = "Hind",
                    }
                },
                {
                    Language.Russian, new()
                    {
                        ["car_service"] = "Автосервис",
                        ["owners"] = "Владельцы",
                        ["owner"] = "Владелец",
                        ["cars"] = "Машины",
                        ["car"] = "Машина",
                        ["services"] = "Услуги",
                        ["service"] = "Услуга",
                        ["add"] = "Добавить",
                        ["update"] = "Обновить",
                        ["delete"] = "Удалить",
                        ["invalid"] = "Данные некорректны!",
                        ["row_not_selected"] = "Строка не выбрана!",
                        ["are_you_sure"] = "Вы уверены?",
                        ["are_you_sure_owner"] = "Вы уверены?\nУдаление владельца удалит его машины!",
                        ["warning"] = "Предупреждение!",
                        ["language"] = "Язык",
                        ["search_owner"] = "Искать владелца",
                        ["search"] = "Поиск",

                        ["id"] = "ID",
                        ["fullname"] = "ФИО",
                        ["phone"] = "Телефон",
                        ["brand"] = "Бренд",
                        ["model"] = "Модель",
                        ["registrationnumber"] = "Регистрационный номер",
                        ["name"] = "Название",
                        ["price"] = "Цена",
                    }
                }
            };

        public static string Get(string id)
        {
            if (Translations[CurrentLanguage].TryGetValue(id, out var value))
                return value;

            return id;
        }
    }
}
