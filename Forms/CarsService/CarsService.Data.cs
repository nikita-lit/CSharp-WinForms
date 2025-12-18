using System.Text.Json;

namespace WinForms.CarsService
{
    public struct UserData
    {
        public LanguageManager.Language Language { get; set; }
    }

    public partial class CarsService
    {
        private UserData _currentUserData;

        public static string UserDataPath => Path.Combine(Program.GetDirectory(), "Data/car_service_user_data.json");

        private void SaveUserData()
        {
            try
            {
                UserData data = new()
                {
                    Language = LanguageManager.CurrentLanguage,
                };

                string json = JsonSerializer.Serialize(data);
                File.WriteAllText(UserDataPath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Customer data saving error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadUserData()
        {
            try
            {
                var json = File.ReadAllText(UserDataPath);
                _currentUserData = JsonSerializer.Deserialize<UserData>(json);
            }
            catch (Exception ex)
            {
                if (ex is not FileNotFoundException)
                    MessageBox.Show(ex.ToString(), "User data loading error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            SaveUserData();
        }
    }
}