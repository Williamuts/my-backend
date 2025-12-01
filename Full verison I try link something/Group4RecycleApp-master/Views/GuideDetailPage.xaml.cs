using Group4RecycleApp.ViewModels;

namespace Group4RecycleApp.Views;

public partial class GuideDetailPage : ContentPage
{

    public GuideDetailPage(GuideDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}