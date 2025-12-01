using Group4RecycleApp.ViewModels;
namespace Group4RecycleApp.Views;

public partial class HistoryPage : ContentPage
{
    public HistoryPage(HistoryViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}