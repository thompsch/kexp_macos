using System;

using AppKit;
using Foundation;
using WebKit;

namespace KEXP
{
    public partial class ViewController : NSViewController
    {
        int volume = 0;

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
                // Update the view, if already loaded.
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
        }

        private void Mute_Activated(object sender, EventArgs e)
        {
            MuteUnMute();
        }

        partial void refresh(Foundation.NSObject nSObject)
        {
            webView.Reload();
        }

        partial void muteClicked(Foundation.NSObject nSObject)
        {
            MuteUnMute();
        }

        public void MuteUnMute()
        {
            webView.CallAsyncJavaScript
                ($"document.querySelectorAll('video, audio').forEach(mediaElement => mediaElement.volume = {volume})"
                , null, null, WKContentWorld.DefaultClient, null);

            volume = volume == 0 ? 1 : 0;
            if (volume == 0)
            {
                muteTextChanged.Title = "Mute";
                muteTextChanged.BackgroundColor = NSColor.Blue;
                mute.Title = "Mute";
                webView.AddSubview(muteTextChanged.ControlView);
            }
            else
            {
                muteTextChanged.Title = "Unmute";
                muteTextChanged.BackgroundColor = NSColor.Red;
                mute.Title = "Unmute";
            }


        }
    }
}
