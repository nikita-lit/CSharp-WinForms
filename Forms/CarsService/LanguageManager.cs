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
                        ["users"] = "Users",
                        ["user"] = "user",
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
                        ["search_car"] = "Search car",
                        ["search_service"] = "Search service",
                        ["search"] = "Search",
                        ["car_services"] = "Maintenance",
                        ["total_revenue"] = "Total Revenue",
                        ["car_owner"] = "Car owner",
                        ["end_time_error"] = "The end time must be later than the start time.",

                        ["id"] = "ID",
                        ["fullname"] = "Full Name",
                        ["phone"] = "Phone Number",
                        ["brand"] = "Brand",
                        ["model"] = "Model",
                        ["registrationnumber"] = "Registration Number",
                        ["name"] = "Name",
                        ["price"] = "Price",
                        ["user_name"] = "Name",
                        ["password"] = "Password",
                        ["role"] = "Role",
                        ["start_time"] = "Start Time",
                        ["end_time"] = "End Time",
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
                        ["users"] = "Kasutajad",
                        ["user"] = "Kasutaja",
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
                        ["search_car"] = "Otsi autot",
                        ["search_service"] = "Otsi teenust",
                        ["search"] = "Otsing",
                        ["car_services"] = "Hooldus",
                        ["total_revenue"] = "Kogutulu",
                        ["car_owner"] = "Auto omanik",
                        ["end_time_error"] = "Lõppaeg peab olema algusajast hilisem.",

                        ["id"] = "ID",
                        ["fullname"] = "Täisnimi",
                        ["phone"] = "Telefon",
                        ["brand"] = "Bränd",
                        ["model"] = "Mudel",
                        ["registrationnumber"] = "Registreerimisnumber",
                        ["name"] = "Nimi",
                        ["price"] = "Hind",
                        ["user_name"] = "Nimi",
                        ["password"] = "Salasõna",
                        ["role"] = "Roll",
                        ["start_time"] = "Algusaeg",
                        ["end_time"] = "Lõppaeg",
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
                        ["users"] = "Пользователи",
                        ["user"] = "Пользователь",
                        ["add"] = "Добавить",
                        ["update"] = "Обновить",
                        ["delete"] = "Удалить",
                        ["invalid"] = "Данные некорректны!",
                        ["row_not_selected"] = "Строка не выбрана!",
                        ["are_you_sure"] = "Вы уверены?",
                        ["are_you_sure_owner"] = "Вы уверены?\nУдаление владельца удалит его машины!",
                        ["warning"] = "Предупреждение!",
                        ["language"] = "Язык",
                        ["search_owner"] = "Поиск владелца",
                        ["search_car"] = "Поиск машины",
                        ["search_service"] = "Поиск услуги",
                        ["search"] = "Поиск",
                        ["car_services"] = "Обслуживание",
                        ["total_revenue"] = "Общий доход",
                        ["car_owner"] = "Владелец машины",
                        ["end_time_error"] = "Время окончания должно быть позже времени начала",

                        ["id"] = "ID",
                        ["fullname"] = "ФИО",
                        ["phone"] = "Телефон",
                        ["brand"] = "Бренд",
                        ["model"] = "Модель",
                        ["registrationnumber"] = "Регистрационный номер",
                        ["name"] = "Название",
                        ["price"] = "Цена",
                        ["user_name"] = "Имя",
                        ["password"] = "Пароль",
                        ["role"] = "Роль",
                        ["start_time"] = "Время начала",
                        ["end_time"] = "Время конца",
                    }
                }
            };

        public static string Get(string id)
        {
            if (Translations[CurrentLanguage].TryGetValue(id.ToLower(), out var value))
                return value;

            return id;
        }
    }
}
