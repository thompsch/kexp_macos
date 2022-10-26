using System;
using WebKit;

namespace KEXP
{
    public class WebViewDelegate : WKNavigationDelegate
    {
        public WebViewDelegate()
        {
        }

        public override void DidFinishNavigation(
            WKWebView webView, WKNavigation navigation)
        {
            webView.CallAsyncJavaScript
                           ($"document.getElementsByClassName('Player-ctaHeadline')[0].click();",
                           null, null, WKContentWorld.DefaultClient, null);
        }

    }
}

