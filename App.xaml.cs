namespace MAUIAppTest
{
    public partial class App : Application
    {
        public App(EmployeePage employeePage)
        {
            InitializeComponent();

            //MainPage = new MainPage();
            //MainPage = new EmployeePage(new ViewModels.EmployeeViewModel(new Services.DatabaseService()));
            MainPage = employeePage;
        }
    }
}
