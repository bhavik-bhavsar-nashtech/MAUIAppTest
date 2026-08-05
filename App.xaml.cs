namespace MAUIAppTest
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //MainPage = new MainPage();
            MainPage = new EmployeePage(new ViewModels.EmployeeViewModel(new Services.DatabaseService()));
        }
    }
}
