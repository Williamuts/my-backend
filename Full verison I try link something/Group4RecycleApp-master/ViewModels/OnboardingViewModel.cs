using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Group4RecycleApp.Models;

namespace Group4RecycleApp.ViewModels
{
    public partial class OnboardingViewModel : ObservableObject
    {
        [ObservableProperty]
        private int position;

        public ObservableCollection<OnboardingItem> Items { get; } = new();

        public OnboardingViewModel()
        {
            // Load Slides
            Items.Add(new OnboardingItem { Title = "Reduce", Description = "Pick up wastes and keep your environment clean", Image = "onboarding_reduce.png" });
            Items.Add(new OnboardingItem { Title = "Recycle", Description = "Sort out wastes and get them recycled", Image = "onboarding_recycle.png" });
            Items.Add(new OnboardingItem { Title = "Reuse", Description = "Make good use of recycled wastes and save cash", Image = "onboarding_reuse.png" });
        }

        [RelayCommand]
        void Next()
        {
            if (Position < Items.Count - 1)
            {
                // Next Slide
                Position++;
            }
            else
            {
                // --- FINAL SLIDE LOGIC ---

                // 1. Save the flag so this page never shows again
                Preferences.Default.Set("HasSeenOnboarding", true);

                // 2. Switch the entire Main Page to AppShell (Login Page)
                Application.Current.MainPage = new AppShell();
            }
        }
    }
}