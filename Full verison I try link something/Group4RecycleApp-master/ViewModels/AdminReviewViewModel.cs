using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Group4RecycleApp.Models;
using Group4RecycleApp.Services;

namespace Group4RecycleApp.ViewModels
{
    public partial class AdminReviewViewModel : ObservableObject
    {
        readonly ApiService _api;

        public ObservableCollection<AdminRequest> Requests { get; } = new();

        public AdminReviewViewModel(ApiService api)
        {
            _api = api;
            _ = LoadRequestsAsync();
        }

        async Task LoadRequestsAsync()
        {
            var list = await _api.GetAdminRequestsAsync();
            Requests.Clear();
            foreach (var r in list)
                Requests.Add(r);
        }

        [RelayCommand]
        public async Task Approve(AdminRequest req)
        {
            if (req == null) return;
            var ok = await _api.ApproveAdminRequestAsync(req.Id);
            if (ok) Requests.Remove(req);
        }

        [RelayCommand]
        public async Task Reject(AdminRequest req)
        {
            if (req == null) return;
            var ok = await _api.RejectAdminRequestAsync(req.Id);
            if (ok) Requests.Remove(req);
        }

        [RelayCommand]
        public async Task Refresh()
        {
            await LoadRequestsAsync();
        }

        [RelayCommand]
        async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}