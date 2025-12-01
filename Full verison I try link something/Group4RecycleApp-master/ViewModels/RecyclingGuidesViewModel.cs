using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Group4RecycleApp.Models;
using Group4RecycleApp.Services; // 記得引用 Services
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Group4RecycleApp.ViewModels
{
    public partial class RecyclingGuidesViewModel : ObservableObject
    {
        // 1. 宣告 ApiService
        private readonly ApiService _apiService;

        public ObservableCollection<RecyclingGuide> Guides { get; } = new();

        [ObservableProperty]
        bool isBusy;

        // 2. 修改建構函式，注入 ApiService
        public RecyclingGuidesViewModel(ApiService apiService)
        {
            _apiService = apiService;

            // 一進來就去抓資料
            LoadGuides();
        }

        private async void LoadGuides()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                // 3. 呼叫後端 API
                var backendGuides = await _apiService.GetRecyclingGuidesAsync();

                Guides.Clear();

                // 4. 定義一些顏色 (因為後端可能沒存顏色，我們在前端輪流分配)
                string[] colors = { "#6F85FD", "#4CAF50", "#2196F3", "#9E9E9E", "#FF9800" };
                int colorIndex = 0;

                foreach (var item in backendGuides)
                {
                    var guide = new RecyclingGuide
                    {
                        Id = item.Id,
                        Title = item.Title,

                        // 👇 [修改 1] 左邊原本是 Icon，現在要改成 ImageUrl
                        ImageUrl = item.ImageUrl,

                        // 👇 右邊也要對應後端傳過來的屬性 (如果後端是 ImageUrl 就用 ImageUrl)
                        // Icon = item.ImageUrl, (這行刪掉)

                        VideoUrl = item.VideoUrl,

                        // 👇 [修改 2] 左邊原本是 JournalUrl，現在要改成 ArticleUrl
                        ArticleUrl = item.ArticleUrl,

                        CreditName = "RecycleRight",
                        Color = colors[colorIndex % colors.Length]
                    };

                    Guides.Add(guide);
                    colorIndex++;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadGuides Error: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Unable to load guides.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task OpenGuide(RecyclingGuide guide)
        {
            if (guide == null) return;

            // 導航到詳細頁面，並把整個 guide 物件傳過去
            var navParam = new Dictionary<string, object>
            {
                { "Guide", guide }
            };
            await Shell.Current.GoToAsync(nameof(Views.GuideDetailPage), navParam);
        }

        [RelayCommand]
        async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}