namespace FinanceManager;

public partial class AuthorisationPage : ContentPage
{
    public AuthorisationPage()
    {
        InitializeComponent();

        
    }

    public static class Constants
    {
        public const string DatabaseFilename = "TodoSQLite.db3";
        public static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    }

    async void  AuthorisationBtn_Clicked(System.Object sender, System.EventArgs e)
    {
        var database = new Database(Constants.DatabasePath);

        if (await database.ISPassword(PasswordEntry.Text))
        {
            await Navigation.PopModalAsync();
        }
        else {
            await DisplayAlert("Ошибка", "Пароль введен не правильно", "Попробывать еще");
            PasswordEntry.Text = "";
        }
    }


}
