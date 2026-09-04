using System;
using System.IO;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Media;
using Microsoft.Extensions.DependencyInjection;
#nullable enable
using ZXing;
using ZXing.Common;
#if ANDROID
using Android.Graphics;
#endif

namespace MAUIAppTest;

public partial class DeviceCapabilityPage : ContentPage
{
    private readonly Services.ErrorHandlingService? _eh;

    public DeviceCapabilityPage(Services.ErrorHandlingService errorHandler)
    {
        InitializeComponent();
        _eh = errorHandler;
    }

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

            using var sourceStream = await photo.OpenReadAsync();
            var fileName = photo.FileName;
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = $"photo_{DateTime.UtcNow:yyyyMMddHHmmss}.jpg";

            var destPath = System.IO.Path.Combine(FileSystem.AppDataDirectory, fileName);
            using (var destStream = File.Create(destPath))
            {
                await sourceStream.CopyToAsync(destStream);
            }

            PhotoImage.Source = ImageSource.FromFile(destPath);
            PhotoPathLabel.Text = destPath;

            if (_eh != null) await _eh.LogMessageAsync($"Photo captured and saved: {destPath}");
            // Clear previous scan result
            ScanResultLabel.Text = string.Empty;
        }
        catch (Exception ex)
        {
            if (_eh != null) await _eh.LogExceptionAsync(ex, "TakePhoto_Clicked");
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async void ScanFromImage_Clicked(object sender, EventArgs e)
    {
        try
        {
            var path = PhotoPathLabel.Text;
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                await DisplayAlert("Scan", "No saved photo to scan. Capture an image first.", "OK");
                return;
            }

#if ANDROID
            var bitmap = BitmapFactory.DecodeFile(path);
            if (bitmap is null)
            {
                await DisplayAlert("Scan", "Failed to load image for scanning", "OK");
                return;
            }

            try
            {
                int width = bitmap.Width;
                int height = bitmap.Height;
                var pixels = new int[width * height];
                bitmap.GetPixels(pixels, 0, width, 0, 0, width, height);

                // Convert ARGB int[] to RGB byte[] (R,G,B per pixel)
                var rgb = new byte[width * height * 3];
                for (int i = 0, j = 0; i < pixels.Length; i++, j += 3)
                {
                    int p = pixels[i];
                    rgb[j] = (byte)((p >> 16) & 0xFF);
                    rgb[j + 1] = (byte)((p >> 8) & 0xFF);
                    rgb[j + 2] = (byte)(p & 0xFF);
                }

                var luminanceSource = new RGBLuminanceSource(rgb, width, height);
                var binaryBitmap = new BinaryBitmap(new HybridBinarizer(luminanceSource));
                var reader = new MultiFormatReader();
                var result = reader.decode(binaryBitmap);

                ScanResultLabel.Text = result?.Text ?? "No barcode/QR found";
                if (_eh != null) await _eh.LogMessageAsync($"Scan result: {ScanResultLabel.Text}");
            }
            finally
            {
                bitmap.Recycle();
                bitmap.Dispose();
            }
#else
            await DisplayAlert("Scan", "Image scanning currently supported on Android builds only.", "OK");
#endif
        }
        catch (Exception ex)
        {
            _ = _eh?.LogExceptionAsync(ex, "ScanFromImage_Clicked");
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
