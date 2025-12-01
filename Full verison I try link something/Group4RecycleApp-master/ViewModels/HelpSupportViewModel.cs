using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Group4RecycleApp.ViewModels
{
    public partial class HelpSupportViewModel : ObservableObject
    {
        [RelayCommand]
        async Task ContactSupport()
        {
            // Open Email App
            await Launcher.Default.OpenAsync(new Uri("mailto:support@recycleapp.com"));
        }

        [RelayCommand]
        async Task OpenTerms()
        {
            await Shell.Current.GoToAsync(nameof(Views.TermsPage));
        }

        [RelayCommand]
        async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}