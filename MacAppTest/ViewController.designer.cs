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
        AppKit.NSButton btnMute { get; set; }

        [Outlet]
        AppKit.NSButton btnRefresh { get; set; }

        [Outlet]
        AppKit.NSButton btnSaveFavorite { get; set; }

        [Outlet]
        AppKit.NSButton btnUnmute { get; set; }

        [Outlet]
        AppKit.NSButton btnUnsaveFavorite { get; set; }

        [Outlet]
        AppKit.NSTextField titleBar { get; set; }

        [Outlet]
        WebKit.WKWebView webView { get; set; }

        [Action("btnMute_clicked:")]
        partial void btnMute_clicked(Foundation.NSObject sender);

        [Action("btnRefresh_clicked:")]
        partial void btnRefresh_clicked(Foundation.NSObject sender);

        [Action("btnUnmute_click:")]
        partial void btnUnmute_click(Foundation.NSObject sender);

        [Action("btnUnmute_clicked:")]
        partial void btnUnmute_clicked(Foundation.NSObject sender);

        [Action("favorite_save:")]
        partial void favorite_save(Foundation.NSObject sender);

        [Action("favorite_unsave:")]
        partial void favorite_unsave(Foundation.NSObject sender);

        [Action("save_favorite:")]
        partial void save_favorite(Foundation.NSObject sender);

        [Action("unsaveFavorite:")]
        partial void unsave_favorite(Foundation.NSObject sender);

        void ReleaseDesignerOutlets()
        {
            if (btnSaveFavorite != null)
            {
                btnSaveFavorite.Dispose();
                btnSaveFavorite = null;
            }

            if (btnMute != null)
            {
                btnMute.Dispose();
                btnMute = null;
            }

            if (btnRefresh != null)
            {
                btnRefresh.Dispose();
                btnRefresh = null;
            }

            if (btnUnsaveFavorite != null)
            {
                btnUnsaveFavorite.Dispose();
                btnUnsaveFavorite = null;
            }

            if (btnUnmute != null)
            {
                btnUnmute.Dispose();
                btnUnmute = null;
            }

            if (titleBar != null)
            {
                titleBar.Dispose();
                titleBar = null;
            }

            if (webView != null)
            {
                webView.Dispose();
                webView = null;
            }
        }
    }
}
