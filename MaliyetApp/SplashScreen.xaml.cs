namespace MaliyetApp;

public partial class SplashScreen : ContentPage
{
	public SplashScreen()
	{

		InitializeComponent();
        this.Dispatcher.StartTimer(TimeSpan.FromSeconds(3), () =>
        {
            Application.Current.MainPage = new AppShell();
            return false; 
        });
       
    }
}