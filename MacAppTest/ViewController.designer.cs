// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace KEXP
{
    [Register("ViewController")]
    partial class ViewController
    {
        [Outlet]
        AppKit.NSButtonCell muteTextChanged { get; set; }

        [Outlet]
        WebKit.WKWebView webView { get; set; }

        [Action("muteClicked:")]
        partial void muteClicked(Foundation.NSObject sender);

        [Action("muteText:")]
        partial void muteText(Foundation.NSObject sender);

        [Action("refresh:")]
        partial void refresh(Foundation.NSObject sender);

        void ReleaseDesignerOutlets()
        {
            if (muteTextChanged != null)
            {
                muteTextChanged.Dispose();
                muteTextChanged = null;
            }

            if (webView != null)
            {
                webView.Dispose();
                webView = null;
            }
        }
    }
}
