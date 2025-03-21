using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UIKit;

namespace MaliyetApp.Platforms.iOS
{
    public class CustomSearchBarHandler : SearchBarHandler
    {
        protected override void ConnectHandler(MauiSearchBar platformView)
        {
            base.ConnectHandler(platformView);

        }
    }
}
