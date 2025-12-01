using Group4RecycleApp.ViewModels;
namespace Group4RecycleApp.Views;

public partial class AccountSettingsPage : ContentPage
{
    public AccountSettingsPage(AccountSettingsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}