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
        VStackLabel.Children.Clear();
        //var todayOperations = await _database.GetAccountStatsByShortStringDate(DateTime.Now);
        var todayOperations = await _database.GetAccountStatsAsync();
        Double income = 0;
        Double outcome = 0;
        Double total = 0;

        if (todayOperations.Count != 0)
        {
            
            
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
                        WidthRequest = 300,



                    };

                    var firstVertStack = new VerticalStackLayout
                    {
                        HorizontalOptions = LayoutOptions.Start,
                        MaximumWidthRequest = 100,
                        VerticalOptions = LayoutOptions.Center,


                    };

                    var secondVertStack = new HorizontalStackLayout
                    {
                        HorizontalOptions = LayoutOptions.End,
                        WidthRequest = 220,
                        VerticalOptions = LayoutOptions.Center,



                    };


                    Label lbValue = new Label
                    {
                        HorizontalOptions = LayoutOptions.End,
                        Text = operation.Value.ToString("0.##"),
                        HorizontalTextAlignment = TextAlignment.End,
                        FontSize = 16,
                        MinimumWidthRequest = 70,
                        MaximumWidthRequest = 70,
                        LineBreakMode = LineBreakMode.TailTruncation,
                    };

                    if (operation.Value < 0)
                    {
                        lbValue.TextColor = Colors.DarkRed;
                    }
                    else
                    {
                        lbValue.TextColor = Colors.Green;
                    }

                    Label lbAccount = new Label
                    {
                        HorizontalOptions = LayoutOptions.End,
                        Text = "Счет: " + account.Name,
                        HorizontalTextAlignment = TextAlignment.End,
                        FontSize = 16,
                        MinimumWidthRequest = 150,
                        MaximumWidthRequest = 150,
                        LineBreakMode = LineBreakMode.TailTruncation,
                    };

                    Image accountImage = new Image
                    {
                        Source = account.Source,
                        HeightRequest = 80,
                        WidthRequest = 80,
                        Margin = new Thickness(5,0),

                    };


                    firstVertStack.Children.Add(accountImage);
                    secondVertStack.Children.Add(lbValue);
                    secondVertStack.Children.Add(lbAccount);
                    stackLayout.Children.Add(firstVertStack);
                    stackLayout.Children.Add(secondVertStack);

                    var frame = new Frame {
                        BorderColor = Colors.DarkRed,
                        Content = stackLayout,
                        WidthRequest = 350,
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

            OutcomeLabel.Text = outcome.ToString("0.##") ?? "0";
            IncomeLabel.Text = income.ToString("0.##") ?? "0";
            TotalLabel.Text = total.ToString("0.##") ?? "0";
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
            OutcomeLabel.Text = "0";
            IncomeLabel.Text = "0";
            TotalLabel.Text =  "0";
            Label Lab = new Label { Text = "Сегодня операций еще не было" , HorizontalOptions = LayoutOptions.Center , Margin= new Thickness(5)};
            VStackLabel.Children.Add(Lab);
        }
       
    }


    async void AddButton_Clicked(System.Object sender, System.EventArgs e)
    {
        await Navigation.PushAsync(new AddOperationFromMainPage());
    }

    async void TakeButton_Clicked(System.Object sender, System.EventArgs e)
    {
        await Navigation.PushAsync(new TakeOperationFromMainPage());
    }
}
