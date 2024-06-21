using Microsoft.Maui.Controls;
using System.Text.RegularExpressions;
using System.Globalization;

namespace FinanceManager;

public partial class GeneratingAccountPage : ContentPage
{
    public static class Constants
    {
        public const string DatabaseFilename = "TodoSQLite.db3";
        public static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    }

    private Database database = new Database(Constants.DatabasePath);

    private Dictionary<string, string> Sources = new Dictionary<string, string>
    {
        {"DebitCard", "card.png" },
        {"Cash", "cash.png" },
        {"CreditCard", "creditcard.png" }
    };

    public GeneratingAccountPage()
    {
        InitializeComponent();
        TypePicker.SelectedIndex = 0;

    }

    void TypePicker_SelectedIndexChanged(System.Object sender, System.EventArgs e)
    {
        string selectedItem = TypePicker.SelectedItem.ToString();
        if (Sources.ContainsKey(selectedItem))
        {
            TypeImage.Source = Sources[selectedItem];
        };
    }

    void StartValueEntry_TextChanged(System.Object sender, Microsoft.Maui.Controls.TextChangedEventArgs e)
    {
        var entry = sender as Entry;
        if (entry == null)
            return;

        Regex regex = new Regex(@"^[0-9]*(\.[0-9]{0,2})?$");

        // Если текст не соответствует регулярному выражению, восстанавливаем предыдущий текст
        if (!regex.IsMatch(entry.Text))
        {
            entry.Text = e?.OldTextValue ?? "";
        }
    }

    async void addButton_Clicked(System.Object sender, System.EventArgs e)
    {
        var account = new TodoItem { Source = Sources[TypePicker.SelectedItem.ToString()]};
        await database.SaveItemAsync(account);
        var accounts = await database.GetItemsAsync();
        Single.TryParse(StartValueEntry.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out Single StartValue);
        var initOperation = new AccountStats { AccountID = accounts[accounts.Count - 1].ID, Operation = "Start Value" , Value = StartValue, Type = "Init" };
        await database.SaveItemAsync(initOperation);
        await Navigation.PopAsync();
    }

    async void CancleButton_Clicked(System.Object sender, System.EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
