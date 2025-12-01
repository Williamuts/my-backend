using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Group4RecycleApp.Models;

namespace Group4RecycleApp.ViewModels
{
    [QueryProperty(nameof(Session), "Session")]
    public partial class HistoryDetailViewModel : ObservableObject
    {
        [ObservableProperty]
        RecycleSession session;

        [RelayCommand]
        async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}