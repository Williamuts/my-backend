using Group4RecycleApp.ViewModels;

namespace Group4RecycleApp.Views;

public partial class AdminEditGuidePage : ContentPage
{
    public AdminEditGuidePage(AdminEditGuideViewModel vm)
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