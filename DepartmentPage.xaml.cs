namespace MAUIAppTest;

public partial class DepartmentPage : ContentPage
{
    public DepartmentPage(ViewModels.DepartmentViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
