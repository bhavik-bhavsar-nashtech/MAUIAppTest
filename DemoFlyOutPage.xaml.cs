namespace MAUIAppTest;

public partial class DemoFlyOutPage : FlyoutPage
{
    private readonly Services.DatabaseService _dbService;
    public DemoFlyOutPage(Services.DatabaseService dbService)
    {
		InitializeComponent();
        _dbService = dbService;
	}

    private void Button_Clicked(object sender, EventArgs e)
    {
		Navigation.PushAsync(new DemoPage(_dbService));
    }
}