using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Group4RecycleApp.Models;
using Group4RecycleApp.Services;

namespace Group4RecycleApp.ViewModels
{
    public partial class HistoryViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        ObservableCollection<RecycleSession> historyList = new();

        [ObservableProperty]
        int totalItemsRecycled;

        [ObservableProperty]
        int treesSaved;

        [ObservableProperty]
        bool isBusy;

        // 注入 ApiService
        public HistoryViewModel(ApiService apiService)
        {
            _apiService = apiService;
            LoadHistoryData();
        }

        // 从 API 加载真实数据
        async void LoadHistoryData()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                // 🔹 调用后端 API 获取回收历史记录
                var sessions = await _apiService.GetRecyclingHistoryAsync();

                HistoryList.Clear();

                int totalItems = 0;

                foreach (var session in sessions)
                {
                    HistoryList.Add(session);
                    totalItems += session.TotalItems;
                }

                // 计算统计数据
                TotalItemsRecycled = totalItems;
                TreesSaved = CalculateTreesSaved(totalItems);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadHistoryData Error: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Unable to load recycling history.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // 计算拯救的树木数量
        int CalculateTreesSaved(int totalItems)
        {
            // 示例：每回收 10 件物品 = 拯救 1 棵树
            return totalItems / 10;
        }

        [RelayCommand]
        async Task ViewDetails(RecycleSession session)
        {
            if (session == null) return;

            var navParam = new Dictionary<string, object>
            {
                { "Session", session }
            };

            await Shell.Current.GoToAsync(nameof(Views.HistoryDetailPage), navParam);
        }

        // 🔹 添加刷新功能（可选）
        [RelayCommand]
        async Task Refresh()
        {
            await Task.Run(() => LoadHistoryData());
        }
    }
}