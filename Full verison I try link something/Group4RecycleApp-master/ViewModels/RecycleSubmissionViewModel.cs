using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Group4RecycleApp.Models;
using Group4RecycleApp.Services;
using System.Diagnostics;

namespace Group4RecycleApp.ViewModels
{
    // Receives data from the previous page
    [QueryProperty(nameof(Category), "Category")]
    public partial class RecycleSubmissionViewModel : ObservableObject
    {
        // 🔹 注入 CameraService
        private readonly CameraService _cameraService;

        [ObservableProperty]
        RecycleCategory category;

        [ObservableProperty]
        string locationName;

        [ObservableProperty]
        string address;

        [ObservableProperty]
        string description;

        [ObservableProperty]
        ImageSource photoPreview;

        // 🔹 添加构造函数，注入 CameraService
        public RecycleSubmissionViewModel(CameraService cameraService)
        {
            _cameraService = cameraService;
        }

        // --- Feature E3: Location Services (GPS) ---
        [RelayCommand]
        async Task GetCurrentLocation()
        {
            try
            {
                // 🔹 先检查位置权限
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }

                if (status != PermissionStatus.Granted)
                {
                    await Shell.Current.DisplayAlert("Permission Required",
                        "Location permission is needed to get your current location.", "OK");
                    return;
                }

                // 1. Try to get last known location (faster)
                var location = await Geolocation.Default.GetLastKnownLocationAsync();

                // 2. If null, request new location (more accurate)
                if (location == null)
                {
                    location = await Geolocation.Default.GetLocationAsync(
                        new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10)));
                }

                if (location != null)
                {
                    Address = $"Lat: {location.Latitude:F6}, Long: {location.Longitude:F6}";
                    LocationName = "Current GPS Location";
                    await Shell.Current.DisplayAlert("Success", "Location obtained!", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Unable to get location. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GPS Error: {ex.Message}");
                await Shell.Current.DisplayAlert("GPS Error",
                    "Make sure location is turned on in your device settings.", "OK");
            }
        }

        // --- Feature E4: Multimedia (Camera) ---
        [RelayCommand]
        async Task TakePhoto()
        {
            try
            {
                // 🔹 使用 CameraService 拍照（已包含权限检查）
                var photo = await _cameraService.TakePhotoAsync();

                if (photo != null)
                {
                    // Save locally to display it
                    var localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                    using var stream = await photo.OpenReadAsync();
                    using var newStream = File.OpenWrite(localFilePath);
                    await stream.CopyToAsync(newStream);

                    PhotoPreview = ImageSource.FromFile(localFilePath);

                    await Shell.Current.DisplayAlert("Success", "Photo captured successfully!", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"TakePhoto Error: {ex.Message}");
                await Shell.Current.DisplayAlert("Error",
                    "Failed to take photo. Please try again.", "OK");
            }
        }

        // 🔹 可选：添加从相册选择照片的功能
        [RelayCommand]
        async Task PickPhoto()
        {
            try
            {
                var photo = await _cameraService.PickPhotoAsync();

                if (photo != null)
                {
                    var localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                    using var stream = await photo.OpenReadAsync();
                    using var newStream = File.OpenWrite(localFilePath);
                    await stream.CopyToAsync(newStream);

                    PhotoPreview = ImageSource.FromFile(localFilePath);

                    await Shell.Current.DisplayAlert("Success", "Photo selected successfully!", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"PickPhoto Error: {ex.Message}");
                await Shell.Current.DisplayAlert("Error",
                    "Failed to pick photo. Please try again.", "OK");
            }
        }

        [RelayCommand]
        async Task Submit()
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(LocationName))
            {
                await Shell.Current.DisplayAlert("Error", "Please select a location.", "OK");
                return;
            }

            if (PhotoPreview == null)
            {
                await Shell.Current.DisplayAlert("Error", "Please take or select a photo.", "OK");
                return;
            }

            // Here you would save to database
            await Shell.Current.DisplayAlert("Success", "Recycling request submitted!", "OK");

            // Go back to Dashboard
            await Shell.Current.GoToAsync("//MainTabs/DashboardPage");
        }

        [RelayCommand]
        async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}