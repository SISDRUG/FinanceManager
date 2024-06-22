using System;
using Microsoft.Maui.Storage;
using SQLite;
using System.IO;

namespace FinanceManager
{
    
    public class TodoItem
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Source { get; set; }
        public string Name { get; set; }

    }

    public class AccountStats
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int AccountID { get; set; }
        public string Operation { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public Single Value { get; set; }
        public DateTime date { get; set; }
    }

    public class AppSettings
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public bool ShowLoginPage { get; set; }
        public string Password { get; set; }
    }

    public class Database
    {
        readonly SQLiteAsyncConnection _database;

        public Database(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<TodoItem>().Wait();
            _database.CreateTableAsync<AccountStats>().Wait();
            _database.CreateTableAsync<AppSettings>().Wait();
        }

        public async Task UpdateSettingsAsync(bool showLoginPage, string password)
        {
            var settings = (await _database.Table<AppSettings>().FirstOrDefaultAsync()) ?? new AppSettings();
            settings.ShowLoginPage = showLoginPage;
            settings.Password = password;

            await _database.InsertOrReplaceAsync(settings);
        }

        public async Task<TodoItem> GetAccountByIdAsync(int id)
        {
            return await _database.Table<TodoItem>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }

        public Task<List<AccountStats>> GetAccountStatsByShortStringDate(DateTime dateTime)
        {

            var sortestats = _database.Table<AccountStats>().Where(i=>i.date.ToShortDateString().Equals(dateTime.ToShortDateString()));
            var tt = sortestats.ToListAsync();

            return tt;
        }

        public async Task<bool> GetShowLoginPageAsync()
        {
            var settings = await _database.Table<AppSettings>().FirstOrDefaultAsync();
            return settings?.ShowLoginPage ?? false; // Возвращает false по умолчанию, если настройки не найдены
        }

        public async Task<bool> ISPassword(string password){

            var settings = await _database.Table<AppSettings>().FirstOrDefaultAsync();
            if (password == (settings?.Password ?? ""))
            {
                return true;
            }
            else
            {
                return false;
            }

        }


        public Task<List<TodoItem>> GetItemByIdAsync(int id)
        {
            return _database.Table<TodoItem>().Where(i => i.ID == id).ToListAsync();
        }

        public Task<List<TodoItem>> GetItemsAsync()
        {
            return _database.Table<TodoItem>().ToListAsync();
        }

        public Task<int> SaveItemAsync(TodoItem item)
        {
            if (item.ID != 0)
            {
                return _database.UpdateAsync(item);
            }
            else
            {
                return _database.InsertAsync(item);
            }
        }

        public Task<int> DeleteItemAsync(TodoItem item)
        {
            return _database.DeleteAsync(item);
        }

        public Task<List<AccountStats>> GetAccountStatsAsync()
        {
            return _database.Table<AccountStats>().ToListAsync();
        }

        public Task<List<AccountStats>> GetAccountStatsByIdAsync(int accountid)
        {
            return _database.Table<AccountStats>().Where(i => i.AccountID == accountid).ToListAsync();
        }



        public Task<int> SaveItemAsync(AccountStats item)
        {
            if (item.ID != 0)
            {
                return _database.UpdateAsync(item);
            }
            else
            {
                return _database.InsertAsync(item);
            }
        }

        public Task<int> DeleteItemAsync(AccountStats item)
        {

            return _database.DeleteAsync(item);
        }

    }

}

