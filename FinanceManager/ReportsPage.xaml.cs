using System.Text.RegularExpressions;

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
    private bool _ShowOperationProcessing = false;
    private Double MainMinValue = 0;
    private Double MainMaxValue = 0;

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
        CurrentMinMaxValueInitialize();
        Indicator.IsRunning = false;
        Indicator.IsVisible = false;
        _ShowOperationProcessing = false;
    }

    public async void CurrentMinMaxValueInitialize()
    {
        var minValue = await _database.GetMinValueStatsAsync();
        MainMinValue = minValue;
        MinValueEntry.Placeholder = minValue.ToString() ;
        var maxValue = await _database.GetMaxValueStatsAsync();
        MainMaxValue = maxValue;
        MaxValueEntry.Placeholder = maxValue.ToString();
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
            if (accountPickerList.Contains(account.Name))
            {
                accountPickerList.Add(account.Name + i);
                _accountDictionary.Add(account.Name + i, account.ID);
                i++;
            }
            else
            {
                accountPickerList.Add(account.Name);
                _accountDictionary.Add(account.Name, account.ID);
            }
        }
        AccountPicker.ItemsSource = accountPickerList;
        AccountPicker.SelectedIndex = 0;
    }


    private async Task ShowOperations()
    {
        if (!_ShowOperationProcessing)
        {
            _ShowOperationProcessing = true;

            Indicator.IsRunning = true;
            Indicator.IsVisible = true;
            operationsStack.Children.Clear();
            Single MaxValue = Single.MaxValue;
            Single MinValue = Single.MinValue;


            Double total = 0;
            List<AccountStats> accountsStats = new List<AccountStats> { };

            if (AccountPicker.SelectedItem.ToString() != "Все")
            {
                accountsStats = await _database.GetAccountStatsByIdAsync(_accountDictionary[AccountPicker.SelectedItem.ToString()]);
            }
            else
            {
                accountsStats = await _database.GetAccountStatsAsync();
            }

            if (MaxValueEntry.Text != null & MaxValueEntry.Text != "")
            {
                MaxValue = Single.Parse(MaxValueEntry.Text);
            }

            if (MinValueEntry.Text != null & MinValueEntry.Text != "")
            {
                MinValue = Single.Parse(MinValueEntry.Text);
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
                        //if (operation.Description.Contains(SearchEntry.Text) || SearchEntry.Text == null || SearchEntry.Text == "")
                        string pattern = $@"\b{SearchEntry.Text}\b";
                        if (Regex.IsMatch(operation.Description, pattern) || SearchEntry.Text == null || SearchEntry.Text == "")
                        {

                            if (true)
                            {

                                if (MaxValue >= operation.Value || MaxValueEntry.Text == null || MaxValueEntry.Text == "")
                                {

                                    if (MinValue <= operation.Value || MinValueEntry.Text == null || MinValueEntry.Text == "")
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

                                        Label lbDate = new Label
                                        {
                                            HorizontalOptions = LayoutOptions.Center,
                                            VerticalOptions = LayoutOptions.Center,
                                            Text = (operation.date.ToString()),
                                            WidthRequest = 200,
                                            HorizontalTextAlignment = Microsoft.Maui.TextAlignment.Start,
                                        };

                                        Label lbAccount = new Label
                                        {
                                            HorizontalOptions = LayoutOptions.Center,
                                            VerticalOptions = LayoutOptions.Center,
                                            Text = (account.Name),
                                            MaximumWidthRequest = 220,
                                            LineBreakMode = LineBreakMode.TailTruncation,
                                        };

                                        Label lbOperation = new Label
                                        {
                                            HorizontalOptions = LayoutOptions.Center,
                                            VerticalOptions = LayoutOptions.Center,
                                            Text = (operation.Operation),
                                        };

                                        Label lbValue = new Label
                                        {
                                            HorizontalOptions = LayoutOptions.Center,
                                            VerticalOptions = LayoutOptions.Center,
                                            Text = (operation.Value.ToString()),
                                            TextColor = Colors.Green,
                                            FontSize = 22,
                                        };

                                        if (operation.Value < 0)
                                        {
                                            lbValue.TextColor = Colors.DarkRed;
                                        }

                                        stackLayout.Children.Add(lbValue);
                                        stackLayout.Children.Add(lbAccount);

                                        HorizontalStackLayout HStack = new HorizontalStackLayout
                                        {
                                            HorizontalOptions = LayoutOptions.Center,
                                            VerticalOptions = LayoutOptions.Center,
                                            Margin = new Thickness(0, 10, 0, 0),

                                        };

                                        HStack.Children.Add(lbDate);
                                        HStack.Children.Add(lbOperation);

                                        stackLayout.Children.Add(HStack);
                                        var frame = new Frame
                                        {
                                            BorderColor = Color.FromHex("#009999"),
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
                        }

                    }
                }
            }

            totalLabel.Text = total.ToString("0.##");
            Indicator.IsRunning = false;
            Indicator.IsVisible = false;
            _ShowOperationProcessing = false;
        }
    }

    void AccountPicker_SelectedIndexChanged(System.Object sender, System.EventArgs e)
    {
        ShowOperations();
    }

    public async void GeneratePickerOperationsList()
    {
        var typePickerList = new List<string>();
        typePickerList.Add("Все");
        typePickerList.Add("Зачисление");
        typePickerList.Add("Запрлата");
        typePickerList.Add("Девиденты");
        typePickerList.Add("Подарок");
        typePickerList.Add("Списание");
        typePickerList.Add("Налог");
        typePickerList.Add("Продукты");
        typePickerList.Add("Кредит");
        typePickerList.Add("Комунльные платежи");
        typePickerList.Add("Долг");
        typePickerList.Add("Покупка");

        OperationPicker.ItemsSource = typePickerList;
        OperationPicker.SelectedIndex = 0;
    }

    public async void GeneratePozitivePickerOperationsList()
    {
        var typePickerList = new List<string>();
        typePickerList.Add("Все");
        typePickerList.Add("Зачисление");
        typePickerList.Add("Запрлата");
        typePickerList.Add("Девиденты");
        typePickerList.Add("Подарок");

        OperationPicker.ItemsSource = typePickerList;
        OperationPicker.SelectedIndex = 0;
    }

    public async void GenerateNegativePickerOperationsList()
    {
        var typePickerList = new List<string>();
        typePickerList.Add("Все");
        typePickerList.Add("Зачисление");
        typePickerList.Add("Запрлата");
        typePickerList.Add("Девиденты");
        typePickerList.Add("Подарок");

        OperationPicker.ItemsSource = typePickerList;
        OperationPicker.SelectedIndex = 0;
    }


    void OperationTypePicker_SelectedIndexChanged(System.Object sender, System.EventArgs e)
    {

        ShowOperations();
        var index = OperationTypePicker.SelectedIndex;
        if (index == 0)
        {
            GeneratePickerOperationsList();
        }

        if (index == 1)
        {
            GeneratePozitivePickerOperationsList();
        }
        if (index == 2)
        {
            GenerateNegativePickerOperationsList();
        }
    }

    void OperationPicker_SelectedIndexChanged(System.Object sender, System.EventArgs e)
    {
        ShowOperations();
    }

    void SearchEntry_TextChanged(System.Object sender, Microsoft.Maui.Controls.TextChangedEventArgs e)
    {
        //Task.Delay(200);
        ShowOperations();

    }

    void MaxValueEntry_TextChanged(System.Object sender, Microsoft.Maui.Controls.TextChangedEventArgs e)
    {
        var entry = sender as Entry;
        if (entry == null)
            return;

        string pattern = @"^-?\d*(\.\d{0,2})?$";
        if (!Regex.IsMatch(entry.Text, pattern))
        {
            // Если ввод не соответствует шаблону, откатываем текст
            entry.Text = Regex.Match(entry.Text, pattern).Value;
            entry.CursorPosition = entry.Text.Length;
        }
        else if (entry.Text != null & entry.Text != "")
        {
            if (entry.Text.ToString()[0] != '-' || entry.Text.Count() > 1)
            {
                ShowOperations();
                Indicator.IsVisible = false;
            }
        }
        else
        {
            ShowOperations();
        }
    }

    void MinValueEntry_TextChanged(System.Object sender, Microsoft.Maui.Controls.TextChangedEventArgs e)
    {
        var entry = sender as Entry;
        if (entry == null)
            return;

        string pattern = @"^-?\d*(\.\d{0,2})?$";
        if (!Regex.IsMatch(entry.Text, pattern))
        {
            // Если ввод не соответствует шаблону, откатываем текст
            entry.Text = Regex.Match(entry.Text, pattern).Value;
            entry.CursorPosition = entry.Text.Length;
        }
        else if (entry.Text != null & entry.Text != "")
        {
            if (entry.Text.ToString()[0] != '-' || entry.Text.Count() > 1)
            {
                ShowOperations();
                Indicator.IsVisible = false;
            }
        }
        else
        {
            ShowOperations();
        }

    }




















    public void PDFbutton_Clicked(System.Object sender, System.EventArgs e)
    {

    }
}

