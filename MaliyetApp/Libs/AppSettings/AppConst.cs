using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaliyetApp.Libs.AppSettings
{
    public class AppConst
    {

       public enum MarketRateType
        {
            Dolar=0,
            Euro=1,
            TL=2
        }


        public static string HostAddress = "https://localhost:7239/";
        public static string ImageAddress = "https://localhost:7239/MobileService/UploadImage";
        public static string ExcelPath = "https://localhost:7239/MobileService/document";
        public static string ImagePath = "https://localhost:7239/MobileService/UploadImage";
    }
}
