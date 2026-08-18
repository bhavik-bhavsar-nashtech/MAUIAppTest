using SQLite;

namespace MAUIAppTest.Models;

public class Preference
{
    [PrimaryKey]
    public string Key { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;
}