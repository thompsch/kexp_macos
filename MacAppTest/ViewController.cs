using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Policy;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Timers;
using AppKit;
using Foundation;
using SceneKit;
using WebKit;
using Xamarin.Forms;

namespace KEXP
{
    public partial class ViewController : NSViewController
    {
        private int volume = 0;
        NSStatusItem item;
        NSStatusBar statusBar;
        NSMenuItem mute;
        NSMenuItem songInfo;

        string currentSong = "Looking...";

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

            songInfo = new NSMenuItem(currentSong);
            songInfo.Enabled = true;
            item.Menu.AddItem(songInfo);

            Timer_Elapsed(null, null);

            Timer timer = new Timer(10000);
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
            timer.Enabled = true;
        }


        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Get current song & artist
            var request = WebRequest.CreateHttp("https://api.kexp.org/v2/plays/?limit=1");
            request.ContentType = "application/json";
            var response = request.GetResponse();

            using (var stream = response.GetResponseStream())
            {
                var reader = new StreamReader(stream);
                var body = reader.ReadToEnd();
                var jd = JsonSerializer.Deserialize<Song>(body);
                currentSong = $"{jd.results[0].artist} - {jd.results[0].song}";
            }
            InvokeOnMainThread(() =>
            {
                songInfo.Title = currentSong;

                titleBar.StringValue = currentSong;
            });
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

    public class Song
    {
        public List<SongResults> results { get; set; }

        public Song() { this.results = new List<SongResults>(); }
    }

    public class SongResults
    {
        public string song { get; set; }
        public string artist { get; set; }
    }
}
