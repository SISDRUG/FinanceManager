namespace FinanceManager;

public partial class AboutOperationPage : ContentPage
{
    public static class Constants
    {
        public const string DatabaseFilename = "TodoSQLite.db3";
        public static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    }

    private Database _database = new Database(Constants.DatabasePath);

    public AboutOperationPage(AccountStats operation)
    {
        InitializeComponent();
        InitializeInformations(operation);
        
    }

    public async Task InitializeInformations(AccountStats operation)
    {
        DataLabel.Text = operation.date.ToLongDateString();
        TimeLabel.Text = operation.date.ToShortTimeString();
        var account = await _database.GetAccountByIdAsync(operation.AccountID);
        NameLabel.Text = "Счет: " + account.Name;
        AccountImage.Source =  account.Source;
        OperationLabel.Text = "Операци: " + operation.Operation;
        ValueLabel.Text = operation.Value.ToString();
        if (operation.Value < 0)
        {
            ValueLabel.TextColor = Colors.Red;
        }
        else
        {
            ValueLabel.TextColor = Colors.Green;
        }
        TypeLabel.Text = "Под тип: " + operation.Type;
        DescriptionLabel.Text = operation.Description;
    }
}
