using Group4RecycleApp.ViewModels;
namespace Group4RecycleApp.Views;

public partial class HelpSupportPage : ContentPage
{
    public HelpSupportPage(HelpSupportViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}