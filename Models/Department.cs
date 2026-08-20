using SQLite;

namespace MAUIAppTest.Models;

public class Department
{
    [PrimaryKey, AutoIncrement]
    public int DepartmentID { get; set; }

    [NotNull]
    public string DepartmentName { get; set; } = string.Empty;

    public string? DepartmentDetail { get; set; }

    public string? DepartmentHeadName { get; set; }

    public bool? IsActive { get; set; }

    public DateTime DepartmentCreated { get; set; } = DateTime.UtcNow;
}
