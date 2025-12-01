using Group4RecycleApp.Views;
using Group4RecycleApp.ViewModels;

namespace Group4RecycleApp;

public partial class App : Application
{
    public App(IServiceProvider services)
    {
        InitializeComponent();

        // 1. Check if user has seen onboarding
        bool hasSeenOnboarding = Preferences.Default.Get("HasSeenOnboarding", false);

        if (hasSeenOnboarding)
        {
            // If YES, go straight to the Main App (Login)
            MainPage = new AppShell();
        }
        else
        {
            // If NO, show Onboarding Page (Standalone)
            var onboardingVM = services.GetService<OnboardingViewModel>();
            MainPage = new OnboardingPage(onboardingVM);
        }
    }
}