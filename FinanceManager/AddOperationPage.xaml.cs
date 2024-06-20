namespace FinanceManager;

public partial class AddOperationPage : ContentPage
{
    public static class Constants
    {
        public const string DatabaseFilename = "TodoSQLite.db3";
        public static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    }

    public AddOperationPage(string cardId)
    {
        InitializeComponent();
        accounIDLabel.Text = cardId;
        GeneratePickerList();


        
    }


    public async void GeneratePickerList()
    {
        var typePickerList = new List<string>();
        typePickerList.Add("income");
        typePickerList.Add("payment");
        typePickerList.Add("products");
        typePickerList.Add("credit");
        typePickerList.Add("house");
        typePickerList.Add("shool");
        typePickerList.Add("present");

        PikerType.ItemsSource = typePickerList;
    }


    async void AddButton_Clicked(System.Object sender, System.EventArgs e)
    {
        var database = new Database(Constants.DatabasePath);
        var operation = new AccountStats { AccountID = Convert.ToInt32(accounIDLabel.Text) , Value = Convert.ToInt32(ValueEntry.Text), Operation = PikerType.SelectedItem?.ToString() ?? "noType" };
        await database.SaveItemAsync(operation);
        await Navigation.PopAsync();
    }

    async void CancleButton_Clicked(System.Object sender, System.EventArgs e)
    {

        await Navigation.PopAsync();
    }    //AccountStats stat = new AccountStats { AccountID = Convert.ToInt32(accountIDLabel.Text), Value = 100, Operation = "income", Type = "friends" };
    //await database.SaveItemAsync(stat);
}
