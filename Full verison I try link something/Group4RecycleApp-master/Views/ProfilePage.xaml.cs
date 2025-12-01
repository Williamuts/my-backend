using Group4RecycleApp.ViewModels;

namespace Group4RecycleApp.Views;

public partial class ProfilePage : ContentPage
{
    public ProfilePage(ProfileViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    // 👇👇👇 [新增這段] 每次頁面顯示時執行 👇👇👇
    protected override void OnAppearing()
    {
        base.OnAppearing();

        // 檢查 BindingContext 是不是 ProfileViewModel，如果是，就刷新資料
        if (BindingContext is ProfileViewModel vm)
        {
            vm.LoadUserData();
        }
    }
}