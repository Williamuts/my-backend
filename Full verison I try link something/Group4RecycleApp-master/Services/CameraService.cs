using System.Diagnostics;

namespace Group4RecycleApp.Services
{
    public class CameraService
    {
        // 检查并请求相机权限
        public async Task<bool> CheckAndRequestCameraPermissionAsync()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.Camera>();

                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.Camera>();
                }

                if (status != PermissionStatus.Granted)
                {
                    await Shell.Current.DisplayAlert(
                        "Permission Required",
                        "Camera permission is needed to take photos. Please enable it in Settings.",
                        "OK");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Camera Permission Error: {ex.Message}");
                return false;
            }
        }

        // 拍照
        public async Task<FileResult> TakePhotoAsync()
        {
            try
            {
                // 先检查权限
                if (!await CheckAndRequestCameraPermissionAsync())
                {
                    return null;
                }

                // 拍照
                var photo = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
                {
                    Title = "Take a photo"
                });

                return photo;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"TakePhoto Error: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Failed to take photo.", "OK");
                return null;
            }
        }

        // 从相册选择照片
        public async Task<FileResult> PickPhotoAsync()
        {
            try
            {
                var photo = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Select a photo"
                });

                return photo;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"PickPhoto Error: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Failed to pick photo.", "OK");
                return null;
            }
        }
    }
}