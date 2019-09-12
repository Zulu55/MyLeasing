using Facebook.LoginKit;
using MyLeasing.Prism.Controls;
using MyLeasing.Prism.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(FacebookLoginButton), typeof(FacebookLoginButtonRenderer))]
namespace MyLeasing.Prism.iOS.Renderers
{
    public class FacebookLoginButtonRenderer : ViewRenderer
    {
        bool disposed;

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);
            if (Control == null)
            {
                var fbLoginBtnView = e.NewElement as FacebookLoginButton;
                var fbLoginbBtnCtrl = new LoginButton();

                fbLoginbBtnCtrl.Completed += AuthCompleted;
                SetNativeControl(fbLoginbBtnCtrl);
            }
        }

        void AuthCompleted(object sender, LoginButtonCompletedEventArgs args)
        {
            var view = Element as FacebookLoginButton;
            if (args.Error != null)
            {
                view.OnError?.Execute(args.Error.ToString());

            }
            else if (args.Result.IsCancelled)
            {
                view.OnCancel?.Execute(null);
            }
            else
            {
                view.OnSuccess?.Execute(args.Result.Token.TokenString);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !this.disposed)
            {
                if (Control != null)
                {
                    (Control as LoginButton).Completed -= AuthCompleted;
                    Control.Dispose();
                }
                this.disposed = true;
            }
            base.Dispose(disposing);
        }
    }
}