using System.Globalization;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Net.Http;


namespace FinanceManager;

public partial class ConverterPage : ContentPage
{
    public class Rate
    {
        //public int Cur_ID { get; set; }
        public string Cur_Abbreviation { get; set; }
        public decimal Cur_OfficialRate { get; set; }
        public int Cur_Scale { get; set; }
        // Добавь другие свойства, если нужно
    }


    private readonly Dictionary<string, decimal> _exchangeRates = new Dictionary<string, decimal>
        {
            {"BYN", 1}
        };

    private readonly Dictionary<string, int> _exchangeScale = new Dictionary<string, int>
        {
            {"BYN", 1}
        };

    private List<Rate> Cur;
    private bool _firstEntryFlag = false;
    private bool _secondEntryFlag = false;
    private bool _firstPickerFlag = false;
    private bool _secondPickerFlag = false;


    public ConverterPage()
    {
        InitializeComponent();
        SecondEntry.TextColor = Colors.WhiteSmoke;
        GetExchangeRatesAsync();
        DayLabel.Text = DateTime.Now.ToLongDateString();
    }


    private static readonly HttpClient client = new HttpClient();

    public async void GetExchangeRatesAsync()
    {
        _firstPickerFlag = true;
        _secondPickerFlag = true;
        using var client = new HttpClient();
        var response = await client.GetAsync("https://api.nbrb.by/exrates/rates?periodicity=0");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            Cur =  Newtonsoft.Json.JsonConvert.DeserializeObject<List<Rate>>(content);
            foreach (var cur in Cur)
            {
                _exchangeRates.Add(cur.Cur_Abbreviation,cur.Cur_OfficialRate);
                _exchangeScale.Add(cur.Cur_Abbreviation, cur.Cur_Scale);
            }
            FirstPicker.ItemsSource = _exchangeRates.Keys.ToArray();
            FirstPicker.SelectedIndex = 0;
            SecondPicker.ItemsSource = _exchangeRates.Keys.ToArray();
            SecondPicker.SelectedIndex = 8;
            //AbbreviationArray = _newExchangeRates.Keys.ToArray();
            //AbbreviationArray.SetValue(dff[0].Cur_Abbreviation,AbbreviationArray.Length);

        }
        else
        {
            Console.WriteLine($"Error: {response.StatusCode}");

        }
        _firstPickerFlag = false;
        _secondPickerFlag = false;
        FirstPicker_SelectedIndexChanged(FirstPicker,EventArgs.Empty);
    }


    async void FirstEntry_TextChanged(System.Object sender, Microsoft.Maui.Controls.TextChangedEventArgs e)
    {
        var entry = sender as Entry;
        if (entry == null)
            return;
        if (FirstEntry.Text == "")
            SecondEntry.Text = "";

        //await Task.Delay(1000);
        
        if (!_secondEntryFlag)
        {
            _firstEntryFlag = true;
            // Регулярное выражение для проверки положительных чисел
            Regex regex = new Regex(@"^[0-9]*(\.[0-9]{0,2})?$");

            // Если текст не соответствует регулярному выражению, восстанавливаем предыдущий текст
            if (!regex.IsMatch(entry.Text))
            {
                entry.Text = e?.OldTextValue ?? "";
            }
            else
            {
                // Выполнение переасчета первого поля
                if (Single.TryParse(FirstEntry.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out Single firstValue))
                {
                    string fromCurrency = FirstPicker.SelectedItem?.ToString();
                    string toCurrency = SecondPicker.SelectedItem?.ToString();

                    if (fromCurrency != null && toCurrency != null && _exchangeRates.ContainsKey(fromCurrency) && _exchangeRates.ContainsKey(toCurrency))
                    {
                        Single fromRate = Convert.ToSingle(_exchangeRates[fromCurrency]) / _exchangeScale[fromCurrency];
                        Single toRate = Convert.ToSingle(_exchangeRates[toCurrency]) / _exchangeScale[toCurrency];

                        
                        Single convertedValue = (firstValue * fromRate) / toRate;
                        SecondEntry.Text = convertedValue.ToString("0.##", CultureInfo.InvariantCulture);
                        SecondEntry.TextColor = Colors.WhiteSmoke;
                    }
                }
            }
        }
        await Task.Delay(100);
        _firstEntryFlag = false;
    }

    async void SecondEntry_TextChanged(System.Object sender, Microsoft.Maui.Controls.TextChangedEventArgs e)
    {
        var entry = sender as Entry;
        if (entry == null)
            return;
        if (SecondEntry.Text == "")
            FirstEntry.Text = "";

        
        if (!_firstEntryFlag)
        {
            _secondEntryFlag = true;
            // Регулярное выражение для проверки положительных чисел
            Regex regex = new Regex(@"^[0-9]*(\.[0-9]{0,2})?$");

            // Если текст не соответствует регулярному выражению, восстанавливаем предыдущий текст
            if (!regex.IsMatch(entry.Text))
            {
                entry.Text = e?.OldTextValue ?? "";
            }
            else
            {
                // ВЫполнение переасчета первого поля
                if (Single.TryParse(SecondEntry.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out Single secondValue))
                {
                    string fromCurrency = FirstPicker.SelectedItem?.ToString();
                    string toCurrency = SecondPicker.SelectedItem?.ToString();

                    if (fromCurrency != null && toCurrency != null && _exchangeRates.ContainsKey(fromCurrency) && _exchangeRates.ContainsKey(toCurrency))
                    {
                        Single fromRate = Convert.ToSingle(_exchangeRates[fromCurrency]) / _exchangeScale[fromCurrency];
                        Single toRate = Convert.ToSingle(_exchangeRates[toCurrency]) / _exchangeScale[toCurrency];

                        Single convertedValue = (secondValue * toRate) / fromRate;
                        FirstEntry.Text = convertedValue.ToString("0.##", CultureInfo.InvariantCulture);
                    }
                }
            }
            await Task.Delay(100);
            _secondEntryFlag = false;
        }
    }


    void FirstPicker_SelectedIndexChanged(System.Object sender, System.EventArgs e)
    {
        if (!_secondPickerFlag)
        {
            _firstPickerFlag = true;
            CurLabelFirst.Text = FirstPicker.SelectedItem?.ToString();
            string fromCurrency = FirstPicker.SelectedItem?.ToString();
            string toCurrency = SecondPicker.SelectedItem?.ToString();
            string curTo = ((_exchangeRates[fromCurrency]) / (_exchangeRates[toCurrency] / _exchangeScale[toCurrency])).ToString("F4");
            string curFrom = ((_exchangeRates[toCurrency]) / (_exchangeRates[fromCurrency] / _exchangeScale[fromCurrency])).ToString("F4");
            CurLabelTo.Text = curTo;
            //CurLabelTo.Text = _exchangeScale[fromCurrency] + fromCurrency + " -- " + curTo + " -> " + toCurrency;
            //CurLabelFrom.Text = fromCurrency + " <- " + curFrom + " -- " + _exchangeScale[toCurrency] + toCurrency;

            CurLabelFrom.Text = curFrom;
            _firstPickerFlag = false;
        }
    }

    void SecondPicker_SelectedIndexChanged(System.Object sender, System.EventArgs e)
    {
        if (!_firstPickerFlag)
        {
            _secondPickerFlag = true;
            CurLabelSecond.Text = SecondPicker.SelectedItem?.ToString();
            string fromCurrency = FirstPicker.SelectedItem?.ToString();
            string toCurrency = SecondPicker.SelectedItem?.ToString();
            string curTo = ((_exchangeRates[fromCurrency]) / (_exchangeRates[toCurrency] / _exchangeScale[toCurrency])).ToString("F2");
            string curFrom = ((_exchangeRates[toCurrency]) / (_exchangeRates[fromCurrency] / _exchangeScale[fromCurrency])).ToString("F2");
            CurLabelTo.Text = curTo;
            //CurLabelTo.Text = _exchangeScale[fromCurrency] + fromCurrency + " -- " + curTo + " -> " + toCurrency;
            //CurLabelFrom.Text = fromCurrency + " <- " + curFrom + " -- " + _exchangeScale[toCurrency] + toCurrency;

            CurLabelFrom.Text = curFrom;
            _secondPickerFlag = false;
        }
    }
}
