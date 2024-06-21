﻿namespace FinanceManager;

public partial class ReportsPage : ContentPage
{
    public static class Constants
    {
        public const string DatabaseFilename = "TodoSQLite.db3";
        public static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    }

    List<StackLayout> stackLayoutCollection = new List<StackLayout>();

    public ReportsPage()
    {
        InitializeComponent();
        ShowOperations();

    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Ваш код для обновления данных или логики
        ShowOperations();
    }


    private async Task ShowOperations()
    {
        operationsStack.Children.Clear();
        var database = new Database(Constants.DatabasePath);
        var stackLayouts = new List<StackLayout>();
        Single total = 0;
        var operations = await database.GetAccountStatsAsync();
        int operationsCount = operations.Count;
        for (int i = 0; i < operationsCount; i++)
        {
            var operation = operations[i];
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
                Text = ("id" + operation.AccountID.ToString() + " value " + operation.Value + " operation " + operation.Operation)
            };

            stackLayout.Children.Add(lb);
            stackLayouts.Add(stackLayout);
        }

        foreach (var stackLayout in stackLayouts)
        {
            operationsStack.Children.Add(stackLayout);
        }

        totalLabel.Text = total.ToString();
    }
}
