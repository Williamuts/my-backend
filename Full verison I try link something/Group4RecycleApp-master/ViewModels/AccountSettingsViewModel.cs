using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Group4RecycleApp.Services; // [1] 引用 Services

namespace Group4RecycleApp.ViewModels
{
    public partial class AccountSettingsViewModel : ObservableObject
    {
        // [2] 宣告 Service 變數
        private readonly ApiService _apiService;

        [ObservableProperty]
        string fullName;

        [ObservableProperty]
        string email;

        [ObservableProperty]
        string phoneNumber;

        [ObservableProperty]
        string address;

        // [3] 修改建構函式：注入 ApiService
        public AccountSettingsViewModel(ApiService apiService)
        {
            _apiService = apiService;

            // 當這個頁面被建立時，自動執行載入資料
            LoadUserData();
        }

        // 從手機緩存讀取資料
        private void LoadUserData()
        {
            // 使用 Preferences.Get("Key", "預設值")
            FullName = Preferences.Get("UserFullName", "");
            Email = Preferences.Get("UserEmail", "");
            PhoneNumber = Preferences.Get("UserPhone", "");
            Address = Preferences.Get("UserAddress", "");
        }

        [RelayCommand]
        async Task SaveChanges()
        {
            // 檢查網路或基本驗證 (可選)
            if (string.IsNullOrWhiteSpace(FullName))
            {
                await Shell.Current.DisplayAlert("Error", "Full Name cannot be empty.", "OK");
                return;
            }

            // [4] 真正呼叫後端 API
            // 注意：不需要傳 Email，因為 Token 裡已經包含了身份信息
            bool isSuccess = await _apiService.UpdateProfileAsync(FullName, PhoneNumber, Address);

            if (isSuccess)
            {
                // [5] 後端更新成功後，才同步更新手機本地的緩存
                // 這樣使用者下次進來看到的才會是新的
                Preferences.Set("UserFullName", FullName);
                Preferences.Set("UserPhone", PhoneNumber);
                Preferences.Set("UserAddress", Address);

                await Shell.Current.DisplayAlert("Success", "Profile updated successfully!", "OK");

                // 返回上一頁
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                // 如果失敗 (例如 Token 過期或網路斷線)
                await Shell.Current.DisplayAlert("Error", "Failed to update profile. Please check your connection.", "OK");
            }
        }

        [RelayCommand]
        async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}