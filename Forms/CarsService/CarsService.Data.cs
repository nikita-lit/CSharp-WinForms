using System.Text.Json;
using WinForms.CarsService.Models;

namespace WinForms.CarsService
{
    public struct UserData
    {
        public string Name { get; set; }
        public string Password { get; set; }

        public LanguageManager.Language Language { get; set; }
    }

    public partial class CarsService
    {
        public static string UserDataPath => Path.Combine(Program.GetDirectory(), "Data/car_service_user_data.json");

        private void SaveUserData(User currentUser)
        {
            if (currentUser == null)
                return;

            try
            {
                UserData data = new()
                {
                    Name = currentUser.Name,
                    Password = currentUser.Password,
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
            SaveUserData(_currentUser);
        }
    }
}