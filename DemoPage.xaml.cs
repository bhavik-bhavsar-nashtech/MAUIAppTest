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

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EmployeePage(new ViewModels.EmployeeViewModel(_dbService)));
		//await Navigation.PushAsync(_employeePage);
    }
}