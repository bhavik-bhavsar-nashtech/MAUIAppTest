using System.IO;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Media;
using Microsoft.Extensions.DependencyInjection;

namespace MAUIAppTest;

public partial class DeviceCapabilityPage : ContentPage
{
    private readonly Services.ErrorHandlingService? _eh;

    public DeviceCapabilityPage(Services.ErrorHandlingService errorHandler) : base()
    {
        InitializeComponent();
        _eh = errorHandler;
    }

    // Constructor with no parameters is removed to ensure only DI is used
    // XAML-defined controls will be used directly
    // Ensure to use named controls from XAML for event handling

    private async void TakePhoto_Clicked(object sender, EventArgs e)
    {
        try
        {
            var status = await Permissions.RequestAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("Permission", "Camera permission denied", "OK");
                return;
            }

            var photo = await MediaPicker.Default.CapturePhotoAsync();
            if (photo is null) return;

            // Save the captured photo to app data and display it
            using var sourceStream = await photo.OpenReadAsync();
            var fileName = photo.FileName;
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = $"photo_{DateTime.UtcNow:yyyyMMddHHmmss}.jpg";

            var destPath = Path.Combine(FileSystem.AppDataDirectory, fileName);
            // Overwrite if exists
            using (var destStream = File.Create(destPath))
            {
                await sourceStream.CopyToAsync(destStream);
            }

            PhotoImage.Source = ImageSource.FromFile(destPath);
            PhotoPathLabel.Text = destPath;

            if (_eh != null) await _eh.LogMessageAsync($"Photo captured and saved: {destPath}");
        }
        catch (Exception ex)
        {
            if (_eh != null) await _eh.LogExceptionAsync(ex, "TakePhoto_Clicked");
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async void GetLocation_Clicked(object sender, EventArgs e)
    {
        try
        {
            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("Permission", "Location permission denied", "OK");
                return;
            }

            var location = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium));
            if (location is null)
            {
                LocationLabel.Text = "Unable to get location";
            }
            else
            {
                LocationLabel.Text = $"Lat: {location.Latitude}, Lon: {location.Longitude}, Alt: {location.Altitude}";
            }

            if (_eh != null) await _eh.LogMessageAsync("Location retrieved via DeviceCapabilityPage");
        }
        catch (Exception ex)
        {
            if (_eh != null) await _eh.LogExceptionAsync(ex, "GetLocation_Clicked");
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private void StartAccel_Clicked(object sender, EventArgs e)
    {
        try
        {
            Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
            Accelerometer.Start(SensorSpeed.UI);
        }
        catch (Exception ex)
        {
            _ = _eh?.LogExceptionAsync(ex, "StartAccel_Clicked");
        }
    }

    private void StopAccel_Clicked(object sender, EventArgs e)
    {
        try
        {
            Accelerometer.ReadingChanged -= Accelerometer_ReadingChanged;
            Accelerometer.Stop();
        }
        catch (Exception ex)
        {
            _ = _eh?.LogExceptionAsync(ex, "StopAccel_Clicked");
        }
    }

    private void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
    {
        var data = e.Reading;
        AccelLabel.Text = $"X:{data.Acceleration.X:F2} Y:{data.Acceleration.Y:F2} Z:{data.Acceleration.Z:F2}";
    }

    private async void SaveSecure_Clicked(object sender, EventArgs e)
    {
        try
        {
            await SecureStorage.Default.SetAsync("demo_key", "secret_value");
            SecureLabel.Text = "Saved";
        }
        catch (Exception ex)
        {
            _ = _eh?.LogExceptionAsync(ex, "SaveSecure_Clicked");
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async void ReadSecure_Clicked(object sender, EventArgs e)
    {
        try
        {
            var val = await SecureStorage.Default.GetAsync("demo_key");
            SecureLabel.Text = val ?? "(not found)";
        }
        catch (Exception ex)
        {
            _ = _eh?.LogExceptionAsync(ex, "ReadSecure_Clicked");
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}
