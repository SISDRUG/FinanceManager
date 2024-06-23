using System.Globalization;
using System.Text.RegularExpressions;
namespace FinanceManager;

public partial class TakeOperationFromMainPage : ContentPage
{
    public static class Constants
    {
        public const string DatabaseFilename = "TodoSQLite.db3";
        public static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    }

    private Database _database = new Database(Constants.DatabasePath);

    private Dictionary<string, int> _accountDictionary = new Dictionary<string, int> { };
    private List<TodoItem> _accounts = new List<TodoItem> { };

    public TakeOperationFromMainPage()
    {
        InitializeComponent();
        GeneratePickerTypeList();
        GeneratePickerAccountList();
    }

    public async void GeneratePickerTypeList()
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
        PikerType.SelectedIndex = 0;
    }

    public async void GeneratePickerAccountList()
    {
        var accounts = await _database.GetAccountsAsync();
        _accounts = accounts;
        int i = 0;
        var accountPickerList = new List<string>();
        AccountImage.Source = accounts[0].Source;
        foreach (var account in accounts)
        {
            if (account.Name == "Базовый")
            {
                i++;
            }
            accountPickerList.Add(account.Name + i);
            _accountDictionary.Add(account.Name + i, account.ID);
        }
        AccountPicker.ItemsSource = accountPickerList;
        AccountPicker.SelectedIndex = 0;
    }

    async void AddButton_Clicked(System.Object sender, System.EventArgs e)
    {
        var database = new Database(Constants.DatabasePath);
        DateTime chosedDate;
        if (DatePiker.Date == DateTime.Today)
        {
            chosedDate = DateTime.Now;
        }
        else
        {
            chosedDate = DatePiker.Date;
        }

        if (DescriptionEditor.Text == null || DescriptionEditor.Text == "")
        {
            DescriptionEditor.Text = "Отсутствует";
        }

        Single.TryParse(ValueEntry.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out Single value);
        var operation = new AccountStats { AccountID = _accountDictionary[AccountPicker.SelectedItem.ToString()], Value = value * -1, Operation = PikerType.SelectedItem?.ToString() ?? "noType", Description = DescriptionEditor.Text, Type = "outcome", date = chosedDate };
        await database.SaveItemAsync(operation);
        await Navigation.PopAsync();
    }

    async void CancleButton_Clicked(System.Object sender, System.EventArgs e)
    {

        await Navigation.PopAsync();
    }    //AccountStats stat = new AccountStats { AccountID = Convert.ToInt32(accountIDLabel.Text), Value = 100, Operation = "income", Type = "friends" };

    void ValueEntry_TextChanged(System.Object sender, Microsoft.Maui.Controls.TextChangedEventArgs e)
    {
        var entry = sender as Entry;
        if (entry == null)
            return;

        Regex regex = new Regex(@"^[0-9]*(\.[0-9]{0,2})?$");

        // Если текст не соответствует регулярному выражению, восстанавливаем предыдущий текст
        if (!regex.IsMatch(entry.Text))
        {
            entry.Text = e?.OldTextValue ?? "";
        }
    }

    void AccountPicker_SelectedIndexChanged(System.Object sender, System.EventArgs e)
    {
        AccountImage.Source = _accounts[AccountPicker.SelectedIndex].Source;
    }
}
