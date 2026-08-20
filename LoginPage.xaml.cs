namespace MAUIAppTest;

public partial class LoginPage : ContentPage
{
    private readonly Services.DatabaseService _dbService;
    public LoginPage(Services.DatabaseService dbService)
    {
        InitializeComponent();
        _dbService = dbService;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var user = UsernameEntry.Text?.Trim();
        var pass = PasswordEntry.Text?.Trim();

        if (user == "admin" && pass == "admin")
        {
            // Navigate to DemoFlyOutPage inside a NavigationPage
            var flyout = new DemoFlyOutPage(_dbService, this);
            var nav = new NavigationPage(flyout);
            Application.Current!.MainPage = nav;
        }
        else
        {
            StatusLabel.Text = "Invalid credentials";
            StatusLabel.IsVisible = true;
        }
    }
}
