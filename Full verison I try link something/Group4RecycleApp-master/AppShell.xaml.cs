using Group4RecycleApp.Views;

namespace Group4RecycleApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // --- REGISTER ROUTES FOR SUB-PAGES ---

        // 1. Auth & Onboarding
        Routing.RegisterRoute(nameof(OnboardingPage), typeof(OnboardingPage));
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));

        // 2. Dashboard Sub-Pages
        Routing.RegisterRoute(nameof(FaqPage), typeof(FaqPage));
        // ❌ AdminDashboardPage 已在 AppShell.xaml 定义，不需要注册

        // 3. Recycling Guides Feature
        Routing.RegisterRoute(nameof(RecyclingGuidesPage), typeof(RecyclingGuidesPage));
        Routing.RegisterRoute(nameof(GuideDetailPage), typeof(GuideDetailPage));

        // 4. Recycle Submission Feature
        Routing.RegisterRoute(nameof(RecycleCategoriesPage), typeof(RecycleCategoriesPage));
        Routing.RegisterRoute(nameof(RecycleSubmissionPage), typeof(RecycleSubmissionPage));

        // 5. History Feature
        Routing.RegisterRoute(nameof(HistoryDetailPage), typeof(HistoryDetailPage));

        // 6. Profile Feature
        Routing.RegisterRoute(nameof(AccountSettingsPage), typeof(AccountSettingsPage));
        Routing.RegisterRoute(nameof(HelpSupportPage), typeof(HelpSupportPage));
        Routing.RegisterRoute(nameof(TermsPage), typeof(TermsPage));

        // 7. Location Feature
        Routing.RegisterRoute(nameof(LocationDetailPage), typeof(LocationDetailPage));

        // 8. Admin Feature
        Routing.RegisterRoute(nameof(AdminAddCenterPage), typeof(AdminAddCenterPage));
        Routing.RegisterRoute(nameof(AdminReviewPage), typeof(AdminReviewPage));
        Routing.RegisterRoute(nameof(AdminManageGuidesPage), typeof(AdminManageGuidesPage)); // ✅ 重新添加
        Routing.RegisterRoute(nameof(AdminEditGuidePage), typeof(AdminEditGuidePage));
    }
}