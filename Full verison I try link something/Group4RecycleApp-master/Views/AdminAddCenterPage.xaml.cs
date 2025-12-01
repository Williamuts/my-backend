using Group4RecycleApp.ViewModels;

namespace Group4RecycleApp.Views;

public partial class AdminAddCenterPage : ContentPage
{
    public AdminAddCenterPage(AdminAddCenterViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    // FIXED: Use PopAsync here too
    protected override bool OnBackButtonPressed()
    {
        Shell.Current.Navigation.PopAsync();
        return true;
    }
}