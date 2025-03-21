using Android.App;
using Android.Content.PM;
using Android.OS;

namespace MaliyetApp
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnStart()
        {
            base.OnStart();
            CheckStoragePermissionAsync();
        }

        private async Task CheckStoragePermissionAsync()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            if (status != PermissionStatus.Granted)
            {
                var result = await Permissions.RequestAsync<Permissions.StorageWrite>();
                if (result != PermissionStatus.Granted)
                {
                    // Kullanıcı izni reddetti
                    Console.WriteLine("Depolama izni reddedildi.");
                }
            }
        }
    }
}
