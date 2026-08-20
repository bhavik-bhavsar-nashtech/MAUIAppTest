namespace MAUIAppTest;

public partial class DemoFlyOutPage : FlyoutPage
{
    private readonly Services.DatabaseService _dbService;
    public DemoFlyOutPage(Services.DatabaseService dbService)
    {
        InitializeComponent();

        _dbService = dbService;

        
        var welcomePage = new DemoPage(_dbService);

        Detail = new NavigationPage(welcomePage);
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        if (Detail is NavigationPage navigationPage)
        {
            await navigationPage.PushAsync(
                new DemoPage(_dbService));
        }
    }

    private async void EmployeeButton_Clicked(object sender, EventArgs e)
    {
        if (Detail is NavigationPage navigationPage)
        {
            var employeePage =
                new EmployeePage(
                    new ViewModels.EmployeeViewModel(_dbService));

            await navigationPage.PushAsync(employeePage);
            //await Navigation.PushAsync(new EmployeePage(new ViewModels.EmployeeViewModel(_dbService)));
        }
    }

    private async void DepartmentButton_Clicked(object sender, EventArgs e)
    {
        if (Detail is NavigationPage navigationPage)
        {
            var demoPage = new DemoTabedPage();

            await navigationPage.PushAsync(demoPage);
        }
    }
}