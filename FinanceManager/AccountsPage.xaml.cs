using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using static FinanceManager.AccountsPage;

namespace FinanceManager;

public partial class AccountsPage : ContentPage
{
    // Использование
    public static class Constants
    {
        public const string DatabaseFilename = "TodoSQLite.db3";
        public static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    }


    List<Frame> framesCollection = new List<Frame>();

    public AccountsPage()
    {

        InitializeComponent();

    }



    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Ваш код для обновления данных или логики
        CreateFrames();
    }



    private async Task ClearBase()
    {
        var database = new Database(Constants.DatabasePath);
        var accounts = await database.GetItemsAsync();
        for (int i = 0; i < accounts.Count(); i++)
        {
            await database.DeleteItemAsync(accounts[i]);
        }
        var operations = await database.GetAccountStatsAsync();
        for (int i = 0; i < operations.Count(); i++)
        {
            await database.DeleteItemAsync(operations[i]);
        }
    }

    private async Task CreateFrames()
    {
        VertStack.Children.Clear();
        var database = new Database(Constants.DatabasePath);
        var frames = new List<Frame>();
        var accounts = await database.GetItemsAsync();
        int accountCount = accounts.Count; // Сохраняем количество аккаунтов

        for (int i = 0; i < accountCount; i++)
        {
            var account = accounts[i]; // Локальная переменная для аккаунта

            Image accountImage = new Image { Source = account.Source,  HeightRequest = 155 };
            Label nameLabel = new Label {Text = account.Name, FontSize = 20 , TextColor = Colors.WhiteSmoke ,FontAttributes = FontAttributes.Bold ,HorizontalOptions = LayoutOptions.Center };
            VerticalStackLayout VSstack = new VerticalStackLayout { HeightRequest = 200 };

            var frameLB = new Frame
            {
                BorderColor = Color.FromRgb(80,80,80),
                BackgroundColor = Color.FromRgb(80, 80, 80),
                CornerRadius = 10,
                MaximumWidthRequest = 200,
                HeightRequest = 60,
                Content = nameLabel,
            };

            VSstack.Children.Add(accountImage);
            VSstack.Children.Add(frameLB);
           

            var frame = new Frame
            {
                BorderColor = Color.FromHex("#009999"),
                CornerRadius = 10,
                Padding = 10,
                HeightRequest = 200,
                Content = VSstack,
            };

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += async (s, e) =>
            {
                await Navigation.PushAsync(new CardAccountPage(account)); // Используем локальную переменную
            };

            frame.GestureRecognizers.Add(tapGestureRecognizer);
            frames.Add(frame);
        }

        foreach (var frame in frames)
        {
            VertStack.Children.Add(frame);
        }

        var addButton = new Button
        {
            Text = "Добавить счет",
            Background = Brush.Green,
            TextColor = Colors.WhiteSmoke,

        };


        addButton.Clicked += AddButton_Clicked;
        VertStack.Children.Add(addButton);

        var clearButton = new Button
        {

            Text = "Удалить все",
            IsEnabled = false,
            Background = Brush.DarkRed,
            TextColor = Colors.WhiteSmoke,

        };

        clearButton.Clicked += ClearButton_Clicked; ;
        VertStack.Children.Add(clearButton);

        if (accountCount > 0)
        {
            clearButton.IsEnabled = true;
            clearButton.TextColor = Colors.WhiteSmoke;
        }
    }

    private async void ClearButton_Clicked(object sender, EventArgs e)
    {
        await ClearBase();
        VertStack.Children.Clear();
        await CreateFrames();
    }

    private async void AddButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GeneratingAccountPage() { Title = "Новый счет"});
    }



}

