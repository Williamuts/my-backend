using Group4RecycleApp.ViewModels;
namespace Group4RecycleApp.Views;

public partial class RecycleCategoriesPage : ContentPage
{
    public RecycleCategoriesPage(RecycleCategoriesViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}