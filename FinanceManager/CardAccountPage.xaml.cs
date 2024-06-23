
namespace FinanceManager;

public partial class CardAccountPage : ContentPage
{
    public static class Constants
    {
        public const string DatabaseFilename = "TodoSQLite.db3";
        public static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    }


    private Database database = new Database(Constants.DatabasePath);

    public CardAccountPage(TodoItem account)
    {
        InitializeComponent();

        accountIDLabel.Text = account.ID.ToString();
        accountName.Text = account.Name;

        StackLayout currentStackLayout = new StackLayout
        {
            Padding = 10,
            HeightRequest = 100,
            Margin = new Thickness(0,0,0,20),
        };

        Image cardImg = new Image { Source = account.Source, HeightRequest = 100};

        currentStackLayout.Children.Add(cardImg);
        //currentFrame.Content.InsertLogicalChild(1,accountIDLabel);

        BancAccountVertStack.Children.Add(currentStackLayout);        


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
        Single total = 0;
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
                Text = "Удалить операцию",
                Background = Brush.DarkRed,
                TextColor = Colors.WhiteSmoke,
                Margin = new Thickness(15),
            };

            deletButton.Clicked += async (sender, e) =>
            {
                await database.DeleteItemAsync(stat);
                ShowStats(accountID);
           };

            stackLayout.Children.Add(lb);
            stackLayout.Children.Add(deletButton);
            var frame = new Frame
            {
                BorderColor = Colors.DarkRed,
                Content = stackLayout,
                WidthRequest = 400,
                Margin = new Thickness(0, 10, 0, 10),
                Padding = 10,
                HeightRequest = 100,
            };

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += async (s, e) =>
            {
                await Navigation.PushAsync(new AboutOperationPage(stat)); // Используем локальную переменную
            };

            frame.GestureRecognizers.Add(tapGestureRecognizer);

            StatVertStack.Children.Add(frame);
        }

        accounValueLabel.Text = total.ToString();

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
}
