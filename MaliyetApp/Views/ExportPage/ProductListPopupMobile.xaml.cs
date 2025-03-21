using CommunityToolkit.Maui.Views;
using MaliyetApp.Libs.AppServices;
using MaliyetApp.Libs.AppSettings;
using MaliyetApp.Libs.Models;
using MaliyetApp.Libs.Models.SQlitecontextDb;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
#if IOS
using UIKit;
#endif
namespace MaliyetApp.Views.ExportPage;

public partial class ProductListPopupMobile : Popup
{
	
        public Action Action;
        private Sale? _sale;
        private Order _order;
        private bool search = false;
        public ProductListPopupMobile(Sale sale = null, Order order = null)
        {
            InitializeComponent();
            Debug.WriteLine("BindingContext ayarlandý");

            _sale = sale;
            _order = order;

#if IOS
        AddTapGestureRecognizer();
#endif
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

    private bool isInitialized = false;
        private List<Sale> scrollListItem = new List<Sale>();
        private ObservableCollection<Sale> scrollObservableCollection = new ObservableCollection<Sale>();
        private ObservableCollection<Sale> orderList = new ObservableCollection<Sale>();
        protected override async void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (isInitialized) return;
            isInitialized = true;

            try
            {
                var year = await SecureStorage.GetAsync("Year");
                Dispatcher.DispatchAsync(async () =>
                {
                    List<Sale> models;
                    if (_order != null && _order.OrderDetails != null)
                    {
                        var orderSale = _order.OrderDetails.Select(a => a.Sale.Id).ToList();
                        models = await DatabaseService.GetAllSaleOrder(year, pageSize, _saleIds: orderSale);

                    }
                    else
                    {
                        models = await DatabaseService.GetAllSaleOrder(year, pageSize);

                    }

                    if (models == null || models.Count == 0)
                    {
                        if (Application.Current?.MainPage != null)
                        {
                            Application.Current.MainPage.DisplayAlert("Uyarý", "Eklenebilecek ürün kalmadý", "Tamam");
                        }
                        else
                        {

                        }
                        //return;
                    }
                    if (_order != null && _order.OrderDetails != null)
                    {
                        var orderSale = _order.OrderDetails.Select(a => a.Sale.Id).ToList();
                        models = models.Where(a => !orderSale.Contains(a.Id)).ToList();
                    }
                    foreach (var item in models)
                    {
                        scrollObservableCollection.Add(item);
                    }

                    list1.ItemsSource = scrollObservableCollection;
                    scrollListItem.AddRange(models);
                    pageSize = 0;
                    if (_order != null)
                    {
                        description.Text = _order.Description;
                        teklif.Text = _order.Title;

                        foreach (var item in _order.OrderDetails)
                        {
                            orderList.Add(item.Sale);
                            saleList.Add(item.Sale);
                        }


                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            selectList.ItemsSource = orderList;
                        });


                    }
                });
            }
            catch (Exception EX)
            {
                if (Application.Current?.MainPage != null)
                {
                    Application.Current.MainPage.DisplayAlert("Uyarý", EX.Message, "Tamam");
                }
                else
                {

                }
                return;
            }
        }
        List<Sale> saleList = new List<Sale>();
        private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {

            var unSelectedModel = (CheckBox)sender;
            if (unSelectedModel.IsChecked)
            {
                saleList.Add((Sale)unSelectedModel.BindingContext);
                Dispatcher.Dispatch(() =>
                {
                    var saleItem = (Sale)unSelectedModel.BindingContext;
                    var model = selectList.ItemsSource as ObservableCollection<Sale> ?? new ObservableCollection<Sale>();
                    if (model.Where(a => a.Id == saleItem.Id).FirstOrDefault() == null)
                    {
                        model.Add((Sale)unSelectedModel.BindingContext);
                        selectList.ItemsSource = model;
                    }

                });

            }
            else
            {
                saleList.Remove((Sale)unSelectedModel.BindingContext);
                Dispatcher.Dispatch(() =>
                {
                    var saleItem = (Sale)unSelectedModel.BindingContext;

                    var model = selectList.ItemsSource as ObservableCollection<Sale> ?? new ObservableCollection<Sale>();
                    if (model.Where(a => a.Id == saleItem.Id).FirstOrDefault() != null)
                    {
                        model.Remove((Sale)unSelectedModel.BindingContext);
                        selectList.ItemsSource = model;
                    }


                });
            }
        }

        private void OnBackgroundTapped(object sender, EventArgs e)
        {
            searchbar?.Unfocus(); // SearchBar'ý odaktan kaldýr
        }

        private async void Kaydet(object sender, EventArgs e)
        {
            try
            {

            Kaydet1.IsEnabled = false;
            Iptal1.IsEnabled = false;

            ExceptionAlert.ShowAlert("Uyarý", "Teklif oluþturuluyor");
                var year = await SecureStorage.GetAsync("Year");

                if (saleList.Count > 0)
                {
                    if (_order == null)
                    {

                        var order = new Order()
                        {

                            Description = description.Text,
                            Title = teklif.Text,
                            LastModificationTime=DateTime.UtcNow
                        };
                        var result =  await DatabaseService.CreateOrder(order, year);

                        foreach (var item in saleList)
                        {
#if WINDOWS
                                        OrderDetails detail = new OrderDetails()
                                                    {

                                                        OrderId = order.Id,
                                                        SaleId = item.Id,
                                                    };       
                                           await DatabaseService.CreateOrderDetail(detail);
    #else
                            if (!await DatabaseService.GetOrderDetailCheckSale(result, item.Id))
                            {
                                OrderDetails detail = new OrderDetails()
                                {

                                    OrderId = result,
                                    SaleId = item.Id,
                                };
                                await DatabaseService.CreateOrderDetail(detail);
                            }
#endif
                     
                        }

#if WINDOWS

#endif
                    Kaydet1.IsEnabled = false;
                    Iptal1.IsEnabled = false;


#if IOS
                     await ExportService.ExportToExcelIOS(result);

#else
                    await ExportService.ExportToExcel(result);

#endif

                    Kaydet1.IsEnabled = false;
                    Iptal1.IsEnabled = true;
                }
                else
                    {
                        _order.Description = description.Text;
                        _order.Title = teklif.Text;
                         _order.LastModificationTime = DateTime.Now;
                        await DatabaseService.UpdateOrder(_order);

                        var ids = saleList.Select(a => a.Id).ToList();

                        var deletedOrderDetail = _order.OrderDetails.Where(a => !ids.Contains(a.SaleId)).ToList();

                        foreach (var item in deletedOrderDetail)
                        {
                        await DatabaseService.DeleteOrderDetail(item.Id);
                        }
                        var orderDetailsList = _order.OrderDetails.Select(a => a.SaleId).ToList();
                        foreach (var item in saleList.Where(a => !orderDetailsList.Contains(a.Id)).ToList())
                        {
                            if (!await DatabaseService.GetOrderDetailCheckSale(_order.Id,item.Id))
                            {
                                OrderDetails detail = new OrderDetails()
                                {
                                    OrderId = _order.Id,
                                    SaleId = item.Id,
                                };
                                await DatabaseService.CreateOrderDetail(detail);
                            }
                        }
                        Kaydet1.IsEnabled = false;
                        Iptal1.IsEnabled = false;

#if IOS
                     await ExportService.ExportToExcelIOS(_order.Id);

#else
                    await ExportService.ExportToExcel(_order.Id);

#endif

                    Kaydet1.IsEnabled = false;
                        Iptal1.IsEnabled = true;
                }




                }
           
                await SecureStorage.SetAsync("OrderRefresh", "true");
            
                 Action?.Invoke();
                await this.CloseAsync();
            }
            catch (Exception ex)
            {

                Kaydet1.IsEnabled = true;
                Iptal1.IsEnabled = true;
                ExceptionAlert.ShowAlert("Excel hatasý", ex.Message);
                Action?.Invoke();
                await this.CloseAsync();
            }
        }

        private async void Iptal(object sender, EventArgs e)
        {
            Action?.Invoke();
            await this.CloseAsync();
        }

        private void CheckBox_BindingContextChanged(object sender, EventArgs e)
        {
            try
            {
                var senderValue2 = (CheckBox)sender;

                this.Dispatcher.Dispatch(() =>
                {
                    var checkboxSale = (Sale)senderValue2.BindingContext;
                    if (_order != null)
                    {
                        if (_order.OrderDetails.Where(a => a.SaleId == checkboxSale.Id).FirstOrDefault() != null)
                        {
                            senderValue2.IsChecked = true;
                        }
                    }
                    if (search)
                    {
                        if (saleList.Where(a => a.Id == checkboxSale.Id).FirstOrDefault() != null)
                        {
                            senderValue2.IsChecked = true;
                        }
                    }
                });
            }
            catch (Exception)
            {

            }



        }

        private async void Aramayap(object sender, TappedEventArgs e)
        {
            try
            {
                finish = true;
                var year = await SecureStorage.GetAsync("Year");
                if (!String.IsNullOrEmpty(searchbar.Text))
                {
                    pageSize = 0;
                    search = true;
                    List<int> ids = new List<int>();
                    if (saleList != null && saleList.Count > 0)
                    {
                        ids = saleList.Select(a => a.Id).ToList();

                    }
                    var model = await DatabaseService.GetFilterSaleAndProductPopup(searchbar.Text, year, pageSize, _saleIds: ids);
                    scrollObservableCollection = new ObservableCollection<Sale>() ?? new ObservableCollection<Sale>();


                    if (model == null || model.Count == 0)
                    {
                        if (Application.Current?.MainPage != null)
                        {
                            Application.Current.MainPage.DisplayAlert("Uyarý", "Aranan içerik bulunamadý", "Tamam");
                        }
                        else
                        {

                        }
                        list1.ItemsSource = null;
                        return;
                    }
                    if (saleList != null && saleList.Count > 0 && model != null)
                    {
                        var orderSale = saleList.Select(a => a.Id).ToList();
                        model = model.Where(a => !orderSale.Contains(a.Id)).ToList();
                    }

                    foreach (var item in model)
                    {
                        scrollObservableCollection.Add(item);

                    }
                    list1.ItemsSource = null;
                    list1.ItemsSource = scrollObservableCollection;
                    pageSize = 0;

                }
                else
                {
                    Dispatcher.DispatchAsync(async() =>
                    {
                        pageSize = 0;
                        search = true;
                        List<Sale> model;
                        if (saleList != null && saleList.Count > 0)
                        {
                            model = await DatabaseService.GetAllSaleOrder(year, pageSize, _saleIds: saleList.Select(a => a.Id).ToList());
                        }
                        else
                        {
                            model = await DatabaseService.GetAllSaleOrder(year, pageSize);

                        }

                        scrollObservableCollection = new ObservableCollection<Sale>() ?? new ObservableCollection<Sale>();
                        if (model == null)
                        {
                            return;
                        }
                        if (saleList != null && saleList.Count > 0 && model != null)
                        {
                            var orderSale = saleList.Select(a => a.Id).ToList();
                            model = model.Where(a => !orderSale.Contains(a.Id)).ToList();
                        }


                        foreach (var item in model)
                        {
                            scrollObservableCollection.Add(item);

                        }
                        list1.ItemsSource = null;
                        list1.ItemsSource = scrollObservableCollection;
                        pageSize = 0;
                        //list1.ItemsSource = model;


                    });
                }
            }
            catch (Exception EX)
            {
                if (Application.Current?.MainPage != null)
                {
                    Application.Current.MainPage.DisplayAlert("Uyarý", EX.Message, "Tamam");
                }
                else
                {

                }
                return;
            }
        }



        int pageSize = 0;
        bool waitScroll = true;
        bool finish = false;
        private async void list1_Scrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            // CollectionView öðesini alýn
            var scroll = (CollectionView)sender;

            if (false)
            {
                waitScroll = false;
                var year = await SecureStorage.GetAsync("Year");
                if (!string.IsNullOrEmpty(searchbar.Text))
                {
                    // Arama metni varsa filtreleme iþlemi
                    this.Dispatcher.DispatchAsync(async() =>
                    {
                        var result = await DatabaseService.GetFilterSaleAndProduct(searchbar.Text, year, pageSize, 1);

                        var models = list1.ItemsSource as ObservableCollection<Sale> ?? new ObservableCollection<Sale>();
                        if (result != null && result.Count > 0)
                        {

                            foreach (var item in result)
                            {
                                models.Add(item);
                            }
                            scrollListItem.AddRange(models); // scroll için

                            int targetIndex = models.Count - 10;
                            if (targetIndex >= 0)
                            {
                                scroll.ScrollTo(targetIndex, position: ScrollToPosition.Center, animate: true);
                            }
                            list1.ItemsSource = models;
                            pageSize++;
                        }
                        else
                        {
                            //finish = true;
                        }
                    });
                }
                else
                {
                    // Arama metni yoksa tüm verileri yükleme
                    this.Dispatcher.DispatchAsync(async() =>
                    {
                        var result = await DatabaseService.GetAllSaleMain(year, pageSize);
                        if (result != null && result.Count > 0)
                        {
                            var models = list1.ItemsSource as ObservableCollection<Sale> ?? new ObservableCollection<Sale>();
                            foreach (var item in result)
                            {
                                models.Add(item);

                            }
                            int targetIndex = models.Count - 10;
                            if (targetIndex >= 0)
                            {
                                scroll.ScrollTo(targetIndex, position: ScrollToPosition.Center, animate: true);
                            }
                            scrollListItem.AddRange(models); // scroll için
                                                             // list1.ItemsSource = models;
                            pageSize++;
                        }
                        else
                        {
                            // finish = true;
                        }
                    });
                }
                Dispatcher.StartTimer(TimeSpan.FromSeconds(2), () =>
                {
                    waitScroll = true;
                    return false;
                });
            }
        }

        private async void Geri(object sender, EventArgs e)
        {
            var year = await SecureStorage.GetAsync("Year");
            Debug.WriteLine($"GERÝ: {pageSize}");
            pageSize--;

            if (pageSize < 0)
            {
                pageSize = 0;
                return;
            }

            Dispatcher.StartTimer(TimeSpan.FromSeconds(3), () =>
            {
                geri.IsEnabled = true;
                ileri.IsEnabled = true;
                return false;
            });
            geri.IsEnabled = false;
            ileri.IsEnabled = false;
            await UpdateListData(searchbar.Text, year, pageSize);
        }

        private async void Ileri(object sender, EventArgs e)
        {
            var year = await SecureStorage.GetAsync("Year");
            Debug.WriteLine($"ÝLERÝ: {pageSize}");
            pageSize++;
            Dispatcher.StartTimer(TimeSpan.FromSeconds(2), () =>
            {
                geri.IsEnabled = true;
                ileri.IsEnabled = true;
                return false;
            });
            geri.IsEnabled = false;
            ileri.IsEnabled = false;
            await UpdateListData(searchbar.Text, year, pageSize);
        }

        /// <summary>
        /// Listeyi günceller ve verileri getirir.
        /// </summary>
        /// <param name="searchText">Arama metni</param>
        /// <param name="year">Yýl bilgisi</param>
        /// <param name="pageSize">Sayfa boyutu</param>
        private async Task UpdateListData(string searchText, string year, int pageSize)
        {
            try
            {
                //this.Dispatcher.Dispatch(() =>
                //{

                ObservableCollection<Sale> dataCollection;

                if (!string.IsNullOrEmpty(searchText))
                {
                    search = true;
                    List<int> ids = new List<int>();

                    if (saleList != null && saleList.Count > 0)
                    {
                        ids = saleList.Select(a => a.Id).ToList();

                    }
                    var filteredData = await DatabaseService.GetFilterSaleAndProductPopup(searchText, year, pageSize, _saleIds: ids);
                    if (saleList != null && saleList.Count > 0 && filteredData != null)
                    {
                        var orderSale = saleList.Select(a => a.Id).ToList();
                        filteredData = filteredData.Where(a => !orderSale.Contains(a.Id)).ToList();
                    }
                    if (filteredData == null)
                    {
                        if (Application.Current?.MainPage != null)
                        {
                            Application.Current.MainPage.DisplayAlert("Uyarý", "Tüm içerik bu kadar", "Tamam");
                        }
                        else
                        {

                        }
                        return;
                    }
                    dataCollection = new ObservableCollection<Sale>(filteredData);
                }
                else
                {
                    search = true;

                    //  var allData = DatabaseService.GetAllSaleMain(year, pageSize);
                    List<int> ids = new List<int>();

                    if (saleList != null && saleList.Count > 0)
                    {
                        ids = saleList.Select(a => a.Id).ToList();

                    }
                    var allData = await DatabaseService.GetAllSaleOrder(year, pageSize, _saleIds: ids);
                    if (allData == null || allData.Count == 0)
                    {
                        if (Application.Current?.MainPage != null)
                        {
                            Application.Current.MainPage.DisplayAlert("Uyarý","Ýçerik  Kalmadý", "Tamam");
                        }
                        else
                        {

                        }
                    return;
                    }

                    if (saleList != null && saleList.Count > 0)
                    {
                        var orderSale = saleList.Select(a => a.Id).ToList();
                        allData = allData.Where(a => !orderSale.Contains(a.Id)).ToList();
                    }
                    dataCollection = new ObservableCollection<Sale>(allData);
                    scrollListItem.AddRange(allData); // Scroll için ekleme
                }

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    list1.ItemsSource = null;
                    list1.ItemsSource = dataCollection;
                });
                //await MainThread.InvokeOnMainThreadAsync(() =>
                //        {
                //                     list1.ItemsSource = null;
                //                    list1.ItemsSource = dataCollection;
                //        });

                // Sayfa boyutunu artýrma koþulu
                if (dataCollection.Count > 19)
                {
                    pageSize++;
                }
                //});
            }
            catch (Exception EX)
            {
                if (Application.Current?.MainPage != null)
                {
                    Application.Current.MainPage.DisplayAlert("Uyarý", EX.Message, "Tamam");
                }
                else
                {

                }
                return;
            }
        }

        private async void ListeyeEkle(object sender, EventArgs e)
        {
            try
            {
                var button = (Button)sender;
                var context = (Sale)button.BindingContext;
                saleList.Add(context);
                //Dispatcher.DispatchAsync(async () =>
                //{

                //});
                await Task.Delay(1000);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var model = selectList.ItemsSource as ObservableCollection<Sale> ?? new ObservableCollection<Sale>();
                    if (model.Where(a => a.Id == context.Id).FirstOrDefault() == null)
                    {
                        if (model == null || model.Count == 0)
                        {
                            model.Add(context);
                            selectList.ItemsSource = model;
                        }
                        else
                        {

                            model.Add(context);
                        }
                        //selectList.ItemsSource = model;

                    }
                });

            }
            catch (Exception EX)
            {
                if (Application.Current?.MainPage != null)
                {
                    Application.Current.MainPage.DisplayAlert("Uyarý", EX.Message, "Tamam");
                }
                else
                {

                }
                return;
            }

        }
        private void ListedenCikar(object sender, EventArgs e)
        {
            try
            {
                var button = (Button)sender;
                var sale = (Sale)button.BindingContext;
                Dispatcher.Dispatch(() =>
                {
                    saleList.Remove(sale);
                    var model = selectList.ItemsSource as ObservableCollection<Sale> ?? new ObservableCollection<Sale>();
                    model.Remove(sale);
                    selectList.ItemsSource = model;

                });
            }
            catch (Exception EX)
            {
                if (Application.Current?.MainPage != null)
                {
                    Application.Current.MainPage.DisplayAlert("Uyarý", EX.Message, "Tamam");
                }
                else
                {

                }
                return;

            }
        }

    private async void searchbar_SearchButtonPressed(object sender, EventArgs e)
    {
        try
        {
            finish = true;
            var year = await SecureStorage.GetAsync("Year");
            if (!String.IsNullOrEmpty(searchbar.Text))
            {
                pageSize = 0;
                search = true;
                List<int> ids = new List<int>();
                if (saleList != null && saleList.Count > 0)
                {
                    ids = saleList.Select(a => a.Id).ToList();

                }
                var model = await DatabaseService.GetFilterSaleAndProductPopup(searchbar.Text, year, pageSize, _saleIds: ids);
                scrollObservableCollection = new ObservableCollection<Sale>() ?? new ObservableCollection<Sale>();


                if (model == null || model.Count == 0)
                {
                    if (Application.Current?.MainPage != null)
                    {
                        Application.Current.MainPage.DisplayAlert("Uyarý", "Aranan içerik bulunamadý", "Tamam");
                    }
                    else
                    {

                    }
                    list1.ItemsSource = null;
                    return;
                }
                if (saleList != null && saleList.Count > 0 && model != null)
                {
                    var orderSale = saleList.Select(a => a.Id).ToList();
                    model = model.Where(a => !orderSale.Contains(a.Id)).ToList();
                }

                foreach (var item in model)
                {
                    scrollObservableCollection.Add(item);

                }
                list1.ItemsSource = null;
                list1.ItemsSource = scrollObservableCollection;
                pageSize = 0;

            }
            else
            {
                Dispatcher.DispatchAsync(async () =>
                {
                    pageSize = 0;
                    search = true;
                    List<Sale> model;
                    if (saleList != null && saleList.Count > 0)
                    {
                        model = await DatabaseService.GetAllSaleOrder(year, pageSize, _saleIds: saleList.Select(a => a.Id).ToList());
                    }
                    else
                    {
                        model = await DatabaseService.GetAllSaleOrder(year, pageSize);

                    }

                    scrollObservableCollection = new ObservableCollection<Sale>() ?? new ObservableCollection<Sale>();
                    if (model == null)
                    {
                        return;
                    }
                    if (saleList != null && saleList.Count > 0 && model != null)
                    {
                        var orderSale = saleList.Select(a => a.Id).ToList();
                        model = model.Where(a => !orderSale.Contains(a.Id)).ToList();
                    }


                    foreach (var item in model)
                    {
                        scrollObservableCollection.Add(item);

                    }
                    list1.ItemsSource = null;
                    list1.ItemsSource = scrollObservableCollection;
                    pageSize = 0;
                    //list1.ItemsSource = model;


                });
            }
        }
        catch (Exception EX)
        {
            if (Application.Current?.MainPage != null)
            {
                Application.Current.MainPage.DisplayAlert("Uyarý", EX.Message, "Tamam");
            }
            else
            {

            }
            return;
        }

    }
    private async  void searchbar_TextChanged_1(object sender, TextChangedEventArgs e)
    {
        if (String.IsNullOrEmpty(e.NewTextValue))
        {
            finish = true;
                 var year = await SecureStorage.GetAsync("Year");
                
                Dispatcher.DispatchAsync( async() =>
                {
                    pageSize = 0;
                    search = true;
                    List<Sale> model;
                    if (saleList != null && saleList.Count > 0)
                    {
                        model =await DatabaseService.GetAllSaleOrder(year, pageSize, _saleIds: saleList.Select(a => a.Id).ToList());
                    }
                    else
                    {
                        model = await DatabaseService.GetAllSaleOrder(year, pageSize);

                    }

                    scrollObservableCollection = new ObservableCollection<Sale>() ?? new ObservableCollection<Sale>();
                    if (model == null)
                    {
                        return;
                    }
                    if (saleList != null && saleList.Count > 0 && model != null)
                    {
                        var orderSale = saleList.Select(a => a.Id).ToList();
                        model = model.Where(a => !orderSale.Contains(a.Id)).ToList();
                    }


                    foreach (var item in model)
                    {
                        scrollObservableCollection.Add(item);

                    }
                    list1.ItemsSource = null;
                    list1.ItemsSource = scrollObservableCollection;
                    pageSize = 0;
                    //list1.ItemsSource = model;


                });
        }
    }
}