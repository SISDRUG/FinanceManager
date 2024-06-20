using System.Text.Json;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace FinanceManager
{
    public partial class SettingsPage : ContentPage
    {
        public static class Constants
        {
            public const string DatabaseFilename = "TodoSQLite.db3";
            public static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
        }

        private bool _isProcessingSwitch = false;

        public SettingsPage()
        {
            InitializeComponent();
            PasswordSwitchInitialize();
        }

        public async Task PasswordSwitchInitialize()
        {
            _isProcessingSwitch = true;
            var database = new Database(Constants.DatabasePath);
            PasswordSwitch.IsToggled = await database.GetShowLoginPageAsync();
            _isProcessingSwitch = false;
        }

        public async Task<bool> PromptForPasswordAsync()
        {
            string password = await Application.Current.MainPage.DisplayPromptAsync("Пароль", "Введите пароль:");
            if (password != null)
            {
                string confirmPassword = await Application.Current.MainPage.DisplayPromptAsync("Подтверждение пароля", "Введите пароль еще раз:");
                if (!string.IsNullOrEmpty(password) && password == confirmPassword)
                {
                    // Здесь код для использования пароля
                    var database = new Database(Constants.DatabasePath);
                    await database.UpdateSettingsAsync(true, password);
                    return true;
                }
                else
                {
                    // Отмена действия или вывод сообщения об ошибке
                    _isProcessingSwitch = true;
                    PasswordSwitch.IsToggled = false;
                    _isProcessingSwitch = false;
                    return false;
                }
            }
            else
            {
                // Отмена действия или вывод сообщения об ошибке
                _isProcessingSwitch = true;
                PasswordSwitch.IsToggled = false;
                _isProcessingSwitch = false;
                return false;
            }
        }

        async void PasswordSwitch_Toggled(System.Object sender, Microsoft.Maui.Controls.ToggledEventArgs e)
        {
            if (_isProcessingSwitch)
            {
                return;
            }

            _isProcessingSwitch = true;
            var database = new Database(Constants.DatabasePath);

            if (PasswordSwitch.IsToggled)
            {
                bool isShowLoginPage = await PromptForPasswordAsync();
                if (!isShowLoginPage)
                {
                    PasswordSwitch.IsToggled = false;
                }
            }
            else
            {
                string password = await Application.Current.MainPage.DisplayPromptAsync("Пароль", "Введите пароль:");
                if (await database.ISPassword(password))
                {
                    await database.UpdateSettingsAsync(false,"");
                    PasswordSwitch.IsToggled = false;
                }
                else
                {
                    PasswordSwitch.IsToggled = true;
                }
            }

            _isProcessingSwitch = false;
        }
    }
}