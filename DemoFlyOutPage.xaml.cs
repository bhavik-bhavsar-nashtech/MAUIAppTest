namespace MAUIAppTest;

public partial class DemoFlyOutPage : FlyoutPage
{
    private readonly LoginPage _loginPage;
    private readonly Services.DatabaseService _dbService;
    public DemoFlyOutPage(Services.DatabaseService dbService, LoginPage loginPage)
    {
        InitializeComponent();

        _dbService = dbService;
        _loginPage = loginPage;


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
            //var demoPage = new DemoTabedPage();
            var deptPage = new DepartmentPage(new ViewModels.DepartmentViewModel(_dbService));

            await navigationPage.PushAsync(deptPage);
        }
    }
    
    private async void LogoutButton_Clicked(object sender, EventArgs e)
    {
        // Ensure UI update on the main thread and replace the app's MainPage
        Application.Current?.Dispatcher.Dispatch(() =>
        {
            var nav = new NavigationPage(_loginPage)
            {
                BarBackground = Colors.Blue,
                BarTextColor = Colors.White
            };
            Application.Current.MainPage = nav;
        });
    }
}