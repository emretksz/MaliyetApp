using DocumentFormat.OpenXml.Bibliography;
using MaliyetApp.Libs.AppServices;
using MaliyetApp.Libs.AppSettings;
using MaliyetApp.Libs.Models;
using MaliyetApp.Libs.Models.SQlitecontextDb;
using MaliyetApp.Views.ProductPage;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using static SQLite.SQLite3;
#if IOS
using UIKit;
#endif

namespace MaliyetApp
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        ObservableCollection<Sale> models = new ObservableCollection<Sale>();
        string baseApiUrl = AppConst.ImageAddress;
        string imagePath = AppConst.ImagePath;

        public MainPage()
        {
            InitializeComponent();
            pageSize = 0;

            Dispatcher.DispatchAsync(() =>
            {
                scrollview.Scrolled += scrollview_Scrolled;
                // ExportService.ExportToExcel();
#if IOS
 AddTapGestureRecognizer();
#endif

                //var result = DatabaseService.GetAllSaleMainNewMethod(year, pageSize); 
                //if (result != null && result.Count > 0)
                //{
                //    foreach (var item in result)
                //    {
                //        models.Add(item); // Yeni verileri doğrudan ekle
                //    }
                //    ProdcutList.ItemsSource = models; // İlk atama
                //    pageSize++;
                //}

            });

        }
        List<Product> productList = new List<Product>();
        List<Sale> saleList = new List<Sale>();
        protected  override async void OnAppearing()
        {
            base.OnAppearing();

            var mainRefresh = await SecureStorage.GetAsync("MainRefresh");

            bool refresh;

            if (String.IsNullOrEmpty(mainRefresh))
                refresh = true;
            else
            {
              var x=  (Boolean.TryParse(mainRefresh, out refresh));
            }

            if (refresh)
            {

                pageSize = 0;
                var year = await SecureStorage.GetAsync("Year");

                IsBusy = true;
                //Dispatcher.StartTimer(TimeSpan.FromSeconds(2), () =>
                //{
                //});
                    var result = await DatabaseService.GetAllSaleMainNewMethod(year, pageSize);
                    models = new ObservableCollection<Sale>();
                    if (result != null && result.Count > 0)
                    {
                        foreach (var item in result)
                        {
#if ANDROID
                        item.Product.ImageURL = $"{imagePath}/{item.Product.ImageURL}";
#elif IOS
                        item.Product.ImageURL = $"{imagePath}/{item.Product.ImageURL}";
#endif
                        models.Add(item); // Yeni verileri doğrudan ekle
                        }

#if WINDOWS
                    Dispatcher.DispatchAsync(() =>
                    {
                        ProdcutList.ItemsSource = models; // İlk atama
                    });

#else

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        ProdcutList.ItemsSource = models; // İlk atama
                    });
#endif



                    pageSize++;

                    }
                    IsBusy = false;

                    Task.Run(async () =>
                    {
                        await SecureStorage.SetAsync("MainRefresh", "false");
                    });
                   // return false;
            }
            //ekleme silme güncellemede cache de veri tutup burayı güncellememiz lazım




            //this.Dispatcher.DispatchAsync(async() =>
            //{
            //    var products = await SecureStorage.GetAsync("ProductList");
            //    var sales = await SecureStorage.GetAsync("SaleList");

            //    if (products != null)
            //    {
            //        productList = JsonSerializer.Deserialize<List<Product>>(products);
            //    }

            //    if (sales != null)
            //    {
            //        saleList = JsonSerializer.Deserialize<List<Sale>>(sales);
            //    }

            //    // Product ve Sale eşleşmelerini yap
            //    if (productList.Count > 0 && saleList.Count > 0)
            //    {
            //        foreach (var sale in saleList)
            //        {
            //            sale.Product = productList.FirstOrDefault(p => p.Id == sale.ProductId);
            //        }

            //        ProdcutList.ItemsSource = saleList;
            //    }

            //});
        }
        private void OnCounterClicked(object sender, EventArgs e)
        {
           

        }
        private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            var imagesender = (Grid)sender;

            var id = imagesender.BindingContext as Sale;
            await Navigation.PushAsync(new UpdateProduct(id.Id));
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreateProduct());
        }

        private async void TapGestureRecognizer_Tapped_1(object sender, TappedEventArgs e)
        {
            var imagesender = (Grid)sender;
            var id = imagesender.BindingContext as Sale;
            await Navigation.PushAsync(new UpdateProduct(id.Id));
        }
        private async void LoadData(string searchtext)
        {
            var year = await SecureStorage.GetAsync("Year");
            if (!string.IsNullOrEmpty(searchtext))
            {
                IsBusy = true;
                this.Dispatcher.DispatchAsync(async () =>
                {
                    var result =await DatabaseService.GetFilterSaleAndProductNewMothod(searchtext, year, pageSize);

                    // var models = ProdcutList.ItemsSource as List<Sale>;
                    var models = ProdcutList.ItemsSource as ObservableCollection<Sale> ?? new ObservableCollection<Sale>();

                    if (result!=null &&result.Count > 0) {

                        if (pageSize==0 && !String.IsNullOrEmpty(searchtext))
                        {
                            ProdcutList.ItemsSource = null;
                            models= new ObservableCollection<Sale>(); 
                            foreach (var item in result)
                            {
#if ANDROID
                                item.Product.ImageURL = $"{imagePath}/{item.Product.ImageURL}";
#elif IOS
                                item.Product.ImageURL = $"{imagePath}/{item.Product.ImageURL}";
#endif

                                models.Add(item);
                            }
                            ProdcutList.ItemsSource = models;
                            loading.IsVisible = false;
                        }
                        else
                        {
                            foreach (var item in result)
                            {
#if ANDROID
                                item.Product.ImageURL = $"{imagePath}/{item.Product.ImageURL}";
#elif IOS
                                item.Product.ImageURL = $"{imagePath}/{item.Product.ImageURL}";
#endif

                                models.Add(item);
                            }
                            ProdcutList.ItemsSource = models;
                            loading.IsVisible = false;
                        }
                       
                         pageSize++;

                    }
                    loading.IsVisible = false;
                    IsBusy = false;
                });
            }
            else
            {

                IsBusy = true;
                
                this.Dispatcher.DispatchAsync(async() =>
                {
                    var result =await  DatabaseService.GetAllSaleMainNewMethod(year, pageSize);
                    if (result != null && result.Count>0)
                    {
                        var resultModel = new ObservableCollection<Sale>();
                     //   var models = ProdcutList.ItemsSource as ObservableCollection<Sale>?? new ObservableCollection<Sale>();
                        foreach (var item in result)
                        {
#if ANDROID
                                                        item.Product.ImageURL = $"{imagePath}/{item.Product.ImageURL}";

#elif IOS
                                                           item.Product.ImageURL = $"{imagePath}/{item.Product.ImageURL}";

#endif

                            resultModel.Add(item);
                        }
                    
                        ProdcutList.ItemsSource = null;
                        ProdcutList.ItemsSource = resultModel;
                        pageSize++;
                        loading.IsVisible = false;
                    }
                    IsBusy = false;
                    loading.IsVisible = false;
                });
            }
        }
        private string searchKey ="";
        private async void AramaYap(object sender, TappedEventArgs e)
        {

            if (!string.IsNullOrEmpty(searchbar.Text))
            {
                pageSize = 0;
                LoadData(searchbar.Text);
                searchKey = searchbar.Text;
                //IsBusy = true;

                //var year = await SecureStorage.GetAsync("Year");


                //this.Dispatcher.Dispatch(() =>
                //{
                //    var result = DatabaseService.GetFilterSaleAndProduct(searchbar.Text, year);
                //    ProdcutList.ItemsSource = null;
                //    ProdcutList.ItemsSource = result;
                //    IsBusy = false;
                //});


            }
            else
            {
                pageSize = 0;
                LoadData("");
                searchKey = "";
                //IsBusy = true;
                //var year = await SecureStorage.GetAsync("Year");
                //this.Dispatcher.Dispatch(() =>
                //{
                //    pageSize = 0;
                //    var result = DatabaseService.GetAllSaleMain(year, pageSize);
                //    ProdcutList.ItemsSource = null;
                //    ProdcutList.ItemsSource = result;
                //    IsBusy = false;
                //});
            }
           


        }

        private void searchbar_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private async void Sil(object sender, TappedEventArgs e)
        {
            try
            {
                var imagesender = (Image)sender;

                var id = imagesender.BindingContext as Sale;


                var answer = await DisplayAlert("Silme İşlemi", "Ürün silinecek,bu ürünü içeren teklifler de silinecek", "Tamam", "Iptal");
                if (answer)
                {
                    IsBusy = true;
                    await DatabaseService.DeleteSale(id.Id);
                    DisplayAlert("Silme İşlemi", "Ürünler siliniyor... ", "Tamam");
                    Dispatcher.StartTimer(TimeSpan.FromSeconds(4), () =>
                    {

                        Dispatcher.Dispatch(() =>
                        {
                            var sonuc = ProdcutList.ItemsSource as ObservableCollection<Sale>;
                            sonuc.Remove(id);
                            IsBusy = false;
                        });

                        // LoadData(searchKey);
                        return false;
                    });
                }
            }
            catch (Exception)
            {
                IsBusy = false;
            }
            
        }
        private async void DownloadSale(object sender, TappedEventArgs e)
        {
            var imagesender = (Image)sender;

            var id = imagesender.BindingContext as Sale;


            var answer = await DisplayAlert("Maliyet Hazırlıyor", "İndirme işlemi başladı", "Tamam", "Iptal");
            if (answer)
            {
                await ExportService.ExportToSaleExcel(id.Id);
            }

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
                    if (!string.IsNullOrEmpty(searchKey))
                    {
                        this.IsBusy = true;
                        this.Dispatcher.DispatchAsync( async() =>
                        {


                            var result = await DatabaseService.GetFilterSaleAndProductNewMothod(searchKey, year, pageSize);

                            var models = ProdcutList.ItemsSource as ObservableCollection<Sale> ?? new ObservableCollection<Sale>();
                            if (result != null && result.Count > 0)
                            {
                                foreach (var item in result)
                                {



#if ANDROID
                                                                 item.Product.ImageURL = $"{imagePath}/{item.Product.ImageURL}";

#elif IOS
                                                                  item.Product.ImageURL = $"{imagePath}/{item.Product.ImageURL}";

#endif
                                    models.Add(item);
                                }
                                //ProdcutList.ItemsSource = models;
                                pageSize++;

                            }
                            this.IsBusy = false;
                            loading.IsVisible = false;
                        });
                    }
                    else
                    {



                        this.Dispatcher.DispatchAsync(async() =>
                        {
                            this.IsBusy = true;
                            var result = await DatabaseService.GetAllSaleMainNewMethod(year, pageSize);
                            if (result != null && result.Count > 0)
                            {
                                var models = ProdcutList.ItemsSource as ObservableCollection<Sale> ?? new ObservableCollection<Sale>();
                                foreach (var item in result)
                                {


#if ANDROID
                                                              item.Product.ImageURL = $"{imagePath}/{item.Product.ImageURL}";

#elif IOS
                                                               item.Product.ImageURL = $"{imagePath}/{item.Product.ImageURL}";

#endif

                                    models.Add(item);
                                }
                                //  models.AddRange(result);
                                // ProdcutList.ItemsSource = null;

                                //ProdcutList.ItemsSource = models; //sildim
                                pageSize++;
                            }
                            this.IsBusy = false;
                            loading.IsVisible = false;
                        });
                    }
                }


            }
            catch (Exception ex)
            {
                await DisplayAlert("Maliyet Hazırlıyor", ex.Message, "Tamam");

            }
        }

        private void searchbar_SearchButtonPressed(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(searchbar.Text))
            {
                pageSize = 0;
                LoadData(searchbar.Text);
                searchKey = searchbar.Text;
                //IsBusy = true;

                //var year = await SecureStorage.GetAsync("Year");


                //this.Dispatcher.Dispatch(() =>
                //{
                //    var result = DatabaseService.GetFilterSaleAndProduct(searchbar.Text, year);
                //    ProdcutList.ItemsSource = null;
                //    ProdcutList.ItemsSource = result;
                //    IsBusy = false;
                //});


            }
            else
            {
                pageSize = 0;
                LoadData("");
                searchKey = "";
                //IsBusy = true;
                //var year = await SecureStorage.GetAsync("Year");
                //this.Dispatcher.Dispatch(() =>
                //{
                //    pageSize = 0;
                //    var result = DatabaseService.GetAllSaleMain(year, pageSize);
                //    ProdcutList.ItemsSource = null;
                //    ProdcutList.ItemsSource = result;
                //    IsBusy = false;
                //});
            }

        }

        private void searchbar_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.NewTextValue))
            {
                pageSize = 0;
                LoadData("");
                searchKey = "";
            }
        }

      
        private void searchbar_Unfocused(object sender, FocusEventArgs e)
        {

//#if IOS
//                        if (sender is SearchBar searchBar)
//                        {
//                            if (searchBar.Handler?.PlatformView is UISearchBar nativeSearchBar)
//                            {
//                                nativeSearchBar.ResignFirstResponder(); // iOS'ta klavyeyi kapat
//                            }
//                        }
//#endif
        }
#if IOS
                 private void AddTapGestureRecognizer()
    {
        // Sayfanın içeriğine dokunma algılayıcısı ekle
        if (this.Handler?.PlatformView is UIView pageView)
        {
            var tapGestureRecognizer = new UITapGestureRecognizer(() =>
            {
                // Klavyeyi kapat
                CloseKeyboard();
            });

            // Dokunma algılayıcısını sayfanın içeriğine ekle
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
    }

}
