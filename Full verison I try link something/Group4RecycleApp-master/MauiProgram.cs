using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using Group4RecycleApp.Views;
using Group4RecycleApp.ViewModels;
using Group4RecycleApp.Services;

// 👇 引入 Android 命名空间 (只在 Android 编译时有效)
#if ANDROID
using Android.Webkit;
using Microsoft.Maui.Handlers;
#endif

namespace Group4RecycleApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitMediaElement()
            .UseMauiMaps()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // =========================================================
        // 👇👇👇 [核心修改] Android WebView 终极配置 👇👇👇
        // =========================================================
#if ANDROID
        WebViewHandler.Mapper.AppendToMapping("FixYouTube", (handler, view) =>
        {
            if (handler.PlatformView is Android.Webkit.WebView nativeWebView)
            {
                var settings = nativeWebView.Settings;

                // 1. 基础设置
                settings.JavaScriptEnabled = true;
                settings.DomStorageEnabled = true;
                settings.DatabaseEnabled = true;

                // 2. 允许自动播放
                settings.MediaPlaybackRequiresUserGesture = false;

                // 3. 允许混合内容 (解决部分资源加载失败问题)
                settings.MixedContentMode = Android.Webkit.MixedContentHandling.AlwaysAllow;

                // 4. [必杀技] 伪装成桌面版 Chrome (Windows)
                settings.UserAgentString =
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";
            }
        });
#endif
        // =========================================================

        // --- Services ---
        builder.Services.AddSingleton<ApiService>();
        builder.Services.AddSingleton<CameraService>(); // 🔹 添加这行

        // --- Pages & ViewModels ---
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<DashboardViewModel>();

        builder.Services.AddTransient<RecyclingGuidesPage>();
        builder.Services.AddTransient<RecyclingGuidesViewModel>();

        builder.Services.AddTransient<GuideDetailPage>();
        builder.Services.AddTransient<GuideDetailViewModel>();

        builder.Services.AddTransient<AdminDashboardPage>();
        builder.Services.AddTransient<AdminDashboardViewModel>();
        builder.Services.AddTransient<AdminManageGuidesPage>();
        builder.Services.AddTransient<AdminManageGuidesViewModel>();
        builder.Services.AddTransient<AdminEditGuidePage>();
        builder.Services.AddTransient<AdminEditGuideViewModel>();
        builder.Services.AddTransient<AdminAddCenterPage>();
        builder.Services.AddTransient<AdminAddCenterViewModel>();
        builder.Services.AddTransient<AdminReviewPage>();
        builder.Services.AddTransient<AdminReviewViewModel>();
        builder.Services.AddTransient<FaqPage>();
        builder.Services.AddTransient<FaqViewModel>();
        builder.Services.AddTransient<RecycleCategoriesPage>();
        builder.Services.AddTransient<RecycleCategoriesViewModel>();
        builder.Services.AddTransient<RecycleSubmissionPage>();
        builder.Services.AddTransient<RecycleSubmissionViewModel>();
        builder.Services.AddTransient<HistoryPage>();
        builder.Services.AddTransient<HistoryViewModel>();
        builder.Services.AddTransient<HistoryDetailPage>();
        builder.Services.AddTransient<HistoryDetailViewModel>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<AccountSettingsPage>();
        builder.Services.AddTransient<AccountSettingsViewModel>();
        builder.Services.AddTransient<HelpSupportPage>();
        builder.Services.AddTransient<HelpSupportViewModel>();
        builder.Services.AddTransient<LocationPage>();
        builder.Services.AddTransient<LocationViewModel>();
        builder.Services.AddTransient<LocationDetailPage>();
        builder.Services.AddTransient<LocationDetailViewModel>();
        builder.Services.AddTransient<TermsPage>();
        builder.Services.AddTransient<OnboardingPage>();
        builder.Services.AddTransient<OnboardingViewModel>();

        return builder.Build();
    }
}