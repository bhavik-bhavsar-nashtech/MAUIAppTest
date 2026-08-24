using SQLite;
using MAUIAppTest.Models;

namespace MAUIAppTest.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _database;
    private const string DbName = "EmployeeLocalDb.db3";
    private readonly ErrorHandlingService _errorHandler;

    public DatabaseService(ErrorHandlingService errorHandler)
    {
        _errorHandler = errorHandler;
    }

    private async Task InitAsync()
    {
        if (_database is not null) return;

        //var dbPath = Path.Combine(FileSystem.AppDataDirectory, DbName);
        var dbPath = await GetDatabasePathAsync();
        _database = new SQLiteAsyncConnection(dbPath);
        await _database.CreateTableAsync<Employee>();
        await _database.CreateTableAsync<Preference>();
        await _database.CreateTableAsync<Department>();

    }

    private string GetDatabasePath2()
    {
#if DEBUG && WINDOWS
    return Path.Combine(Directory.GetCurrentDirectory(), DbName);

#elif ANDROID
    return Path.Combine(@"D:\RD\MAUI_App\MAUIAppTest\", DbName);
#else
        return Path.Combine(FileSystem.AppDataDirectory, DbName);
#endif
    }

    private async Task<string> GetDatabasePathAsync()
    {
        var dbPath = Path.Combine(
            FileSystem.AppDataDirectory,
            DbName);

        #if DEBUG && WINDOWS
                dbPath = Path.Combine(@"D:\RD\MAUI_App\MAUIAppTest\", DbName);
        #endif

        // Already copied previously
        if (File.Exists(dbPath))
            return dbPath;

        // First run: copy database from application package
        using var sourceStream =
            await FileSystem.OpenAppPackageFileAsync(DbName);

        using var destinationStream =
            File.Create(dbPath);

        await sourceStream.CopyToAsync(destinationStream);

        return dbPath;
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
        //Preferences.Default.Set("Last_Modified_Date", DateTime.UtcNow.ToString("O"));

        // Persist last modified in SQLite preferences table
        var now = DateTime.UtcNow.ToString("O");
        await SetPreferenceAsync("Last_Modified_Date", now);

        if (employee.Id != 0)
        {
            var res = await _database!.UpdateAsync(employee);
            await _errorHandler.LogMessageAsync($"Employee updated: {employee.FullName} (Id={employee.Id})");
            return res;
        }

        var inserted = await _database!.InsertAsync(employee);
        await _errorHandler.LogMessageAsync($"Employee created: {employee.FullName} (Id={inserted})");
        return inserted;
    }

    public async Task<int> DeleteEmployeeAsync(Employee employee)
    {
        await InitAsync();
        var res = await _database!.DeleteAsync(employee);
        await _errorHandler.LogMessageAsync($"Employee deleted: {employee.FullName} (Id={employee.Id})");
        return res;
    }

    // Expose a test method to validate centralized error logging
    public async Task TestErrorLoggingAsync()
    {
        // Log a test message
        await _errorHandler.LogMessageAsync("Test log from DatabaseService.TestErrorLoggingAsync");

        // Optionally throw an exception to test exception logging
        try
        {
            throw new InvalidOperationException("This is a test exception from DatabaseService.TestErrorLoggingAsync");
        }
        catch (Exception ex)
        {
            await _errorHandler.LogExceptionAsync(ex, "TestErrorLoggingAsync");
            throw; // rethrow so caller can observe if desired
        }
    }

    // Preference helpers
    public async Task<string?> GetPreferenceAsync(string key)
    {
        await InitAsync();
        var pref = await _database!.Table<Preference>().Where(p => p.Key == key).FirstOrDefaultAsync();
        return pref?.Value;
    }

    public async Task<int> SetPreferenceAsync(string key, string value)
    {
        await InitAsync();
        var existing = await _database!.FindAsync<Preference>(key);
        if (existing is null)
        {
            var p = new Preference { Key = key, Value = value };
            return await _database.InsertAsync(p);
        }

        existing.Value = value;
        return await _database.UpdateAsync(existing);
    }

    public async Task<int> DeletePreferenceAsync(string key)
    {
        await InitAsync();
        var existing = await _database!.FindAsync<Preference>(key);
        if (existing is null) return 0;
        return await _database.DeleteAsync(existing);
    }

    public async Task<List<Department>> GetDepartmentsAsync()
    {
        await InitAsync();
        return await _database!.Table<Department>().ToListAsync();
    }

    // Department helpers
    public async Task<int> SaveDepartmentAsync(Models.Department dept)
    {
        await InitAsync();
        if (dept.DepartmentID != 0)
        {
            await _errorHandler.LogMessageAsync($"Department updated: {dept.DepartmentName} (Id={dept.DepartmentID})");
            return await _database!.UpdateAsync(dept);
        }

        await _errorHandler.LogMessageAsync($"Department created: {dept.DepartmentName} (Id={dept.DepartmentID})");
        return await _database!.InsertAsync(dept);
    }

    public async Task<int> DeleteDepartmentAsync(Models.Department dept)
    {
        await InitAsync();
        await _errorHandler.LogMessageAsync($"Department deleted: {dept.DepartmentName} (Id={dept.DepartmentID})");
        return await _database!.DeleteAsync(dept);
    }
}
