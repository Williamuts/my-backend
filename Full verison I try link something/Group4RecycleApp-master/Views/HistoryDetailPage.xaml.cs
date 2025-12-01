using Group4RecycleApp.ViewModels;
namespace Group4RecycleApp.Views;

public partial class HistoryDetailPage : ContentPage
{
    public HistoryDetailPage(HistoryDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}