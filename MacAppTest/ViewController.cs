using System;

using AppKit;
using Foundation;
using WebKit;

namespace KEXP
{
    public partial class ViewController : NSViewController
    {
        private int volume = 0;
        NSStatusItem item;
        NSStatusBar statusBar;
        NSMenuItem mute;

        public override NSObject RepresentedObject
        {
            get
            {
                return base.RepresentedObject;
            }
            set
            {
                base.RepresentedObject = value;
            }
        }

        public ViewController(IntPtr handle) : base(handle)
        {

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var url = new NSUrl("https://kexp.org");
            var request = new NSUrlRequest(url);
            webView.LoadRequest(request);

            statusBar = NSStatusBar.SystemStatusBar;
            item = statusBar.CreateStatusItem(NSStatusItemLength.Variable);
            item.Title = "KEXP";
            item.HighlightMode = true;
            item.Menu = new NSMenu();
            mute = new NSMenuItem("Mute");
            mute.Activated += Mute_Activated;
            item.Menu.AddItem(mute);
            btnUnmute.Hidden = true;
        }

        private void Mute_Activated(object sender, EventArgs e)
        {
            MuteUnMute();
        }

        partial void btnRefresh_clicked(Foundation.NSObject nSObject)
        {
            webView.Reload();
        }

        partial void btnMute_clicked(Foundation.NSObject nSObject)
        {
            MuteUnMute();
        }

        partial void btnUnmute_clicked(NSObject sender)
        {
            MuteUnMute();
        }

        public void MuteUnMute()
        {
            webView.CallAsyncJavaScript
                ($"document.querySelectorAll('video, audio').forEach(mediaElement => mediaElement.volume = {volume})",
                null, null, WKContentWorld.DefaultClient, null);

            volume = volume == 0 ? 1 : 0;
            if (volume == 0)
            {
                mute.Title = "Mute";
                btnUnmute.Hidden = true;
                btnMute.Hidden = false;
            }
            else
            {
                mute.Title = "Unmute";
                btnUnmute.Hidden = false;
                btnMute.Hidden = true;
            }
        }
    }
}
