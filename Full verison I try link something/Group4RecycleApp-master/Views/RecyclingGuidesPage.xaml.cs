using Group4RecycleApp.ViewModels;
namespace Group4RecycleApp.Views;

public partial class RecyclingGuidesPage : ContentPage
{
    public RecyclingGuidesPage(RecyclingGuidesViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}