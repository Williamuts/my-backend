using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Group4RecycleApp.Models;

namespace Group4RecycleApp.ViewModels
{
    [QueryProperty(nameof(Guide), "Guide")]
    public partial class GuideDetailViewModel : ObservableObject
    {
        [ObservableProperty]
        RecyclingGuide guide;

        [RelayCommand]
        async Task OpenJournal()
        {
            if (Guide != null && !string.IsNullOrEmpty(Guide.ArticleUrl))
            {
                // Open link in external browser
                await Launcher.Default.OpenAsync(new Uri(Guide.ArticleUrl));
            }
        }

        [RelayCommand]
        async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}