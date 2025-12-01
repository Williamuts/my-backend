using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Group4RecycleApp.ViewModels
{
    public partial class ProfileViewModel : ObservableObject
    {
        // 1. 移除預設值 (Hardcoded data)
        [ObservableProperty]
        string userName;

        [ObservableProperty]
        string userEmail;

        public ProfileViewModel()
        {
            // 2. 建構函式：一進來就載入資料
            LoadUserData();
        }

        // 3. 讀取資料的方法 (設為 Public，方便 View 呼叫)
        public void LoadUserData()
        {
            // 讀取登入時存下來的 Key
            UserName = Preferences.Get("UserFullName", "Guest User");
            UserEmail = Preferences.Get("UserEmail", "");
        }

        [RelayCommand]
        async Task NavigateToSettings()
        {
            await Shell.Current.GoToAsync(nameof(Views.AccountSettingsPage));
        }

        [RelayCommand]
        async Task NavigateToSupport()
        {
            await Shell.Current.GoToAsync(nameof(Views.HelpSupportPage));
        }

        [RelayCommand]
        async Task Logout()
        {
            bool answer = await Shell.Current.DisplayAlert("Logout", "Are you sure you want to logout?", "Yes", "No");
            if (answer)
            {
                // 4. [重要] 登出時清除手機裡存的資料
                Preferences.Clear();
                // 或者只清除特定的：Preferences.Remove("AuthToken");

                // Go back to Login Page (Absolute route)
                await Shell.Current.GoToAsync("//LoginPage");
            }
        }

        [RelayCommand]
        async Task BackToHome()
        {
            // Go to Dashboard (Home tab)
            await Shell.Current.GoToAsync("//DashboardPage");
        }
    }
}