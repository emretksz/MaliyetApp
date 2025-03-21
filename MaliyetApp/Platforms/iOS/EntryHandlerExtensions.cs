
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UIKit;

namespace MaliyetApp.Platforms.iOS
{
   
    public static class EntryHandlerExtensions
    {
        public static void AddDoneButtonToNumericKeyboard()
        {
            EntryHandler.Mapper.AppendToMapping("NumericKeyboardWithDone", (handler, view) =>
            {
                if (view.Keyboard == Keyboard.Numeric && handler.PlatformView is UITextField textField)
                {
                    // Toolbar oluştur
                    var toolbar = new UIToolbar(new CoreGraphics.CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, 44))
                    {
                        BarStyle = UIBarStyle.Default,
                        Translucent = true
                    };

                    // Done butonu ekle
                    var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, (sender, e) =>
                    {
                        textField.ResignFirstResponder(); // Klavyeyi kapat
                    });

                    toolbar.Items = new[] { new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace), doneButton };
                    textField.InputAccessoryView = toolbar; // Toolbar'ı klavyeye ekle
                }
            });
        }

      
            public static void RemoveCancelButton()
            {
                SearchBarHandler.Mapper.AppendToMapping("RemoveCancelButton", (handler, view) =>
                {
                    if (handler.PlatformView is UISearchBar searchBar)
                    {
                        // "Cancel" butonunu gizle
                        searchBar.ShowsCancelButton = false;
                        //searchBar.ShowsSearchResultsButton = true;

                    }

                });
             }
    }

}

