namespace FinanceManager;

public partial class MainPage : ContentPage
{
    public static class Constants
    {
        public const string DatabaseFilename = "TodoSQLite.db3";
        public static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    }

    private Database _database = new Database(Constants.DatabasePath);

    public MainPage()
    {
        InitializeComponent();
        DataLabel.Text = DateTime.Now.ToShortDateString();
        ShowLoginPage();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Ваш код для обновления данных или логики
        ShowTodayOperations();
    }

    public async Task ShowLoginPage()
    {
        if (await _database.GetShowLoginPageAsync())
        {
            await Navigation.PushModalAsync(new AuthorisationPage());
        }
    }

    public async Task ShowTodayOperations()
    {
        VStackTodayOperations.Children.Clear();
        //var todayOperations = await _database.GetAccountStatsByShortStringDate(DateTime.Now);
        var todayOperations = await _database.GetAccountStatsAsync();
        
        Console.WriteLine();
        if (todayOperations.Count != 0)
        {
            float income = 0;
            float outcome = 0;
            float total = 0;
            
            foreach (var operation in todayOperations)
            {
                if (operation.date.ToShortDateString() == DateTime.Now.ToShortDateString())
                {

                    var account = await _database.GetAccountByIdAsync(operation.AccountID);
                    total += operation.Value;

                    if (operation.Type == "income")
                    {
                        income += operation.Value;
                    }

                    if (operation.Type == "outcome")
                    {
                        outcome += operation.Value;
                    }



                    var stackLayout = new HorizontalStackLayout
                    {
                        HorizontalOptions = LayoutOptions.Center,
                        WidthRequest = 500,



                    };

                    var firstVertStack = new VerticalStackLayout
                    {
                        HorizontalOptions = LayoutOptions.Start,
                        MaximumWidthRequest = 100,
                        VerticalOptions = LayoutOptions.Center,


                    };

                    var secondVertStack = new VerticalStackLayout
                    {
                        HorizontalOptions = LayoutOptions.End,
                        WidthRequest = 360,
                        VerticalOptions = LayoutOptions.Center,



                    };

                    string labelText =operation.Value.ToString()+ "     " + "Счет: " + account.Name;

                    Label lb = new Label
                    {
                        HorizontalOptions = LayoutOptions.End,
                        Text = labelText,
                        HorizontalTextAlignment = TextAlignment.End,
                        FontSize = 20,
                        MaximumWidthRequest = 360,
                        LineBreakMode = LineBreakMode.TailTruncation,
                    };

                    if (operation.Value < 0)
                    {
                        lb.TextColor = Colors.DarkRed;
                    }
                    else
                    {
                        lb.TextColor = Colors.Green;
                    }


                    Image accountImage = new Image
                    {
                        Source = account.Source,
                        HeightRequest = 80,
                        WidthRequest = 80,
                        Margin = new Thickness(40,0),

                    };


                    firstVertStack.Children.Add(accountImage);
                    secondVertStack.Children.Add(lb);
                    stackLayout.Children.Add(firstVertStack);
                    stackLayout.Children.Add(secondVertStack);

                    var frame = new Frame {
                        BorderColor = Colors.DarkRed,
                        Content = stackLayout,
                        WidthRequest = 500,
                        Margin = new Thickness(0, 10, 0, 10),
                        Padding = 10,
                        HeightRequest = 100,
                    };

                    

                    var tapGestureRecognizer = new TapGestureRecognizer();
                    tapGestureRecognizer.Tapped += async (s, e) =>
                    {
                        await Navigation.PushAsync(new AboutOperationPage(operation));
                    };

                    frame.GestureRecognizers.Add(tapGestureRecognizer);

                    VStackTodayOperations.Children.Add(frame);
                }
            }
            //var totalAmount = Convert.ToUInt64(income + outcome * -1) ;
            //PBar.ProgressTo(outcome, (uint)totalAmount ,Easing.BounceIn);
            var totalAmount = (income / ((income + outcome *-1)/100))/100;

            PBar.Progress = totalAmount;

            OutcomeLabel.Text = outcome.ToString() ?? "0";
            IncomeLabel.Text = income.ToString() ?? "0";
            TotalLabel.Text = total.ToString() ?? "0";
            if (total < 0)
            {
                TotalLabel.TextColor = Colors.Red;
            }
            else
            {
                TotalLabel.TextColor = Colors.Green;
            }

        }
        else
        {
            Label Lab = new Label { Text = "Сегодня операций еще не было" };
            VStackTodayOperations.Children.Add(Lab);
        }
       
    }

}
