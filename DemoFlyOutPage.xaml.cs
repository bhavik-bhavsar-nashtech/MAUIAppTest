namespace MAUIAppTest;

public partial class DemoFlyOutPage : FlyoutPage
{
    private readonly Services.DatabaseService _dbService;
    public DemoFlyOutPage(Services.DatabaseService dbService)
    {
        InitializeComponent();

        _dbService = dbService;

        var employeePage =
            new EmployeePage(
                new ViewModels.EmployeeViewModel(_dbService));

        Detail = new NavigationPage(employeePage);
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        if (Detail is NavigationPage navigationPage)
        {
            await navigationPage.PushAsync(
                new DemoPage(_dbService));
        }
    }
}