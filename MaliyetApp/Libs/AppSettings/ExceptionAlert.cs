using CommunityToolkit.Maui.Alerts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaliyetApp.Libs.AppSettings
{
    public class ExceptionAlert
    {


        public static void ShowAlert(string title,string message)
        {
            if (Application.Current?.MainPage != null)
            {
                Application.Current.MainPage.DisplayAlert(title, message, "Tamam");
            }
            else
            {
                Application.Current.MainPage.DisplayPromptAsync("xxx","test","OKKK");
            }
        }
        

    }

    }

