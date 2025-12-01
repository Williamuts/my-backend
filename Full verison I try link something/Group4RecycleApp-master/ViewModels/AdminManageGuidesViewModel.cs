using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Group4RecycleApp.Models;
using Group4RecycleApp.Services; // 1. 引用 Services
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Group4RecycleApp.ViewModels
{
    public partial class AdminManageGuidesViewModel : ObservableObject
    {
        // 2. 宣告 ApiService
        private readonly ApiService _apiService;

        public ObservableCollection<RecyclingGuide> Guides { get; } = new();

        [ObservableProperty]
        bool isBusy;

        // 3. 修改建構函式，注入 ApiService
        public AdminManageGuidesViewModel(ApiService apiService)
        {
            _apiService = apiService;

            // 一進來就載入資料
            LoadGuides();
        }

        public async void LoadGuides()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                // 4. 從後端抓取真實資料
                var backendGuides = await _apiService.GetRecyclingGuidesAsync();

                Guides.Clear();

                // 顏色陣列 (用來裝飾列表)
                string[] colors = { "#6F85FD", "#4CAF50", "#2196F3", "#9E9E9E", "#FF9800" };
                int colorIndex = 0;

                foreach (var item in backendGuides)
                {
                    // 5. 將後端資料轉換為物件 (注意屬性名稱要對應新的 Model)
                    var guide = new RecyclingGuide
                    {
                        Id = item.Id,
                        Title = item.Title,

                        // 👇 修改：使用 ImageUrl
                        ImageUrl = item.ImageUrl,

                        // 👇 修改：使用 VideoUrl
                        VideoUrl = item.VideoUrl,

                        // 👇 修改：使用 ArticleUrl
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
        async Task EditGuide(RecyclingGuide guide)
        {
            if (guide == null) return;

            // 導航到編輯頁面，並傳遞選中的 guide
            var navParam = new Dictionary<string, object> { { "Guide", guide } };
            await Shell.Current.GoToAsync(nameof(Views.AdminEditGuidePage), navParam);
        }

        [RelayCommand]
        async Task GoBack()
        {
            // 建議統一用這個方式返回
            await Shell.Current.GoToAsync("..");
        }
    }
}