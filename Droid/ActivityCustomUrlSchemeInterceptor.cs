using System;
using System.Text;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Debug = System.Diagnostics.Debug;
namespace repro.Droid
{
    [Activity(Label = "ActivityCustomUrlSchemeInterceptor", NoHistory = true, LaunchMode = LaunchMode.SingleTop)]
    [IntentFilter
        (
            new[] { Intent.ActionView },
            Categories = new[]
            {
                Intent.CategoryDefault,
                Intent.CategoryBrowsable
            },
            DataSchemes = new[]
            {
                "com.xamarin.traditional.standard.samples.oauth.providers.android",
                "fb1577191872356154",
                "addin"
            },
            //DataHost = "localhost",
            DataHosts = new[]
            {
                "localhost",
                "authorize" // Facebook in fb1889013594699403://authorize 
            },
            DataPaths = new[]
            {
                "/", // Facebook
                "/oauth2redirectpath" // MeetUp
            },
            AutoVerify = true
        )
    ]
    [IntentFilter
        (
            new[] { Intent.ActionView },
            Categories = new[]
            {
                Intent.CategoryDefault,
                Intent.CategoryBrowsable
            },
            DataSchemes = new[]
            {
                "com.googleusercontent.apps.167871008065-o5a0ct6kmubik9d1tmvv1vfq88gflms0"
            },
            DataPaths = new[]
            {
                "/oauth2redirect"
            }
        )
    ]
    public class ActivityCustomUrlSchemeInterceptor : Activity
    {
        private string message;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var uri_android = Intent.Data;

#if DEBUG
            var sb = new StringBuilder();
            sb.AppendLine("ActivityCustomUrlSchemeInterceptor.OnCreate()");
            sb.Append("     uri_android = ").AppendLine(uri_android.ToString());
            Debug.WriteLine(sb.ToString());
#endif

            // Convert iOS NSUrl to C#/netxf/BCL System.Uri - common API
            var uri_netfx = new Uri(uri_android.ToString());

            // load redirect_url Page
            AuthenticationState.Authenticator.OnPageLoading(uri_netfx);

            Finish();
        }
    }
}
