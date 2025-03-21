using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Handlers;
using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml.Controls;

[assembly: ExportRenderer(typeof(Microsoft.UI.Xaml.Controls.ScrollView), typeof(MaliyetApp.Platforms.Windows.CustomScrollViewHandler))]
namespace MaliyetApp.Platforms.Windows
{
    public class CustomScrollViewHandler : ScrollViewHandler
    {
        protected override void ConnectHandler(Microsoft.UI.Xaml.Controls.ScrollViewer platformView)
        {
            base.ConnectHandler(platformView);

            if (platformView != null)
            {
                // Scroll bar'ı her zaman görünür yap
                //platformView.VerticalScrollBarVisibility = Microsoft.UI.Xaml.Controls.ScrollBarVisibility.Visible;
                //platformView.HorizontalScrollBarVisibility = Microsoft.UI.Xaml.Controls.ScrollBarVisibility.Visible;

                // Scroll bar stilini değiştirmek
                var scrollBarStyle = new Microsoft.UI.Xaml.Style(typeof(Microsoft.UI.Xaml.Controls.Primitives.ScrollBar));

                // Scroll bar genişliği ayarı
                //scrollBarStyle.Setters.Add(new Microsoft.UI.Xaml.Setter(Microsoft.UI.Xaml.Controls.Primitives.ScrollBar.BorderThicknessProperty, 50));

                scrollBarStyle.Setters.Add(new Microsoft.UI.Xaml.Setter(Microsoft.UI.Xaml.Controls.Primitives.ScrollBar.WidthProperty, 20));
                // Scroll bar rengi ayarı
                scrollBarStyle.Setters.Add(new Microsoft.UI.Xaml.Setter(Microsoft.UI.Xaml.Controls.Primitives.ScrollBar.ForegroundProperty, new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red)));
                scrollBarStyle.Setters.Add(new Microsoft.UI.Xaml.Setter(Microsoft.UI.Xaml.Controls.Primitives.ScrollBar.BackgroundProperty, new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Gray)));

                // Platform View'de Scroll bar stilini uygula
                platformView.Resources.Add(typeof(Microsoft.UI.Xaml.Controls.Primitives.ScrollBar), scrollBarStyle);
            }
        }

        //public class CustomScrollViewHandler : ScrollViewHandler
        //{
        //    protected override void ConnectHandler(ScrollViewer platformView)
        //    {
        //        base.ConnectHandler(platformView);

        //        // Kaydırma çubuklarını her zaman göster
        //        platformView.HorizontalScrollBarVisibility = Microsoft.UI.Xaml.Controls.ScrollBarVisibility.Visible;
        //        platformView.VerticalScrollBarVisibility = Microsoft.UI.Xaml.Controls.ScrollBarVisibility.Visible;
        //    }
        //}
    }
}
