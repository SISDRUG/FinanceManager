using System.Text.RegularExpressions;
using System.Globalization;

namespace FinanceManager;

public partial class TakeOperationsPage : ContentPage
{

    public static class Constants
    {
        public const string DatabaseFilename = "TodoSQLite.db3";
        public static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    }

    private Database _database = new Database(Constants.DatabasePath);

    public TakeOperationsPage(string cardId)
    {
        InitializeComponent();
        accounIDLabel.Text = cardId;
        GeneratePickerList();
        InitializeInformation(cardId);
    }



    public async void GeneratePickerList()
    {
        var typePickerList = new List<string>();
        typePickerList.Add("Списание");
        typePickerList.Add("Налог");
        typePickerList.Add("Продукты");
        typePickerList.Add("Кредит");
        typePickerList.Add("Комунльные платежи");
        typePickerList.Add("Долг");
        typePickerList.Add("Покупка");

        PikerType.ItemsSource = typePickerList;
        PikerType.SelectedIndex = 0;
    }

    async Task InitializeInformation(string CardId)
    {
        var account = await _database.GetAccountByIdAsync(int.Parse(CardId));
        AccountImage.Source = account.Source;
        NameLabel.Text = account.Name;

    }


    async void AddButton_Clicked(System.Object sender, System.EventArgs e)
    {
        var database = new Database(Constants.DatabasePath);
        DateTime chosedDate;
        if (DatePiker.Date == DateTime.Today)
        {
            chosedDate = DateTime.Now;
        }
        else
        {
            chosedDate = DatePiker.Date;
        }

        if (DescriptionEditor.Text == null || DescriptionEditor.Text == "")
        {
            DescriptionEditor.Text = "Отсутствует";
        }

        Double.TryParse(ValueEntry.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out Double value);
        var operation = new AccountStats { AccountID = Convert.ToInt32(accounIDLabel.Text), Value = value * -1, Operation = PikerType.SelectedItem?.ToString() ?? "noType", Description = DescriptionEditor.Text, Type = "outcome", date = chosedDate };
        await database.SaveItemAsync(operation);
        await Navigation.PopAsync();
    }

    async void CancleButton_Clicked(System.Object sender, System.EventArgs e)
    {

        await Navigation.PopAsync();
    }    //AccountStats stat = new AccountStats { AccountID = Convert.ToInt32(accountIDLabel.Text), Value = 100, Operation = "income", Type = "friends" };

    void ValueEntry_TextChanged(System.Object sender, Microsoft.Maui.Controls.TextChangedEventArgs e)
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
    }    //await database.SaveItemAsync(stat);
}