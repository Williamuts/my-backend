using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Group4RecycleApp.Views;

namespace Group4RecycleApp.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        // 1. 移除預設值 "Ahmad"
        [ObservableProperty]
        string userName;

        public DashboardViewModel()
        {
            // 建構時載入資料
            LoadUserData();
        }

        // 2. 載入並處理名字
        public void LoadUserData()
        {
            // 從 Preferences 讀取全名 (例如: "Ahmad Shah Peng")
            string fullName = Preferences.Get("UserFullName", "User");

            // 為了讓首頁顯示 "Hi, Ahmad" 而不是 "Hi, Ahmad Shah Peng"
            // 我們試著只抓取第一個空格前的字
            if (!string.IsNullOrEmpty(fullName))
            {
                var names = fullName.Split(' ');
                UserName = names.Length > 0 ? names[0] : fullName;
            }
            else
            {
                UserName = "Guest";
            }
        }

        [RelayCommand]
        async Task NavigateToFaq()
        {
            await Shell.Current.GoToAsync(nameof(Views.FaqPage));
        }

        [RelayCommand]
        async Task NavigateToRecycle()
        {
            await Shell.Current.GoToAsync(nameof(Views.RecycleCategoriesPage));
        }

        [RelayCommand]
        async Task NavigateToGuides()
        {
            await Shell.Current.GoToAsync(nameof(Views.RecyclingGuidesPage));
        }

        [RelayCommand]
        async Task NavigateToNearMe()
        {
            // 注意：這裡使用絕對路徑 //LocationPage 是 OK 的
            // 確保 AppShell.xaml 裡有定義這個 Route
            await Shell.Current.GoToAsync("//LocationPage");
        }
    }
}