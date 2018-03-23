using System;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Auth.Presenters;
using Xamarin.Forms;

namespace repro
{
    public class SocialServices
    {

        public async Task<Xamarin.Auth.Account> LoginAsync(AuthProviders provider, bool is_native = true)
        {
            switch (provider)
            {
                case AuthProviders.Facebook:
                    return await SocialLogin(FacebookSignup(is_native));
                case AuthProviders.Google:
                    return await SocialLogin(GoogleSignup(is_native));
                case AuthProviders.Twitter:
                    return await SocialLogin(TwitterSignup(is_native));
                case AuthProviders.Linkedin:
                    return await SocialLogin(LinkedinSignup(is_native));
                default:
                    return null;
            }
        }

        public async Task<Xamarin.Auth.Account> SocialLogin<T>(T authenticator)
        {
            var tcs1 = new TaskCompletionSource<AuthenticatorEventArgs>();
            EventHandler<AuthenticatorCompletedEventArgs> completed =
                (o, e) =>
                {
                    try
                    {
                        var eventargs = new AuthenticatorEventArgs(e.Account);
                        tcs1.TrySetResult(eventargs);
                    }
                    catch (Exception ex)
                    {
                        var eventargs = new AuthenticatorEventArgs(ex);
                        tcs1.TrySetResult(eventargs);
                    }
                };
            EventHandler<AuthenticatorErrorEventArgs> error =
                (o, e) =>
                {
                    try
                    {
                        if (e.Exception != null)
                        {
                            var eventargs = new AuthenticatorEventArgs(e.Exception);
                            tcs1.TrySetResult(eventargs);
                        }
                        else
                        {
                            var eventargs = new AuthenticatorEventArgs(e.Message);
                            tcs1.TrySetResult(eventargs);
                        }
                    }
                    catch (Exception ex)
                    {
                        var eventargs = new AuthenticatorEventArgs(ex);
                        tcs1.TrySetResult(eventargs);
                    }
                };

            try
            {
                if (typeof(T) == typeof(OAuth2Authenticator))
                {
                    (authenticator as OAuth2Authenticator).Completed += completed;
                    (authenticator as OAuth2Authenticator).Error += error;

                }
                else
                {
                    (authenticator as OAuth1Authenticator).Completed += completed;
                    (authenticator as OAuth1Authenticator).Error += error;
                }

                Builder(authenticator);
                var result = await tcs1.Task;
                return result.Account;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                if (typeof(T) == typeof(OAuth2Authenticator))
                {
                    (authenticator as OAuth2Authenticator).Completed -= completed;
                    (authenticator as OAuth2Authenticator).Error -= error;
                }
                else
                {
                    (authenticator as OAuth1Authenticator).Completed -= completed;
                    (authenticator as OAuth1Authenticator).Error -= error;
                }
            }
        }

        public void Builder<T>(T authenticator)
        {
            // after initialization (creation and event subscribing) exposing local object 
            if (typeof(T) == typeof(OAuth2Authenticator))
                AuthenticationState.Authenticator = authenticator as OAuth2Authenticator;
            OAuthLoginPresenter presenter = null;
            presenter = new OAuthLoginPresenter();
            presenter.Login(authenticator as Authenticator);
        }

        private OAuth2Authenticator FacebookSignup(bool native_ui)
        {
            var authenticator
                = new OAuth2Authenticator
                (
                    new Func<string>
                    (
                        () =>
                        {
                            var retval_client_id = "oops something is wrong!";
                            retval_client_id = Constants.fb_app_id;
                            return retval_client_id;
                        }
                    ).Invoke(),
                    authorizeUrl:
                    new Func<Uri>
                    (
                        () =>
                        {
                            string uri = null;
                            if (native_ui)
                                uri = "https://www.facebook.com/v2.9/dialog/oauth";
                            else
                                uri = "https://m.facebook.com/dialog/oauth/";
                            return new Uri(uri);
                        }
                    ).Invoke(),
                    redirectUrl:
                    new Func<Uri>
                    (
                        () =>
                        {
                            string uri = null;
                            if (native_ui)
                                uri = $"fb{Constants.fb_app_id}://authorize";
                            else
                                uri = "http://www.facebook.com/connect/login_success.html";
                            return new Uri(uri);
                        }
                    ).Invoke(),
                    scope: "email", // "basic", "email",
                    getUsernameAsync: null,
                    isUsingNativeUI: native_ui
                )
                {
                    AllowCancel = true,
                    ShowErrors = false
                };
            return authenticator;
        }

        private static OAuth1Authenticator TwitterSignup(bool native_ui)
        {
            var authenticator = new OAuth1Authenticator(
                Constants.consumerKey,
                isUsingNativeUI: native_ui,
                consumerSecret: Constants.consumerSecret,
                requestTokenUrl: new Uri("https://api.twitter.com/oauth/request_token"),
                authorizeUrl: new Uri("https://api.twitter.com/oauth/authorize"),
                accessTokenUrl: new Uri("https://api.twitter.com/oauth/access_token"),
                callbackUrl: new Uri("https://mobile.twitter.com/")
            );
            authenticator.ShowErrors = false;
            authenticator.AllowCancel = true;
            return authenticator;
        }

        private static OAuth2Authenticator LinkedinSignup(bool native_ui)
        {
            var authenticator = new OAuth2Authenticator
            (
                Constants.linkedinClientId,
                Constants.linkedinClientSecret,
                authorizeUrl: new Uri("https://www.linkedin.com/oauth/v2/authorization"),
                accessTokenUrl: new Uri("https://www.linkedin.com/oauth/v2/accessToken"),
                redirectUrl: new Uri("https://devapi.ad-din.ca/authCallback/linkedIn"),
                scope: "r_basicprofile r_emailaddress", // "basic", "email",
                isUsingNativeUI: native_ui
            )
            {
                AllowCancel = true,
                ShowErrors = false
            };
            return authenticator;
        }

        private static OAuth2Authenticator GoogleSignup(bool native_ui)
        {
            var authenticator
                = new OAuth2Authenticator
                (
                    new Func<string>
                    (
                        () =>
                        {
                            var retval_client_id = "oops something is wrong!";

                            switch (Device.RuntimePlatform)
                            {
                                case "Android":
                                    retval_client_id = Constants.googleClientIdAndroid + ".apps.googleusercontent.com";
                                    break;
                                case "iOS":
                                    retval_client_id = Constants.googleClientIdIOS + ".apps.googleusercontent.com";
                                    break;
                            }

                            return retval_client_id;
                        }
                    ).Invoke(),
                    null,
                    authorizeUrl: new Uri("https://accounts.google.com/o/oauth2/auth"),
                    accessTokenUrl: new Uri("https://accounts.google.com/o/oauth2/token"),
                    redirectUrl:
                    new Func<Uri>
                    (
                        () =>
                        {
                            string uri = null;
                            switch (Device.RuntimePlatform)
                            {
                                case "Android":
                                    uri = "com.googleusercontent.apps." + Constants.googleClientIdAndroid +
                                          ":/oauth2redirect";
                                    ;
                                    break;
                                case "iOS":
                                    uri = "com.googleusercontent.apps." + Constants.googleClientIdIOS +
                                          ":/oauth2redirect";
                                    break;
                            }

                            return new Uri(uri);
                        }
                    ).Invoke(),
                    scope:
                    //"profile"
                    "https://www.googleapis.com/auth/userinfo.email"
                    ,
                    getUsernameAsync: null,
                    isUsingNativeUI: native_ui
                )
                {
                    ShowErrors = false,
                    AllowCancel = true
                };
            return authenticator;
        }
    }
}
