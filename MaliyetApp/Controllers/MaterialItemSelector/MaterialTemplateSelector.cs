using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaliyetApp.Controllers.MaterialItemSelector
{
    public class MaterialTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DesktopTemplate { get; set; }
        public DataTemplate PhoneTemplate { get; set; }
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (DeviceInfo.Idiom == DeviceIdiom.Desktop)
                return DesktopTemplate;

            return PhoneTemplate;
        }
    }
}
