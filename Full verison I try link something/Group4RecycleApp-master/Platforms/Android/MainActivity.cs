using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Webkit;

namespace Group4RecycleApp
{
    [Activity(
        Theme = "@style/Maui.SplashTheme",
        MainLauncher = true,
        LaunchMode = LaunchMode.SingleTop,
        ConfigurationChanges = ConfigChanges.ScreenSize
                             | ConfigChanges.Orientation
                             | ConfigChanges.UiMode
                             | ConfigChanges.ScreenLayout
                             | ConfigChanges.SmallestScreenSize
                             | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Force Chrome-like user-agent (fix YouTube Error 4)
            string chromeUA =
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                "AppleWebKit/537.36 (KHTML, like Gecko) " +
                "Chrome/120.0.0.0 Safari/537.36";

            try
            {
                // Create a dummy WebView to apply global UA
                var dummyWebView = new Android.Webkit.WebView(this);
                dummyWebView.Settings.UserAgentString = chromeUA;
            }
            catch
            {
                // ignore errors (just in case)
            }
        }
    }
}
