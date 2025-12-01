using Group4RecycleApp.ViewModels;

namespace Group4RecycleApp.Views;

public partial class AdminReviewPage : ContentPage
{
    public AdminReviewPage(AdminReviewViewModel vm)
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