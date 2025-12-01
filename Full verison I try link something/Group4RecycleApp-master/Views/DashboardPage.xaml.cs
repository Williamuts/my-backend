using Group4RecycleApp.ViewModels;

namespace Group4RecycleApp.Views;

public partial class DashboardPage : ContentPage
{
    public DashboardPage(DashboardViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    // 👇 加這段：每次切換到首頁時，重新讀取名字
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is DashboardViewModel vm)
        {
            vm.LoadUserData();
        }
    }
}