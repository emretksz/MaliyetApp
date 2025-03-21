using CommunityToolkit.Maui.Views;
using MaliyetApp.Libs.AppSettings;
using MaliyetApp.Libs.Models.SQlitecontextDb;
using System.Windows.Input;

namespace MaliyetApp.Views.SettingPage;
public partial class AppSettingPage : ContentPage
{
    public ICommand SaveSystemDataCommand { get; }
    bool first = true;
    DateTime lastSelectedYear;

    public AppSettingPage()
    {
        InitializeComponent();
        SaveSystemDataCommand = new Command(SaveSystemData);
        BindingContext = this;

    #if WINDOWS

    verileriAktar.IsVisible=true;
    #endif
    }
   

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            var year = await SecureStorage.GetAsync("Year");
            int parsedYear;
            if (year != null && int.TryParse(year, out parsedYear) && parsedYear != 0)
            {
                datePicker.Date = new DateTime(parsedYear, datePicker.Date.Month, datePicker.Date.Day);
                lastSelectedYear = datePicker.Date; // Son se�ilen y�l� kaydet
            }
            else
            {
                var now = DateTime.Now;
                datePicker.Date = new DateTime(now.Year, now.Month, now.Day);
                lastSelectedYear = datePicker.Date; // Son se�ilen y�l� kaydet
            }
            first = false;
        }
        catch (Exception)
        {
            // Hata durumunda yap�lacak i�lemler
        }
    }

    private void SaveSystemData()
    {
        // Burada Save i�lemi yap�labilir
        var selectedYear = datePicker.Date.Year;
    }

    private void datePicker_Focused(object sender, FocusEventArgs e)
    {
        var senders = (DatePicker)sender;
        senders.BackgroundColor = Colors.Gray;
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        // Veritaban� senkronizasyonu veya ba�ka i�lemler
        IsBusy = true;
        //uyari.IsVisible = true;
        LoadingPopup loadingPopup = new LoadingPopup();
        this.ShowPopup(loadingPopup);
        await SyncServices.StartSyncSevices();
        loadingPopup.Close();
        DisplayAlert("i�lem ba�ar�l�", "Aktarma i�lemi tamamland�", "Tamam");
        //uyari.IsVisible = false;
        IsBusy = false;
    }
    private async void datePicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        var senders = (DatePicker)sender;
        var selectedYear = senders.Date.Year;

        // E�er y�l de�i�tiyse, yeni y�l� g�venli depolamaya kaydet
        if (selectedYear != lastSelectedYear.Year)
        {
            await SecureStorage.SetAsync("Year", selectedYear.ToString());
            lastSelectedYear = senders.Date; // Son se�ilen y�l� g�ncelle
        }
    }
}

//public partial class AppSettingPage : ContentPage
//{
//    public ICommand SaveSystemDataCommand { get; }
//    bool first = true;
//    public AppSettingPage()
//	{
//		InitializeComponent();
//        SaveSystemDataCommand = new Command(SaveSystemData);
//        BindingContext = this;
//    }
//    protected async override void OnAppearing()
//    {
//        base.OnAppearing();
//        try
//        {

//            var year = await SecureStorage.GetAsync("Year");
//            int parsedYear;
//            if (year != null && int.TryParse(year, out parsedYear) && parsedYear != 0)
//            {
//                datePicker.Date = new DateTime(parsedYear, datePicker.Date.Month, datePicker.Date.Day);
//            }
//            else
//            {
//                var now = DateTime.Now;
//                datePicker.Date = new DateTime(now.Year, now.Month, now.Day);
//            }
//            first = false;
//        }
//        catch (Exception)
//        {

//        }

//    }
//    private void SaveSystemData()
//    {
//        var selectedYear = datePicker.Date.Year;
//    }

//    private void datePicker_Focused(object sender, FocusEventArgs e)
//    {
//        var senders = (DatePicker)sender;
//        senders.BackgroundColor = Colors.Gray;
//    }

//    private async void Button_Clicked(object sender, EventArgs e)
//    {
//        // await DatabaseService.SyncLocalDataToServer();
//        IsBusy = true;
//        uyari.IsVisible = true;
//        await SyncServices.StartSyncSevices();
//        uyari.IsVisible = false;
//        IsBusy = false;
//    }


//    private async void datePicker_DateSelected(object sender, DateChangedEventArgs e)
//    {
//        var senders = (DatePicker)sender;
//        await SecureStorage.SetAsync("Year", senders.Date.Year.ToString());
//    }
//}