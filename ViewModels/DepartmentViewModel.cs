using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUIAppTest.Models;
using MAUIAppTest.Services;
using Microsoft.Maui.Controls;

namespace MAUIAppTest.ViewModels;

public partial class DepartmentViewModel : ObservableObject
{
    private readonly DatabaseService _dbService;

    public ObservableCollection<Department> Departments { get; set; } = new();
    public ObservableCollection<string> EmployeeNames { get; set; } = new();

    [ObservableProperty] private int _departmentID;
    [ObservableProperty] private string _departmentName = string.Empty;
    [ObservableProperty] private string? _departmentDetail;
    [ObservableProperty] private string? _departmentHeadName;
    [ObservableProperty] private bool _isActive = true;
    [ObservableProperty] private DateTime _departmentCreated = DateTime.UtcNow;

    public DepartmentViewModel(DatabaseService dbService)
    {
        _dbService = dbService;
        _ = LoadDepartmentsAsync();
        _ = LoadEmployeeNamesAsync();
    }

    private async Task LoadEmployeeNamesAsync()
    {
        var emps = await _dbService.GetEmployeesAsync();
        EmployeeNames.Clear();
        foreach (var e in emps)
            EmployeeNames.Add(e.FullName);
    }

    [RelayCommand]
    private async Task LoadDepartmentsAsync()
    {
        var list = await _dbService.GetDepartmentsAsync();
        Departments.Clear();
        foreach (var d in list)
            Departments.Add(d);
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        // Validation: DepartmentName required
        if (string.IsNullOrWhiteSpace(DepartmentName)) return;

        var dept = new Department
        {
            DepartmentID = DepartmentID,
            DepartmentName = DepartmentName,
            DepartmentDetail = DepartmentDetail,
            DepartmentHeadName = DepartmentHeadName,
            IsActive = IsActive,
            DepartmentCreated = DepartmentCreated
        };

        await _dbService.SaveDepartmentAsync(dept);
        ClearForm();
        await LoadDepartmentsAsync();
    }

    [RelayCommand]
    private void SelectForEdit(Department? department)
    {
        if (department is null) return;
        DepartmentID = department.DepartmentID;
        DepartmentName = department.DepartmentName;
        DepartmentDetail = department.DepartmentDetail;
        DepartmentHeadName = department.DepartmentHeadName;
        IsActive = department.IsActive ?? true;
        DepartmentCreated = department.DepartmentCreated;
    }

    [RelayCommand]
    private async Task DeleteAsync(Department? department)
    {
        if (department is null) return;
        await _dbService.DeleteDepartmentAsync(department);
        await LoadDepartmentsAsync();
    }

    [RelayCommand]
    private void ClearForm()
    {
        DepartmentID = 0;
        DepartmentName = string.Empty;
        DepartmentDetail = null;
        DepartmentHeadName = null;
        IsActive = true;
        DepartmentCreated = DateTime.UtcNow;
    }
}
