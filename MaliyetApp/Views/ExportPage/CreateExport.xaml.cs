using CommunityToolkit.Maui.Views;
using DocumentFormat.OpenXml.Bibliography;
using MaliyetApp.Libs.Models;
using MaliyetApp.Libs.Models.SQlitecontextDb;
using MaliyetApp.Views.Actions;
using Microsoft.Maui.Controls.PlatformConfiguration;
using System.Collections.ObjectModel;
#if IOS
using UIKit;
#endif
namespace MaliyetApp.Views.ExportPage;

public partial class CreateExport : ContentPage
{
	public CreateExport()
	{
		InitializeComponent();

        this.Dispatcher.Dispatch(() =>
        {
#if IOS
            AddTapGestureRecognizer();
#endif
        });
    }
#if IOS
                 private void AddTapGestureRecognizer()
    {
        // Sayfanýn içeriðine dokunma algýlayýcýsý ekle
        if (this.Handler?.PlatformView is UIView pageView)
        {
            var tapGestureRecognizer = new UITapGestureRecognizer(() =>
            {
                // Klavyeyi kapat
                CloseKeyboard();
            });

            // Dokunma algýlayýcýsýný sayfanýn içeriðine ekle
            pageView.AddGestureRecognizer(tapGestureRecognizer);
        }
    }

    private void CloseKeyboard()
    {
        if (searchbar.Handler?.PlatformView is UISearchBar nativeSearchBar)
        {
            nativeSearchBar.ResignFirstResponder(); // Klavyeyi kapat
        }
    }
#endif
    protected override async void OnAppearing()
    {

        try
        {
            var orderRefresh = await SecureStorage.GetAsync("OrderRefresh");

            bool refresh;

            if (String.IsNullOrEmpty(orderRefresh))
                refresh = true;
            else
            {
                var x = (Boolean.TryParse(orderRefresh, out refresh));
            }

            if (refresh)
            {
                IsBusy = true;
                pageSize = 0;
                base.OnAppearing();
                var year = await SecureStorage.GetAsync("Year");
                Dispatcher.DispatchAsync(async () =>
                {
                    var result = await DatabaseService.GetAllOrder(year);
                    if (result == null)
                    {
                        return;
                    }
                    var model = new ObservableCollection<Order>();

                    foreach (var item in result)
                    {
                        model.Add(item);
                    }
                    ProdcutList.ItemsSource = model;
                    BindingContext = model;
                    pageSize++;
                    loading.IsVisible = false;
                    IsBusy = false;
                    Task.Run(async () =>
                    {
                        await SecureStorage.SetAsync("OrderRefresh", "false");
                    });
                });
            }
        }
        catch (Exception)
        {
            IsBusy = false;
        }
    }
    private async void Button_Clicked(object sender, EventArgs e)
    {

#if WINDOWS
        var popup = new ProductListPopup();
                popup.Action += () =>
                {
                   
                    OnAppearing();
                };
                await this.ShowPopupAsync(popup);
#elif IOS
        var popup = new ProductListPopupMobile();
                popup.Action += () =>
                {
                    OnAppearing();
                };
                await this.ShowPopupAsync(popup);
#elif ANDROID
        var popup = new ProductListPopupMobile();
                popup.Action += () =>
                {
                    OnAppearing();
                };
                await this.ShowPopupAsync(popup);
#endif

        //var popup = new TestPage();
        //popup.Action += () =>
        //{
        //    OnAppearing();
        //};
        //await Navigation.PushAsync(popup);
    }
    private void OnCounterClicked(object sender, EventArgs e)
    {


    }
 
  

    private async void teklifOlustur(object sender, TappedEventArgs e)
    {
        var saleValue = (Grid)sender;
        var id = saleValue.BindingContext as Order;

#if WINDOWS
                      var popup = new ProductListPopup(null,await DatabaseService.GetOrderById(id.Id));
                    popup.Action += () =>
                    {
                        OnAppearing();
                    };
                    await this.ShowPopupAsync(popup);
#elif IOS
                        var popup = new ProductListPopupMobile(null, await DatabaseService.GetOrderById(id.Id));
                    popup.Action += () =>
                    {
                        OnAppearing();
                    };
                    await this.ShowPopupAsync(popup);
#elif ANDROID
                    var popup = new ProductListPopupMobile(null,await  DatabaseService.GetOrderById(id.Id));
                    popup.Action += () =>
                    {
                        OnAppearing();
                    };
                    await this.ShowPopupAsync(popup);
#endif


    }
    private async void sil(object sender, TappedEventArgs e)
    {
      
        var answer = await DisplayAlert("Silme Onayý", "Gerçekten silmek istiyor musunuz?", "Tamam", "Ýptal");
        if (answer)
        {
            try
            {
                IsBusy = true;
                Dispatcher.Dispatch(async() =>
                {
                    var saleValue = (Image)sender;
                    var id = saleValue.BindingContext as Order;

                    await DatabaseService.DeleteOrder(id.Id);
                    // this.ShowPopup(new DeletePopup(id.Id, "order"));
                    Dispatcher.StartTimer(TimeSpan.FromSeconds(2), () =>
                    {
                        IsBusy = false;

                        Task.Run(async () =>
                        {
                            await SecureStorage.SetAsync("OrderRefresh", "true");
                        });
                        OnAppearing();
                        return false;
                    });
                });
            }
            catch (Exception)
            {

            }
         
           
 
        }
       
    }
    

