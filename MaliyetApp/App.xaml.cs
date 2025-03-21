using MaliyetApp.Libs.AppSettings;

namespace MaliyetApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            UserAppTheme = AppTheme.Light;

            // string dbPath = Path.Combine(FileSystem.AppDataDirectory, "appdata.db");

            Task.Run(async () =>
            {
                 await SecureStorage.SetAsync("MainRefresh","true"); 
                await SecureStorage.SetAsync("OrderRefresh", "true"); 

            });

#if WINDOWS
              string appDataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "brsmaliyetprogram.db");
                        string directoryPath = Path.GetDirectoryName(appDataDirectory);

                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }
      
#else
            try
            {
                //string DatabaseFileName = "brsmaliyetprogram.db";
                //string BackupFileName = "brsmaliyetprogrambackupdata.db";

                //// Veritabanı yolu
                //string AppDataDirectory = Path.Combine(FileSystem.AppDataDirectory, DatabaseFileName);

                //string BackupFilePath = Path.Combine(FileSystem.AppDataDirectory, BackupFileName);


                //if (!File.Exists(AppDataDirectory))
                //{
                //    using (var stream = File.Create(AppDataDirectory))
                //    {
                //        // Dosyayı hemen kapat
                //    }
                //}

            }
            catch (Exception ex)
            {
                ExceptionAlert.ShowAlert("spless",ex.Message);
            }
            #endif

            MainPage = new SplashScreen();
       

        }
    }
}
