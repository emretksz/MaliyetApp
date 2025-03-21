﻿using CommunityToolkit.Maui;
using MaliyetApp.Libs.Models.SQlitecontextDb;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.Maui.Platform;
using FFImageLoading.Maui;


#if WINDOWS
using Microsoft.UI.Windowing;
using Microsoft.UI;
using WinRT.Interop;
#endif
namespace MaliyetApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit().UseFFImageLoading()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if WINDOWS
                    builder.ConfigureLifecycleEvents(events =>
                    {
                        // Make sure to add "using Microsoft.Maui.LifecycleEvents;" in the top of the file 
                        events.AddWindows(windowsLifecycleBuilder =>
                        {
                            windowsLifecycleBuilder.OnWindowCreated(window =>
                            {
                                window.ExtendsContentIntoTitleBar = true;
                                var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
                                var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
                                var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(id);
                                switch (appWindow.Presenter)
                                {
                                    case Microsoft.UI.Windowing.OverlappedPresenter overlappedPresenter:
                                        overlappedPresenter.SetBorderAndTitleBar(true, true);
                                        overlappedPresenter.Maximize();
                                        break;
                                }
                            });
                        });
                    });
            builder.ConfigureMauiHandlers(handlers =>
            {
                handlers.AddHandler<ScrollView, MaliyetApp.Platforms.Windows.CustomScrollViewHandler>();
            });

#endif

#if IOS
            MaliyetApp.Platforms.iOS.EntryHandlerExtensions.AddDoneButtonToNumericKeyboard();
                        MaliyetApp.Platforms.iOS.EntryHandlerExtensions.RemoveCancelButton();

            
#endif
# if WINDOWS

            builder.Services.AddSingleton<DatabaseService>(DatabaseService.Instance);
#endif
       
#if DEBUG     
            builder.Logging.AddDebug();
#endif
           

            return builder.Build();
        }
    }
}
