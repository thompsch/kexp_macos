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
        NSMenuItem infoMenu;
        NSMenuItem mute;
        NSMenuItem songInfo;
        NSMenuItem recentMenu;
        NSMenuItem manage;

        Realm realm;
        App app;

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
        public override void ViewDidDisappear()
        {
            realm.Dispose();
            base.ViewDidDisappear();
            NSApplication.SharedApplication.Terminate(this);
        }
        public async override void ViewDidLoad()
        {
            base.ViewDidLoad();
            btnUnsaveFavorite.Hidden = true;

            await DoRealmStuff();

            CurrentSong = new Song()
            {
                UserId = app.CurrentUser.Id
            };

            var url = new NSUrl("https://kexp.org");
            var request = new NSUrlRequest(url);
            webView.LoadRequest(request);

            webView.NavigationDelegate = new WebViewDelegate();
            SetUpStatusMenu();
            GetCurrentSong();

            Timer timer = new Timer(5000);
            timer.Elapsed += timer_Elapsed;
            timer.AutoReset = true;
            timer.Enabled = true;
        }


        private async Task DoRealmStuff()
        {
            app = App.Create("kexpsongs-pnevf");

            if (app.CurrentUser == null)
            {
                await app.LogInAsync(
                Credentials.ApiKey("rCpFIckLbKhL83IcOebQLv8KBfK9LCn0gBaAQDEWK0bkFA6RwwkbD8AKnPwZ7qCv"));

            }
            var config = new FlexibleSyncConfiguration(app.CurrentUser)
            {
                PopulateInitialSubscriptions = (realm) =>
                {
                    var mySongs = realm.All<Song>().Where(n => n.UserId == app.CurrentUser.Id);
                    realm.Subscriptions.Add(mySongs);
                }
            };

            realm = await Realm.GetInstanceAsync(config);
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

            infoMenu = new NSMenuItem(currentSongString);

            var remember = new NSMenuItem("Add to Favorites");
            remember.Activated += Remember_Activated;
            songInfo.Submenu.AddItem(remember);
            item.Menu.AddItem(songInfo);

            recentMenu = new NSMenuItem("Recently Saved Songs");
            recentMenu.Submenu = new NSMenu();
            manage = new NSMenuItem("Manage Favorites...");
            manage.Activated += Manage_Activated;
            recentMenu.Submenu.AddItem(manage);

            UpdateRecentList();
            item.Menu.AddItem(recentMenu);
        }

        private void Manage_Activated(object sender, EventArgs e)
        {
            //TODO: new UI????
            // throw new NotImplementedException();
        }

        private void UpdateRecentList()
        {
            recentMenu.Submenu.RemoveAllItems();
            recentMenu.Submenu.AddItem(manage);

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
            FavoriteOrNot(true);
        }

        private void FavoriteOrNot(bool save)
        {
            if (CurrentSong == null || CurrentSong.Title == null)
            {
                return;
            }

            var alreadyFavorite = realm.Find<Song>(CurrentSong.Id);

            if (save && alreadyFavorite == null)
            {
                realm.Write(() =>
                {
                    realm.Add<Song>(CurrentSong);
                });

                UpdateRecentList();

                btnSaveFavorite.Hidden = true;
                btnUnsaveFavorite.Hidden = false;
            }
            else
            {
                if (alreadyFavorite != null)
                {
                    realm.Write(() =>
                    {
                        realm.Remove(CurrentSong);
                    });

                    UpdateRecentList();
                    GetCurrentSong();
                    btnSaveFavorite.Hidden = false;
                    btnUnsaveFavorite.Hidden = true;
                }
            }
        }

        public void GetCurrentSong()
        {
            infoMenu.Title = "OH YEAH";
            /* Useful if we want to save roundtrip calls 
             * cuyrrently, however, it is fetching the album name
             * we want to get song_title, but that has been formatted 
             * for website view and needs to string manipulated.
             * 
            // check page's album name to see if it has changed
            WKJavascriptEvaluationResult handler = (NSObject result, NSError err) =>
            {
                if (err != null)
                {
                    System.Console.WriteLine(err);
                }
                if (result != null)
                {
                    System.Console.WriteLine(result);
                    if (result.ToString() == CurrentSong.Title) { }
                }
            };

            webView.EvaluateJavaScript("document.getElementsByClassName('Player-album')[0].textContent",
                handler);

            */
            var request = WebRequest.CreateHttp("https://api.kexp.org/v2/plays/?limit=1");
            request.ContentType = "application/json";
            var response = request.GetResponse();

            using (var stream = response.GetResponseStream())
            {
                var reader = new StreamReader(stream);
                var body = reader.ReadToEnd();
                var jd = JsonSerializer.Deserialize<JsonSong>(body);
                var song = jd.results[0];
                InvokeOnMainThread(() =>
                {
                    if (!CurrentSong.IsValid)
                    {
                        CurrentSong = new Song()
                        {
                            UserId = app.CurrentUser.Id,
                        };
                    }

                    if (CurrentSong != null && CurrentSong.Title != song.song)
                    {
                        CurrentSong = new Song()
                        {
                            UserId = app.CurrentUser.Id,
                            Title = song.song,
                            Artist = song.artist,
                        };
                        DateTimeOffset airDate;
                        DateTimeOffset.TryParse(song.airdate, out airDate);
                        CurrentSong.AirTime = airDate;
                        currentSongString = FormatSongForDisplay(CurrentSong);

                        btnSaveFavorite.Hidden = false;
                        btnUnsaveFavorite.Hidden = true;
                        songInfo.Title = currentSongString;

                        titleBar.StringValue = currentSongString;
                    }
                });
            }
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

        #region Event Handlers
        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            GetCurrentSong();
        }
        partial void save_favorite(NSObject sender)
        {
            FavoriteOrNot(true);
        }
        partial void unsave_favorite(NSObject sender)
        {
            FavoriteOrNot(false);
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

        #endregion

        private string FormatSongForDisplay(Song song)
        {
            return $"{song.Artist} - {song.Title}";
        }
    }
}
