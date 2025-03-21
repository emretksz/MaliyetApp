using Microsoft.Maui.Controls.Handlers;
using Microsoft.Maui.Controls.Platform;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MaliyetApp.Platforms.Windows
{
    public class ShellViewHandler : ShellHandler
    {
        protected override void ConnectHandler(ShellView platformView)
        {
            base.ConnectHandler(platformView);
            if (platformView != null)
            {
                // Navigasyon bar arka plan rengini değiştir
                //var blushBrush = new SolidColorBrush(Windows. Colors.AliceBlue);
                //platformView.Background = blushBrush;

                //// Başlık metni rengini değiştir
                //platformView.TitleBar.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Black);

                //// İkonları değiştirme
                //if (platformView.PaneFooter is StackPanel footerPanel)
                //{
                //    foreach (var element in footerPanel.Children)
                //    {
                //        if (element is IconElement icon)
                //        {
                //            icon.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Red);
                //        }
                //    }
                //}
            }
        }
       
    }
}
