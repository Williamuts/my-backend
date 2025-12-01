using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Group4RecycleApp.Views;
using Group4RecycleApp.Services; // 引用 Services 命名空間

namespace Group4RecycleApp.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        // 宣告 Service 變數
        private readonly ApiService _apiService;

        // 建立建構函式 (Constructor) 進行注入
        public LoginViewModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        [ObservableProperty]
        string fullName;

        [ObservableProperty]
        string email;

        [ObservableProperty]
        string password;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsSignUpMode))]
        [NotifyPropertyChangedFor(nameof(MainButtonText))]
        [NotifyPropertyChangedFor(nameof(LoginTabColor))]
        [NotifyPropertyChangedFor(nameof(SignUpTabColor))]
        [NotifyPropertyChangedFor(nameof(LoginIndicatorVisible))]
        [NotifyPropertyChangedFor(nameof(SignUpIndicatorVisible))]
        bool isLoginMode = true;

        // --- Helper Properties ---
        public bool IsSignUpMode => !IsLoginMode;
        public string MainButtonText => IsLoginMode ? "Login" : "Create account";
        public Color LoginTabColor => IsLoginMode ? Color.FromArgb("#00C569") : Colors.Gray;
        public Color SignUpTabColor => !IsLoginMode ? Color.FromArgb("#00C569") : Colors.Gray;
        public bool LoginIndicatorVisible => IsLoginMode;
        public bool SignUpIndicatorVisible => !IsLoginMode;

        // --- Commands ---
        [RelayCommand]
        void SwitchToLogin() => IsLoginMode = true;

        [RelayCommand]
        void SwitchToSignUp() => IsLoginMode = false;

        [RelayCommand]
        async Task ForgotPassword()
        {
            if (string.IsNullOrEmpty(Email))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter your email address first.", "OK");
                return;
            }

            // In a real app, this would call an API to send a reset email
            await Shell.Current.DisplayAlert("Reset Password", $"A password reset link has been sent to {Email}", "OK");
        }

        [RelayCommand]
        async Task MainAction()
        {
            if (IsLoginMode)
            {
                // --- 登入邏輯 ---

                // 1. 保留原本的 Admin 後門 (方便測試用)
                if (!string.IsNullOrWhiteSpace(Email) && Email == "admin" && Password == "admin123")
                {
                    await Shell.Current.GoToAsync("//AdminDashboardRoute");
                    return;
                }

                // 2. 檢查輸入是否為空
                if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                {
                    await Shell.Current.DisplayAlert("Error", "Please enter email and password", "OK");
                    return;
                }

                try
                {
                    // 3. 呼叫 API 登入
                    bool isSuccess = await _apiService.LoginAsync(Email, Password);

                    if (isSuccess)
                    {
                        // 4. 判斷角色並導航
                        string userRole = Preferences.Get("UserRole", "User");

                        if (userRole == "Admin")
                        {
                            await Shell.Current.GoToAsync("//AdminDashboardPage");
                        }
                        else
                        {
                            await Shell.Current.GoToAsync("//MainTabs");
                        }
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Failed", "Invalid username or password", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Error", $"Connection failed: {ex.Message}", "OK");
                }
            }
            else
            {
                // --- 註冊邏輯 (Sign Up) ---

                // 1. 檢查輸入
                if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                {
                    await Shell.Current.DisplayAlert("Error", "Please fill in all fields", "OK");
                    return;
                }

                // 2. 呼叫 API 註冊
                bool isSuccess = await _apiService.RegisterAsync(Email, Password, FullName);

                if (isSuccess)
                {
                    await Shell.Current.DisplayAlert("Success", "Account created successfully! Please login.", "OK");

                    // 3. 註冊成功後，自動切換回登入模式，方便用戶直接登入
                    SwitchToLogin();
                }
                else
                {
                    // 👇 [修改] 如果注册失败（通常是因为账号已存在），询问是否直接去登录
                    bool answer = await Shell.Current.DisplayAlert("Error", "Registration failed. Email might already be taken.\nDo you want to login instead?", "Yes", "No");
                    if (answer)
                    {
                        SwitchToLogin(); // 自动切回登录页
                    }
                }
            }
        }
    }
}