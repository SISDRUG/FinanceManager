

namespace FinanceManager;

public partial class ReportsPage : ContentPage
{
    public static class Constants
    {
        public const string DatabaseFilename = "TodoSQLite.db3";
        public static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    }

    private Database _database = new Database(Constants.DatabasePath);
    private Dictionary<string, int> _accountDictionary = new Dictionary<string, int> { };
    private List<TodoItem> _accounts = new List<TodoItem> { };

    public ReportsPage()
    {
        InitializeComponent();
        //GeneratePickerAccountList();
        //GeneratePickerOperationsList();
        OperationTypePicker.SelectedIndex = 0;
        //ShowOperations();

    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Ваш код для обновления данных или логики
        GeneratePickerAccountList();
        GeneratePickerOperationsList();
        ShowOperations();
    }

    public async void GeneratePickerOperationsList()
    {
        var typePickerList = new List<string>();
        typePickerList.Add("Все");
        typePickerList.Add("income");
        typePickerList.Add("payment");
        typePickerList.Add("products");
        typePickerList.Add("credit");
        typePickerList.Add("house");
        typePickerList.Add("shool");
        typePickerList.Add("present");

        OperationPicker.ItemsSource = typePickerList;
        OperationPicker.SelectedIndex = 0;
    }

    public async void GeneratePickerAccountList()
    {
        var accounts = await _database.GetAccountsAsync();
        _accounts = accounts;
        int i = 0;
        var accountPickerList = new List<string>();
        _accountDictionary.Clear();
        accountPickerList.Clear();
        accountPickerList.Add("Все");
        foreach (var account in _accounts)
        {

            accountPickerList.Add(account.Name);
            _accountDictionary.Add(account.Name, account.ID);
        }
        AccountPicker.ItemsSource = accountPickerList;
        AccountPicker.SelectedIndex = 0;
    }


    private async Task ShowOperations()
    {
        operationsStack.Children.Clear();
        
        Single total = 0;
        List<AccountStats> accountsStats = new List<AccountStats> { };

        if (AccountPicker.SelectedItem.ToString() != "Все")
        {
            accountsStats = await _database.GetAccountStatsByIdAsync(_accountDictionary[AccountPicker.SelectedItem.ToString()]);
        }
        else
        {
            accountsStats = await _database.GetAccountStatsAsync();
        }



        foreach (var operation in accountsStats)
        {
            string operationType = "Все";
            if (OperationTypePicker.SelectedItem.ToString() == "Зачисление")
            {
                operationType = "income";
            }
            else if (OperationTypePicker.SelectedItem.ToString() == "Списывания")
            {
                 operationType = "outcome"; 
            }


            if (operation.Type.ToString() == operationType || operationType == "Все")
            {
                if (operation.Operation.ToString() == OperationPicker.SelectedItem.ToString() || OperationPicker.SelectedItem.ToString() == "Все")
                {


                     // Локальная переменная для аккаунта
                    var account = await _database.GetAccountByIdAsync(operation.AccountID);
                    total += operation.Value;
                    var stackLayout = new StackLayout
                    {
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        Padding = 10,
                        HeightRequest = 100,
                    };

                    Label lb = new Label
                    {
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        //HeightRequest = 100,

                        Text = (account.Name + "  " + operation.Value + "  " + operation.date.ToString()),
                        TextColor = Colors.Green,
                    };

                    if (operation.Value < 0)
                    {
                        lb.TextColor = Colors.DarkRed;
                    }

                    stackLayout.Children.Add(lb);
                    var frame = new Frame
                    {
                        BorderColor = Colors.DarkRed,
                        Content = stackLayout,
                        WidthRequest = 300,
                        Margin = new Thickness(0, 10, 0, 10),
                        Padding = 10,
                        HeightRequest = 100,
                    };

                    var tapGestureRecognizer = new TapGestureRecognizer();
                    tapGestureRecognizer.Tapped += async (s, e) =>
                    {
                        await Navigation.PushAsync(new AboutOperationPage(operation)); // Используем локальную переменную
                    };

                    frame.GestureRecognizers.Add(tapGestureRecognizer);

                    operationsStack.Children.Add(frame);
                }
            }
        }

        totalLabel.Text = total.ToString();
    }

    void AccountPicker_SelectedIndexChanged(System.Object sender, System.EventArgs e)
    {
        ShowOperations();
    }

    void OperationTypePicker_SelectedIndexChanged(System.Object sender, System.EventArgs e)
    {
        ShowOperations();
    }

    void OperationPicker_SelectedIndexChanged(System.Object sender, System.EventArgs e)
    {
        ShowOperations();
    }
}
