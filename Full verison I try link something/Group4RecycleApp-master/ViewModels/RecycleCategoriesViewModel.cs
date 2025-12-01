using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Group4RecycleApp.Models;

namespace Group4RecycleApp.ViewModels
{
    public partial class RecycleCategoriesViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<RecycleCategory> categories = new();

        [ObservableProperty]
        RecycleCategory selectedCategory;

        public RecycleCategoriesViewModel()
        {
            // Load Categories
            Categories.Add(new RecycleCategory { Name = "Plastic", Icon = "cat_plastic.png" });
            Categories.Add(new RecycleCategory { Name = "Glass", Icon = "cat_glass.png" });
            Categories.Add(new RecycleCategory { Name = "Clothes", Icon = "cat_clothes.png" });
            Categories.Add(new RecycleCategory { Name = "Paper", Icon = "cat_paper.png" });
            Categories.Add(new RecycleCategory { Name = "Metal", Icon = "cat_metal.png" });
        }

        [RelayCommand]
        void SelectCategory(RecycleCategory item)
        {
            // 1. Reset ALL categories to default state
            foreach (var cat in Categories)
            {
                cat.IsSelected = false;
                cat.Color = "White";           // Default Background
                cat.BorderColor = "LightGray"; // Default Border
                cat.BorderWidth = 1;           // Thin Border
            }

            // 2. Highlight the SELECTED one
            item.IsSelected = true;
            item.Color = "#D1F2EB";       // Light Green Background
            item.BorderColor = "#00C569"; // Strong Green Border
            item.BorderWidth = 3;         // Thicker Border

            SelectedCategory = item;
        }

        [RelayCommand]
        async Task Next()
        {
            if (SelectedCategory == null)
            {
                await Shell.Current.DisplayAlert("Alert", "Please select a category first.", "OK");
                return;
            }

            var navParam = new Dictionary<string, object>
            {
                { "Category", SelectedCategory }
            };
            await Shell.Current.GoToAsync(nameof(Views.RecycleSubmissionPage), navParam);
        }

        [RelayCommand]
        async Task NavigateToGuides()
        {
            
            await Shell.Current.GoToAsync(nameof(Views.RecyclingGuidesPage));
        }

        [RelayCommand]
        async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}