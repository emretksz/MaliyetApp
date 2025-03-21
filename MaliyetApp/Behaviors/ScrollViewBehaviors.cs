using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaliyetApp.Behaviors
{
    public class ScrollViewBehaviors: Behavior<ScrollView>
        {
    protected override void OnAttachedTo(ScrollView bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.VerticalScrollBarVisibility = ScrollBarVisibility.Always;
            bindable.VerticalScrollBarVisibility = ScrollBarVisibility.Always;

//#if WINDOWS
//#endif
//        // Windows platformunda kaydırma çubuğunu genişletmek
//        bindable.HandlerChanged += (sender, args) =>
//        {
//            if (bindable.Handler.PlatformView is Microsoft.UI.Xaml.Controls.ScrollViewer scrollViewer)
//            {
//                // Kaydırma çubuğunu her zaman görünür yapar
//                scrollViewer.VerticalScrollBarVisibility = Microsoft.UI.Xaml.Controls.ScrollBarVisibility.Visible;

//                // Kaydırma çubuğunun genişliğini arttırmak için (örneğin, 12 birim)
//                scrollViewer.VerticalScrollBarThickness = 12;
//            }
//        };
            //#if WINDOWS



            //                            // Windows için kaydırma çubuğu ayarlarını özelleştirmek
            //                            bindable.HandlerChanged += (sender, args) =>
            //                            {
            //                                if (bindable.Handler.PlatformView is Microsoft.UI.Xaml.Controls.ScrollViewer scrollViewer)
            //                                {
            //                                    scrollViewer.VerticalScrollBarVisibility = Microsoft.UI.Xaml.Controls.ScrollBarVisibility.Visible; // Kaydırma çubuğunu her zaman görünür yapar
            //                                   // scrollViewer.BorderThickness = new Microsoft.UI.Xaml.Thickness(80);

            //                                }

            //                            };

            //#endif
            // ScrollView ile etkileşimleri burada ele alabilirsiniz
        }

        protected override void OnDetachingFrom(ScrollView bindable)
        {
            base.OnDetachingFrom(bindable);
        }
    }
}
