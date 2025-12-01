using Group4RecycleApp.ViewModels;

namespace Group4RecycleApp.Views;

public partial class AdminManageGuidesPage : ContentPage
{
    public AdminManageGuidesPage(AdminManageGuidesViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override bool OnBackButtonPressed()
    {
        Shell.Current.Navigation.PopAsync();
        return true;
    }

}