    private async void AramaYap(object sender, TappedEventArgs e)
    {

        try
        {
            var year = await SecureStorage.GetAsync("Year");
            if (!string.IsNullOrEmpty(searchbar.Text))
            {
                IsBusy = true;
                pageSize = 0;
                this.Dispatcher.DispatchAsync(async () =>
                {
                    var models = new ObservableCollection<Order>();
                    //var result = DatabaseService.GetFilterSaleAndProduct(searchbar.Text);
                    var result = await  DatabaseService.GetFilterOrderAndProductExportList(searchbar.Text, year,pageSize);
                    if (pageSize == 0 && result!=null)
                    {
                        ProdcutList.ItemsSource = null;
                        models = new ObservableCollection<Order>();
                        foreach (var item in result)
                        {
                            models.Add(item);
                        }
                        ProdcutList.ItemsSource = models;

                    }
                    else
                    {
                        if (result == null) {

                            return;
                        }
                        foreach (var item in result)
                        {
                            models.Add(item);
                        }
                        ProdcutList.ItemsSource = models;
                    }
                    loading.IsVisible = false;
                    IsBusy = false;
                    pageSize++;
                });


            }
            else
            {

                pageSize = 0;
                IsBusy = true;

                this.Dispatcher.DispatchAsync(async () =>
                {
                    var result = await DatabaseService.GetAllOrder(year, pageSize);
                    if (result != null && result.Count > 0)
                    {
                        var resultModel = new ObservableCollection<Order>();
                        //   var models = ProdcutList.ItemsSource as ObservableCollection<Sale>?? new ObservableCollection<Sale>();
                        foreach (var item in result)
                        {
                            resultModel.Add(item);
                        }

                        ProdcutList.ItemsSource = null;
                        ProdcutList.ItemsSource = resultModel;
                        pageSize++;
                    }
                    IsBusy = false;
                    loading.IsVisible = false;

                });

            }

        }
        catch (Exception)
        {

        }


    }

    private void searchbar_TextChanged(object sender, TextChangedEventArgs e)
    {

    }
    int pageSize = 0;
    bool scrollWait = false;
    private async void scrollview_Scrolled(object sender, ScrolledEventArgs e)
    {
        try
        {
            var scroll = (ScrollView)sender;
            loading.IsVisible = true;
            double tolerance = 20;
            bool isAtBottom = scroll.ScrollY >= (scroll.ContentSize.Height - scroll.Height - tolerance);


            if (isAtBottom&& !scrollWait)
                {
#if WINDOWS
                     scrollWait = false;
#else
                    scrollWait = true;
                    this.Dispatcher.StartTimer(TimeSpan.FromSeconds(4), () =>
                    {
                        scrollWait = false;
                        return false;
                    });
#endif

                var year = await SecureStorage.GetAsync("Year");
                if (!string.IsNullOrEmpty(searchbar.Text))
                {
                    this.Dispatcher.DispatchAsync(async () =>
                    {


                        var result = await DatabaseService.GetFilterOrderAndProductExportList(searchbar.Text, year, pageSize);
                        var models = ProdcutList.ItemsSource as ObservableCollection<Order> ?? new ObservableCollection<Order>();
                        if (result != null && result.Count > 0)
                        {
                            foreach (var item in result)
                            {
                                models.Add(item);
                            }
                            ProdcutList.ItemsSource = models;
                            pageSize++;

                        }
                        loading.IsVisible = false;
                    });
                }
                else
                {



                    this.Dispatcher.DispatchAsync(async () =>
                    {
                        var result = await DatabaseService.GetAllOrder(year, pageSize);
                        if (result != null && result.Count > 0)
                        {
                            var models = ProdcutList.ItemsSource as ObservableCollection<Order> ?? new ObservableCollection<Order>();
                            foreach (var item in result)
                            {
                                models.Add(item);
                            }
                     
                            ProdcutList.ItemsSource = models;
                            pageSize++;
                        }
                        loading.IsVisible = false;
                    });
                }
            }


        }
        catch (Exception ex)
        {
            await DisplayAlert("Maliyet", ex.Message, "Tamam");

        }
    }

    private void searchbar_SearchButtonPressed(object sender, EventArgs e)
    {
        AramaYap(new object(),new TappedEventArgs(""));
    }

    private void searchbar_TextChanged_1(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(searchbar.Text))
        {
            AramaYap(new object(), new TappedEventArgs(""));
        }
    }

}