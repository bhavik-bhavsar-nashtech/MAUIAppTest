using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUIAppTest.Models;
using MAUIAppTest.Services;
using Microsoft.Maui.Controls;

namespace MAUIAppTest.ViewModels;

public partial class EmployeeViewModel : ObservableObject
{
    public readonly DatabaseService _dbService;

    public ObservableCollection<Employee> Employees { get; set; } = [];

    // Fields annotated with ObservableProperty automatically produce public counterpart properties
    [ObservableProperty] private int _id;
    [ObservableProperty] private string _fullName = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _department = string.Empty;
    [ObservableProperty] private string _lastSyncInfo = "Never";
    [ObservableProperty] private string _theme = "Unspecified";

    public EmployeeViewModel(DatabaseService dbService)
    {
        _dbService = dbService;

        // Kick off load operations
        _ = LoadEmployeesAsync();
    }

    private async Task LoadMetadataAsync()
    {
        //var lastSaved = Preferences.Default.Get("Last_Modified_Date", "Never");
        var lastSaved = await _dbService.GetPreferenceAsync("Last_Modified_Date") ?? "Never";
        LastSyncInfo = $"Last local change: {lastSaved}";

        var themePref = await _dbService.GetPreferenceAsync("App_Theme") ?? "Unspecified";
        Theme = themePref;
        ApplyTheme(themePref);
    }

    [RelayCommand]
    private async Task LoadEmployeesAsync()
    {
        var list = await _dbService.GetEmployeesAsync();
        Employees.Clear();
        foreach (var emp in list)
        {
            Employees.Add(emp);
        }

        await LoadMetadataAsync();
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(FullName)) return;

        var emp = new Employee
        {
            Id = Id,
            FullName = FullName,
            Email = Email,
            Department = Department
        };

        await _dbService.SaveEmployeeAsync(emp);
        ClearForm();
        await LoadEmployeesAsync();
    }

    [RelayCommand]
    private void SelectForEdit(Employee? employee)
    {
        if (employee is null) return;
        Id = employee.Id;
        FullName = employee.FullName;
        Email = employee.Email;
        Department = employee.Department;
    }

    [RelayCommand]
    private async Task DeleteAsync(Employee? employee)
    {
        if (employee is null) return;
        await _dbService.DeleteEmployeeAsync(employee);
        await LoadEmployeesAsync();
    }

    [RelayCommand]
    private void ClearForm()
    {
        Id = 0;
        FullName = string.Empty;
        Email = string.Empty;
        Department = string.Empty;
    }

    private void ApplyTheme(string theme)
    {
        if (Application.Current is null) return;

        if (Enum.TryParse<AppTheme>(theme, ignoreCase: true, out var parsed))
        {
            Application.Current.UserAppTheme = parsed;
        }
        else
        {
            Application.Current.UserAppTheme = AppTheme.Unspecified;
        }
    }

    [RelayCommand]
    private async Task SetThemeAsync(string theme)
    {
        if (string.IsNullOrWhiteSpace(theme)) return;

        await _dbService.SetPreferenceAsync("App_Theme", theme);
        Theme = theme;
        ApplyTheme(theme);
    }
}
