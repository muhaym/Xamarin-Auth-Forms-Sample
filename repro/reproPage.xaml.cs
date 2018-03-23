using Xamarin.Forms;

namespace repro
{
    public partial class reproPage : ContentPage
    {
        SocialServices social = new SocialServices();
        async void Facebook_Clicked(object sender, System.EventArgs e)
        {
            var res = await social.LoginAsync(AuthProviders.Facebook, true);
            if (res != null)
            {
            }
            else
            {
                await DisplayAlert("Cancelled", "Action was cancelled by user", "OK");
            }
        }
        async void Twitter_Clicked(object sender, System.EventArgs e)
        {
            var res = await social.LoginAsync(AuthProviders.Twitter);
            if (res != null)
            {
            }
            else
            {
                await DisplayAlert("Cancelled", "Action was cancelled by user", "OK");
            }
        }
        async void Linkedin_Clicked(object sender, System.EventArgs e)
        {
            var res = await social.LoginAsync(AuthProviders.Linkedin);
            if (res != null)
            {
            }
            else
            {
                await DisplayAlert("Cancelled", "Action was cancelled by user", "OK");
            }
        }
        async void Google_Clicked(object sender, System.EventArgs e)
        {
            var res = await social.LoginAsync(AuthProviders.Google);
            if (res != null)
            {
            }
            else
            {
                await DisplayAlert("Cancelled", "Action was cancelled by user", "OK");
            }
        }

        public reproPage()
        {
            InitializeComponent();
        }
    }
}
