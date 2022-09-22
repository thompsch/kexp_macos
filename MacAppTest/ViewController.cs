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
using Realms;
using Realms.Sync;
using System.Linq;
using System.Threading.Tasks;

namespace KEXP
{
    public partial class ViewController : NSViewController
    {
        private int volume = 0;
        NSStatusItem item;
        NSStatusBar statusBar;
        NSMenuItem mute;
        NSMenuItem songInfo;
        NSMenuItem recentMenu;

        Realm realm;

        Song CurrentSong;
        string currentSongString = "Loading...";

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

        public async override void ViewDidLoad()
        {
            base.ViewDidLoad();


            // REALM


            var app = App.Create("kexpsongs-pnevf");
            User user = await app.LogInAsync(
                Credentials.ApiKey("rCpFIckLbKhL83IcOebQLv8KBfK9LCn0gBaAQDEWK0bkFA6RwwkbD8AKnPwZ7qCv"));

            var config = new FlexibleSyncConfiguration(app.CurrentUser)
            {
                PopulateInitialSubscriptions = (realm) =>
                {
                    var mySongs = realm.All<Song>().Where(n => n.UserId == user.Id);
                    realm.Subscriptions.Add(mySongs);
                }
            };

            realm = await Realm.GetInstanceAsync(config);

            // END REALM

            CurrentSong = new Song();
            CurrentSong.UserId = user.Id;

            var url = new NSUrl("https://kexp.org");
            var request = new NSUrlRequest(url);
            webView.LoadRequest(request);

            SetUpStatusMenu();


            Timer timer = new Timer(10000);
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private void SetUpStatusMenu()
        {
            statusBar = NSStatusBar.SystemStatusBar;

            item = statusBar.CreateStatusItem(NSStatusItemLength.Square);
            item.Image = NSImage.ImageNamed("menu_icon");
            item.Image.Size = new CoreGraphics.CGSize(18.0, 18.0);
            item.Image.Template = false;
            item.HighlightMode = true;
            item.Menu = new NSMenu();

            mute = new NSMenuItem("Mute");
            mute.Activated += Mute_Activated;
            item.Menu.AddItem(mute);
            btnUnmute.Hidden = true;

            songInfo = new NSMenuItem(currentSongString);
            songInfo.Enabled = true;
            songInfo.Submenu = new NSMenu();

            var remember = new NSMenuItem("Remember this song");
            remember.Activated += Remember_Activated;
            songInfo.Submenu.AddItem(remember);
            item.Menu.AddItem(songInfo);

            recentMenu = new NSMenuItem("Recently Saved Songs");
            //recentMenu.Activated += Recent_Activated;
            recentMenu.Submenu = new NSMenu();
            UpdateRecentList();
            item.Menu.AddItem(recentMenu);
        }

        private void UpdateRecentList()
        {
            //clear existing list
            recentMenu.Submenu = new NSMenu();

            var recentsongs = realm.All<Song>()
                .OrderByDescending(s => s.AirTime)
                .ToList();

            for (int i = 0; i < recentsongs.Count; i++)
            {
                if (i > 9) return;

                Song s = recentsongs[i];
                recentMenu.Submenu.AddItem(new NSMenuItem(FormatSongForDisplay(s)));
            }
        }

        private void Remember_Activated(object sender, EventArgs e)
        {
            if (CurrentSong.Title == null)
            {
                return;
            }

            realm.Write(() =>
            {
                realm.Add<Song>(CurrentSong);
            });

            UpdateRecentList();
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
                var jd = JsonSerializer.Deserialize<JsonSong>(body);
                var song = jd.results[0];

                CurrentSong.Title = song.song;
                CurrentSong.Artist = song.artist;
                DateTimeOffset airDate;
                DateTimeOffset.TryParse(song.airdate, out airDate);
                CurrentSong.AirTime = airDate;
                currentSongString = FormatSongForDisplay(CurrentSong);
            }
            InvokeOnMainThread(() =>
            {
                songInfo.Title = currentSongString;
                titleBar.StringValue = currentSongString;
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

        private string FormatSongForDisplay(Song song)
        {
            return $"{song.Artist} - {song.Title}";
        }
    }

    public class JsonSong
    {
        public List<JsonSongResults> results { get; set; }
        public JsonSong() { this.results = new List<JsonSongResults>(); }
    }

    public class JsonSongResults
    {
        public string song { get; set; }
        public string artist { get; set; }
        public string airdate { get; set; }
    }
}
