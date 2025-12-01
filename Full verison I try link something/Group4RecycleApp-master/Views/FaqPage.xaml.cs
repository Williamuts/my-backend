using Group4RecycleApp.ViewModels;
namespace Group4RecycleApp.Views;

public partial class FaqPage : ContentPage
{
    public FaqPage(FaqViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}