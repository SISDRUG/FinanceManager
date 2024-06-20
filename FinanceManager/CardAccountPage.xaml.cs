namespace FinanceManager;

public partial class CardAccountPage : ContentPage
{
    public static class Constants
    {
        public const string DatabaseFilename = "TodoSQLite.db3";
        public static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    }

    List<StackLayout> stackLayoutCollection = new List<StackLayout>();

    public CardAccountPage(TodoItem account)
    {
        InitializeComponent();

        accountIDLabel.Text = account.ID.ToString();

        StackLayout currentStackLayout = new StackLayout
        {
            Padding = 10,
            HeightRequest = 200,
            Margin = new Thickness(0,20,0,200),
        };

        Image cardImg = new Image { Source = account.Source };

        currentStackLayout.Children.Add(cardImg);
        //currentFrame.Content.InsertLogicalChild(1,accountIDLabel);

        BancAccountVertStack.Children.Add(currentStackLayout);

        ShowStats(account.ID);
        


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
        //var accountsStats = await database.GetAccountStatsAsync();
        var accountsStats = await database.GetAccountStatsByIdAsync(accountID);
        var stackLayouts = new List<StackLayout>();
        int statsCount = accountsStats.Count;
        int total = 0;
        for (int i = 0; i < statsCount; i++)
        {
            var stat = accountsStats[i]; // Локальная переменная для аккаунта
            total += stat.Value;
            var stackLayout = new StackLayout
            {
                HorizontalOptions = LayoutOptions.Center ,
                VerticalOptions = LayoutOptions.Center,
                Padding = 10,
                HeightRequest = 100,
            };

            Label lb = new Label
            {
                Text = ("id" + stat.AccountID.ToString() + " value " + stat.Value + " operation " + stat.Operation)
            };

            Button deletButton = new Button
            {
                Text = "del",
                Background = Brush.DarkRed,
                TextColor = Colors.WhiteSmoke,
            };

            deletButton.Clicked += async (sender, e) =>
            {
                await database.DeleteItemAsync(stat);
                ShowStats(accountID);
           };

            stackLayout.Children.Add(lb);
            stackLayout.Children.Add(deletButton);
            stackLayouts.Add(stackLayout);
        }

        accounValueLabel.Text = total.ToString();

        foreach (var stackLayout in stackLayouts)
        {
            StatVertStack.Children.Add(stackLayout);
        }
    }



    async void addStatBtn_Clicked(System.Object sender, System.EventArgs e)
    {
        await Navigation.PushAsync(new AddOperationPage(accountIDLabel.Text));
        
        
        
    }
}
