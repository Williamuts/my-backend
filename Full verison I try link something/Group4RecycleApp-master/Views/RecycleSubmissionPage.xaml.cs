using Group4RecycleApp.ViewModels;
namespace Group4RecycleApp.Views;

public partial class RecycleSubmissionPage : ContentPage
{
    public RecycleSubmissionPage(RecycleSubmissionViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}