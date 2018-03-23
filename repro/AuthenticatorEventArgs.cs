using System;

namespace repro
{
    public class AuthenticatorEventArgs : EventArgs
    {
        //
        // Constructors
        //
        public AuthenticatorEventArgs(string message)
        {
            Message = message;
        }

        public AuthenticatorEventArgs(Exception exception)
        {
            Message = exception.Message;
            Exception = exception;
        }

        public AuthenticatorEventArgs(Xamarin.Auth.Account account)
        {
            Account = account;
        }

        //
        // Properties
        //
        public Exception Exception { get; }

        public string Message { get; }

        public bool IsAuthenticated => Account != null;

        public Xamarin.Auth.Account Account { get; }
    }
}
