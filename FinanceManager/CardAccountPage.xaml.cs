
namespace FinanceManager;

public partial class CardAccountPage : ContentPage
{
    public static class Constants
    {
        public const string DatabaseFilename = "TodoSQLite.db3";
        public static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    }


    private Database database = new Database(Constants.DatabasePath);
    private TodoItem _account = new TodoItem();



    public CardAccountPage(TodoItem account)
    {
        InitializeComponent();
        this.Title = account.Name;
        accountIDLabel.Text = account.ID.ToString();
        _account = account;
        AccountImage.Source = account.Source;


        //StackLayout currentStackLayout = new StackLayout
        //{
        //    Padding = 10,
        //    HeightRequest = 100,
        //    Margin = new Thickness(70,0,0,20),
    //};

    //    Image cardImg = new Image { Source = account.Source, HeightRequest = 100};

        //currentStackLayout.Children.Add(cardImg);
        ////currentFrame.Content.InsertLogicalChild(1,accountIDLabel);

        //BancAccountVertStack.Children.Add(currentStackLayout);        


    }


    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Ваш код для обновления данных или логики
        ShowStats(Convert.ToInt32(accountIDLabel.Text));
        
    }


    private async Task ShowStats(int accountID)
    {
        StatVertStack.Children.Clear();
        var database = new Database(Constants.DatabasePath);
        var account = await database.GetAccountByIdAsync(accountID);
        var accountsStats = await database.GetAccountStatsByIdAsync(accountID);
        var stackLayouts = new List<StackLayout>();
        int statsCount = accountsStats.Count;
        Double total = 0;
        for (int i = 0; i < statsCount; i++)
        {
            var stat = accountsStats[i]; // Локальная переменная для аккаунта
            total += stat.Value;
            var stackLayout = new StackLayout
            {
                HorizontalOptions = LayoutOptions.Center ,
                VerticalOptions = LayoutOptions.Center,
                Padding = 10,
                HeightRequest = 120,
            };

            Label lbDate = new Label
            {
                Text = (stat.date.ToLongDateString()),
                HorizontalOptions = LayoutOptions.Center
            };
            Label lbTime = new Label
            {
                Text = (stat.date.ToLongTimeString()),
                FontSize = 12,
                WidthRequest= 133,
                HorizontalOptions = LayoutOptions.Center,
            };
            Label lbValue = new Label
            {
                Text = (stat.Value.ToString("0.##")),
                WidthRequest = 100,
                Margin = new Thickness(40, 0),
                TextColor = Colors.Green,
                FontSize = 18,
                HorizontalOptions = LayoutOptions.Start
            };
            Label lbOperation = new Label
            {
                Text = stat.Operation,
                WidthRequest = 133,
                HorizontalOptions = LayoutOptions.End,
            };

            Button deletButton = new Button
            {
                Text = "Удалить операцию",
                Background = Brush.DarkRed,
                TextColor = Colors.WhiteSmoke,
                Margin = new Thickness(10),
            };

            deletButton.Clicked += async (sender, e) =>
            {
                await database.DeleteItemAsync(stat);
                ShowStats(accountID);
           };

            if (stat.Value < 0)
            {
                lbValue.TextColor = Colors.DarkRed;
            }


            HorizontalStackLayout HStack = new HorizontalStackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                WidthRequest = 400,
            };

            HStack.Children.Add(lbValue);
            HStack.Children.Add(lbTime);
            HStack.Children.Add(lbOperation);

            stackLayout.Children.Add(lbDate);
            stackLayout.Children.Add(lbTime);
            stackLayout.Children.Add(HStack);
            stackLayout.Children.Add(deletButton);



            var frame = new Frame
            {
                BorderColor = Colors.DarkRed,
                Content = stackLayout,
                WidthRequest = 400,
                Margin = new Thickness(0, 10, 0, 10),
                Padding = 10,
                HeightRequest = 120,
            };

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += async (s, e) =>
            {
                await Navigation.PushAsync(new AboutOperationPage(stat)); // Используем локальную переменную
            };

            frame.GestureRecognizers.Add(tapGestureRecognizer);

            StatVertStack.Children.Add(frame);
        }

        accounValueLabel.Text = total.ToString("0.##");

    }



    async void addStatBtn_Clicked(System.Object sender, System.EventArgs e)
    {
        await Navigation.PushAsync(new AddOperationPage(accountIDLabel.Text));
        
        
        
    }

    async void DeletAccountButton_Clicked(System.Object sender, System.EventArgs e)
    {
        var thisAccount = await database.GetItemByIdAsync(int.Parse(accountIDLabel.Text));
        await database.DeleteItemAsync(thisAccount[0]);
        var thisAccountOperations = await database.GetAccountStatsByIdAsync(int.Parse(accountIDLabel.Text));
        foreach (var operation in thisAccountOperations)
        {
            await database.DeleteItemAsync(operation);
        }
        await Navigation.PopAsync();
    }

    async void takeStatBtn_Clicked(System.Object sender, System.EventArgs e)
    {
        await Navigation.PushAsync(new TakeOperationsPage(accountIDLabel.Text));

    }

    async void Button_Clicked(System.Object sender, System.EventArgs e)
    {
        var account = await database.GetAccountByIdAsync(int.Parse(accountIDLabel.Text));
        await Navigation.PushAsync(new CardAccountSettingPage(account));
        _account = await database.GetAccountByIdAsync(int.Parse(accountIDLabel.Text));
        //accountName.Text = _account.Name;
    }
}
