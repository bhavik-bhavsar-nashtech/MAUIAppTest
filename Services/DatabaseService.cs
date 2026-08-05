using SQLite;
using MAUIAppTest.Models;

namespace MAUIAppTest.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _database;
    private const string DbName = "EmployeeLocalDb.db3";

    private async Task InitAsync()
    {
        if (_database is not null) return;

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, DbName);
        _database = new SQLiteAsyncConnection(dbPath);
        await _database.CreateTableAsync<Employee>();
    }

    public async Task<List<Employee>> GetEmployeesAsync()
    {
        await InitAsync();
        return await _database!.Table<Employee>().ToListAsync();
    }

    public async Task<int> SaveEmployeeAsync(Employee employee)
    {
        await InitAsync();

        // Save metadata using standard Preferences storage
        Preferences.Default.Set("Last_Modified_Date", DateTime.UtcNow.ToString("O"));

        if (employee.Id != 0)
            return await _database!.UpdateAsync(employee);

        return await _database!.InsertAsync(employee);
    }

    public async Task<int> DeleteEmployeeAsync(Employee employee)
    {
        await InitAsync();
        return await _database!.DeleteAsync(employee);
    }
}
