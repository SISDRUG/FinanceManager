namespace FinanceManager;

public partial class CardAccountSettingPage : ContentPage
{

    public static class Constants
    {
        public const string DatabaseFilename = "TodoSQLite.db3";
        public static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    }


    private Database database = new Database(Constants.DatabasePath);

    private TodoItem _account = new TodoItem();

    public CardAccountSettingPage(TodoItem account)
    {
        InitializeComponent();
        CardImage.Source = account.Source;
        EntryAccountName.Text = account.Name;
        _account = account;
    }

    async void Button_Clicked(System.Object sender, System.EventArgs e)
    {
        await Navigation.PopAsync();
    }

    async void ChangeButton_Clicked(System.Object sender, System.EventArgs e)
    {
        if (EntryAccountName.Text != "")
        {
            _account.Name = EntryAccountName.Text;
            await database.SaveItemAsync(_account);
            await Navigation.PopAsync();
        }


        
    }
}
