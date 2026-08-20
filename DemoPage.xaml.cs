using System.Threading.Tasks;

namespace MAUIAppTest;

public partial class DemoPage : ContentPage
{
    private readonly Services.DatabaseService _dbService;
    //private readonly EmployeePage _employeePage;      


    public DemoPage(Services.DatabaseService dbService)
	{
		InitializeComponent(); 
        //_employeePage = employeePage;
        _dbService = dbService;
    }
}