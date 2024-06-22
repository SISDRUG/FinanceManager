using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace FinanceManager;

public partial class App : Application
{
    public static class Constants
    {
        public const string DatabaseFilename = "TodoSQLite.db3";
        public static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    }


    public App()
    {
        
        InitializeComponent();
   
        MainPage = new NavigationPage( new MyTabbedPage());
    }



}



public class MyTabbedPage : TabbedPage
{
    public MyTabbedPage()
    {
        this.Padding = 20;

        // Добавление дочерних страниц
        Children.Add(new MainPage() { Title = "Главная" });
        Children.Add(new AccountsPage() { Title = "Счета"});
        Children.Add(new ReportsPage() { Title = "Отчет"});
        Children.Add(new ConverterPage() { Title = "Конвертер" });
        Children.Add(new SettingsPage() { Title = "Настройки"});
    }

}


