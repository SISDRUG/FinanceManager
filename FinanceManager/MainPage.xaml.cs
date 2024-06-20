﻿using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using static FinanceManager.MainPage;

namespace FinanceManager;

public partial class MainPage : ContentPage
{
    // Использование
    public static class Constants
    {
        public const string DatabaseFilename = "TodoSQLite.db3";
        public static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    }


    List<Frame> framesCollection = new List<Frame>();

    public MainPage()
    {

        InitializeComponent();
        ShowLoginPage();
        CreateFrames();

    }

    public async Task ShowLoginPage()
    {
        var database = new Database(Constants.DatabasePath);
        if (await database.GetShowLoginPageAsync())
        {
            await Navigation.PushModalAsync(new AuthorisationPage());
        }
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
        var database = new Database(Constants.DatabasePath);
        var frames = new List<Frame>();
        var accounts = await database.GetItemsAsync();
        int accountCount = accounts.Count; // Сохраняем количество аккаунтов

        for (int i = 0; i < accountCount; i++)
        {
            var account = accounts[i]; // Локальная переменная для аккаунта
            var frame = new Frame
            {
                BorderColor = Colors.DarkRed,
                CornerRadius = 10,
                Padding = 10,
                HeightRequest = 200,
                Content = new Image { Source = account.Source },
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
            Text = "add",
            WidthRequest = 90,
            Background = Brush.DarkRed,
            TextColor = Colors.WhiteSmoke,

        };


        addButton.Clicked += AddButton_Clicked;
        VertStack.Children.Add(addButton);

        var clearButton = new Button
        {

            Text = "clear",
            WidthRequest = 100,
            IsEnabled = false,
            Background = Brush.DarkRed,
            TextColor = Colors.WhiteSmoke,

        };

        clearButton.Clicked += ClearButton_Clicked; ;
        VertStack.Children.Add(clearButton);

        if (accountCount > 0)
        {
            clearButton.IsEnabled = true;
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
        await Navigation.PushModalAsync(new GeneratingAccountPage());
    }



}

