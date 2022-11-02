using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using AppKit;
using Foundation;

namespace KEXP
{
    [Register("AppDelegate")]
    public partial class AppDelegate : NSApplicationDelegate
    {
        private string songInfo;
        NSStatusItem item;
        NSStatusBar statusBar;

        public AppDelegate()
        {
        }
        public override void DidBecomeActive(NSNotification notification)
        {

            //var https = (HttpWebRequest)WebRequest.Create("https://api.kexp.org/v2/plays/?limit=1");
            //https.BeginGetResponse(GotSongInfo, https);
        }
        public override void DidFinishLaunching(NSNotification notification)
        {
            // Insert code here to initialize your application
            // Create a status bar menu





            ////var address = new NSMenuItem(songInfo);

            ///*address.Activated += (sender, e) => {
            //    PhraseAddress(address);
            //};*/
            //// item.Menu.AddItem(address);

            //var date = new NSMenuItem("Date");
            ////date.Activated += (sender, e) =>
            ////{
            ////    PhraseDate(date);
            ////};
            //item.Menu.AddItem(date);

            //var greeting = new NSMenuItem("Greeting");
            ////greeting.Activated += (sender, e) =>
            ////{
            ////    PhraseGreeting(greeting);
            ////};
            //item.Menu.AddItem(greeting);

            //var signature = new NSMenuItem("Signature");
            ////signature.Activated += (sender, e) =>
            ////{
            ////    PhraseSignature(signature);
            ////};
            //item.Menu.AddItem(signature);
        }

        public override void WillTerminate(NSNotification notification)
        {
            // Insert code here to tear down your application
        }

        void GotSongInfo(IAsyncResult result)
        {
            var request = result.AsyncState as HttpWebRequest;

            try
            {
                var response = request.EndGetResponse(result);
                var reader = new StreamReader(response.GetResponseStream());
                string content = reader.ReadToEnd();
                var blop = System.Text.Json.JsonSerializer.Deserialize(content, typeof(KeyValuePair));
                // songInfo = blop["song"];

                //InvokeOnMainThread(() =>
                //{
                //    var si = new NSMenuItem(songInfo);
                //    statusBar = NSStatusBar.SystemStatusBar;


                //    item = statusBar.CreateStatusItem(NSStatusItemLength.Variable);
                //    item.Title = "mute";
                //    item.HighlightMode = true;

                //    var mute = new NSMenuItem("Mute");
                //    mute.Activated += Mute_Activated;

                //});

            }
            catch (Exception e)
            {

            }
        }


    }
}

