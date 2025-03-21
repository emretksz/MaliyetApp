using CommunityToolkit.Maui.Views;
using MaliyetApp.Libs.AppSettings;
using MaliyetApp.Libs.Handlers;
using MaliyetApp.Libs.Models;
using MaliyetApp.Libs.Models.SQlitecontextDb;
using MaliyetApp.ViewModels;
using MaliyetApp.Views.OrderPage;
using Microsoft.Maui.Dispatching;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Text.Json;

namespace MaliyetApp.Views.ProductPage;

public partial class CreateProduct : ContentPage
{
    public MaterialListViewModel ViewModel { get; set; }
    Product product = new Product();
    Sale sale = new Sale();
    List<SaleDetail> SaleDetailDto = new List<SaleDetail>();
    string baseApiUrl = AppConst.ImageAddress;
    public CreateProduct()
    {
        InitializeComponent();




            //ViewModel = new MaterialListViewModel();
            //this.BindingContext = ViewModel;
            ////#if WINDOWS
            ////            Shell.SetNavBarIsVisible(this, true);
            ////#endif
       

            //toggleSwitch.Toggled += (s, e) =>
            //{
            //    bool isToggled = e.Value;
            //    if (isToggled)
            //    {
            //        product1.IsVisible = true;
            //        Sales1.IsVisible = false;
            //    }
            //    else
            //    {
            //        product1.IsVisible = false;
            //        Sales1.IsVisible = true;
            //    }
            //};

    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        #if WINDOWS
                var backButtonBehavior = new BackButtonBehavior
                {
                    IsEnabled = true,
                    IconOverride = "back_icon.png", // İkon dosyası projenizde mevcut olmalı
                    Command = new Command(async () =>
                    {
                        // Geri gitmek için yapılacak işlem
                        await Shell.Current.GoToAsync("..");
                    })
                };

                Shell.SetBackButtonBehavior(this, backButtonBehavior);
        #endif
        try
        {
            //var dolar = await SecureStorage.GetAsync("Dolar");
            //var euro = await SecureStorage.GetAsync("Euro");

            //float dolarKuru = string.IsNullOrEmpty(dolar) ? 0f : float.Parse(dolar);
            //float euroKuru = string.IsNullOrEmpty(euro) ? 0f : float.Parse(euro);

            //if (dolarKuru > 0)
            //{
            //    Dolar_.Text = dolarKuru.ToString();
            //}
            //if (euroKuru > 0)
            //{
            //    Euro_.Text = euroKuru.ToString();
            //}
        }
        catch (Exception ex)
        {

        }


    }
    #region Dolar Euro texchange
    private async void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        //euro
        try
        {
            if (!string.IsNullOrEmpty(e.NewTextValue))
            {
                // Sayı kontrolü yapılıyor
                if (double.TryParse(e.NewTextValue, out double parsedValue))
                {
                    // Eğer sayı ise depola
                    await SecureStorage.SetAsync("Euro", e.NewTextValue);
                }
                else
                {
                    DisplayAlert("Uyarı", "Harf yazmayın", "tamam");
                }
            }
        }
        catch (Exception ex)
        {
            // Hata mesajı
            Console.WriteLine($"Hata: {ex.Message}");
        }
    }

    private async void Entry_TextChanged_1(object sender, TextChangedEventArgs e)
    {
        //Dolar
      
        try
        {
            if (!string.IsNullOrEmpty(e.NewTextValue))
            {
                // Sayı kontrolü yapılıyor
                if (double.TryParse(e.NewTextValue, out double parsedValue))
                {
                    // Eğer sayı ise depola
                    await SecureStorage.SetAsync("Dolar", e.NewTextValue);
                }
                else
                {
                    // Gelen metin sayı değilse uyarı verebilirsiniz
                    DisplayAlert("Uyarı", "Harf yazmayın", "tamam");
                }
            }
        }
        catch (Exception ex)
        {
            // Hata mesajı
            Console.WriteLine($"Hata: {ex.Message}");
        }


    }


    #endregion



    #region Resim Yükle

    string originalFilePath = "";
    string copiedFilePath = "";
    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        try
        {
            FileResult fileResult = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Bir resim seçin",
                FileTypes = FilePickerFileType.Images
            });

            if (fileResult != null)
            {
                // Dosya yolunu alın
#if IOS
                        // Seçilen dosyanın orijinal yolu
                     try 
	{	        
		     string originalFilePath = fileResult.FullPath;

                        // Uygulamanın özel dizinine dosyayı kopyala
                        string appDirectory = FileSystem.AppDataDirectory; // Uygulamanın özel dizini
                        string fileName = Path.GetFileName(originalFilePath);
                         copiedFilePath = Path.Combine(appDirectory, fileName);

                        using (var sourceStream = File.OpenRead(originalFilePath))
                        using (var destinationStream = File.Create(copiedFilePath))
                        {
                            await sourceStream.CopyToAsync(destinationStream);
                        }

                        // Kopyalanan dosyayı kullan
                        this.Dispatcher.Dispatch(() =>
                        {
                            SelectedImage.Source = ImageSource.FromStream(() => File.OpenRead(copiedFilePath));
                        });

	}
	catch (global::System.Exception EX)
	{


	}
                        // Sunucuya yükleme işlemi
                     
#else
                originalFilePath = fileResult.FullPath;
                            this.Dispatcher.Dispatch(() =>
                            {
                                // Resmi `Image` kontrolünde göster
                                SelectedImage.Source = ImageSource.FromStream(() => File.OpenRead(originalFilePath));

                            });
            #endif

            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Resim seçilemedi: {ex.Message}", "Tamam");
        }
    }
#endregion


    #region Malzeme Popup

    private List<SaleDetail> _tempPopupMaterial = new List<SaleDetail>();
    private async void OnSubmitPopup(object sender, EventArgs e)
    {
        try
        {
            var popup = new MaterialPopup(sale.Id, _tempPopupMaterial);
//#if WINDOWS

//            popup.ListAction += (returnlist) =>
//            {
//                this.Dispatcher.Dispatch(() =>
//                {

//                    var viewModel = new MaterialListViewModel
//                    {
//                        Sale = new ObservableCollection<SaleDetail>(returnlist)
//                    };


//                    if (_tempPopupMaterial.Count > 0)
//                    {
//                        foreach (var item in _tempPopupMaterial)
//                        {
//                            viewModel.Sale.Add(item);
//                        }
//                        _tempPopupMaterial.AddRange(returnlist);
//                        _tempPopupMaterial.ForEach(x => x.SaleId = sale.Id);
//                    }
//                    else
//                    {
//                        _tempPopupMaterial.AddRange(viewModel.Sale.ToList());
//                        _tempPopupMaterial.ForEach(x => x.SaleId = sale.Id);
//                    }
//                    materialList.ItemsSource = viewModel.Sale;

//                    BindingContext = viewModel;
//                });

//            };

//#elif ANDROID
//#else
//#endif
            popup.ListAction += (returnlist) =>
            {
                this.Dispatcher.DispatchAsync(() =>
                {
                    IsBusy = true;
                    try
                    {
                        var viewModel = new MaterialListViewModel
                        {
                            Sale = new ObservableCollection<SaleDetail>(returnlist)
                        };


                        if (_tempPopupMaterial.Count > 0)
                        {
                            foreach (var item in _tempPopupMaterial)
                            {
                                viewModel.Sale.Add(item);
                            }
                            _tempPopupMaterial.AddRange(returnlist);
                            _tempPopupMaterial.ForEach(x => x.SaleId = sale.Id);
                        }
                        else
                        {
                            _tempPopupMaterial.AddRange(viewModel.Sale.ToList());
                            _tempPopupMaterial.ForEach(x => x.SaleId = sale.Id);
                        }
                        materialList.ItemsSource = viewModel.Sale;

                        BindingContext = viewModel;
                        IsBusy = false;
                    }
                    catch (Exception)
                    {
                        IsBusy = false;
                    }
                });

            };


            //popup.ListAction += async (returnlist) =>
            //                {
            //                    await Task.Delay(1200);
            //                      await MainThread.InvokeOnMainThreadAsync(async() =>
            //                    {
            //                         IsBusy = true;
            //                        try
            //                        {
            //                         if (_tempPopupMaterial.Count == 0)
            //                            Sales1.IsVisible = false;

            //                            var modelList = materialList.ItemsSource as ObservableCollection<SaleDetail> ?? new ObservableCollection<SaleDetail>();
            //                            var newModel = new ObservableCollection<SaleDetail>();
            //                            if (_tempPopupMaterial.Count > 0)
            //                            {
            //                                foreach (var item in returnlist)
            //                                {
            //                                    newModel.Add(item);
            //                                           await Task.Delay(500);
            //                                }

            //                                foreach (var item in newModel)
            //                                {
            //                                    modelList.Add(item);
            //                                     await Task.Delay(500);
            //                                }
		          //                            _tempPopupMaterial.AddRange(returnlist);
            //                                _tempPopupMaterial.ForEach(x => x.SaleId = sale.Id);
	                                       
            //                            }
            //                            else
            //                            {
            //                                foreach (var item in returnlist)
            //                                {
            //                                    modelList.Add(item);
            //                                     await Task.Delay(500);
            //                                }
            //                                materialList.ItemsSource = modelList;
            //                                _tempPopupMaterial.AddRange(returnlist);
            //                                _tempPopupMaterial.ForEach(x => x.SaleId = sale.Id);
            //                            }
                                               
		          //                              Sales1.IsVisible = true;
            //                                      IsBusy = false;
	                                  
            //                        }
            //                        catch (Exception ex)
            //                        {
            //                          Sales1.IsVisible = true;
            //                                 IsBusy = false;
            //                         await DisplayAlert("Hata", $"Dosya yüklenirken hata oluştu: {ex.Message}", "Tamam");
            //                        }
            //                    });

            //                };






            await this.ShowPopupAsync(popup);
        }
        catch (Exception)
        {

        }
    }

#endregion



    #region Product ve Sale oluşturma

    private async void Button_Clicked(object sender, EventArgs e)
    {

        try
        {

        
            var year = await SecureStorage.GetAsync("Year");

            this.Dispatcher.Dispatch(async () =>
            {
                güncelle.IsEnabled = false;
                Dispatcher.StartTimer(TimeSpan.FromSeconds(5), () =>
                {
                    güncelle.IsEnabled = true;
                    return false;
                });
                product.Brand = marka.Text;
                product.ModelNumber = modelNu.Text;
                product.ModelDescription = modelAciklama.Text;
                product.Description = aciklama.Text;
                product.YasSize = yassize.Text;
                product.InnerFabricName = ickumasadi.Text;
                product.InnerFabricContent = ickumasicerigi.Text;
                product.InnerFabricSupply = ickumastedarigi.Text;
                product.OuterFabricName = diskumasadi.Text;
                product.OuterFabricContent = diskumasicerigi.Text;
                product.OuterFabricSupply = diskumastedarigi.Text;

                //string appFolder = Path.Combine(FileSystem.Current.AppDataDirectory, "Images");

                //if (!Directory.Exists(appFolder))
                //    Directory.CreateDirectory(appFolder);

                //string newFileName = $"Image_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                //string newFilePath = Path.Combine(appFolder, newFileName);

                //// Resmi hedef klasöre kopyala
                //if (!String.IsNullOrEmpty(originalFilePath))
                //{
                //    File.Copy(originalFilePath, newFilePath, true);
                //    product.ImageURL = newFilePath;

                //}
#if WINDOWS
                                    string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                                    string costFolder = Path.Combine(documentsFolder, "MaliyetApp");

                                    if (!Directory.Exists(costFolder))
                                        Directory.CreateDirectory(costFolder);

                                    string newFileName = $"Image_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                                    string newFilePath = Path.Combine(costFolder, newFileName);

                                    // Resmi hedef klasöre kopyala
                                    if (!String.IsNullOrEmpty(originalFilePath))
                                    {
                                        File.Copy(originalFilePath, newFilePath, true);
                                        product.ImageURL = newFilePath;
                                    }
#elif IOS
                if (!string.IsNullOrEmpty(copiedFilePath))
                {
                  try
                    {
                        using (var client = new HttpClient())
                        {
                            using (var content = new MultipartFormDataContent())
                            {
                                // Dosyayı aç ve StreamContent olarak ekle
                                var fileStream = File.OpenRead(copiedFilePath);
                                var fileContent = new StreamContent(fileStream);
                                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");

                                // MultipartFormDataContent'e dosyayı ekle
                                content.Add(fileContent, "formFile", Path.GetFileName(copiedFilePath));

                                // API'ye POST isteği gönder
                                var response = await client.PostAsync(baseApiUrl, content);

                                if (response.IsSuccessStatusCode)
                                {
                                    var jsonResponse = await response.Content.ReadAsStringAsync();
                                    var filePathFromResponse = JObject.Parse(jsonResponse)["filePath"]?.ToString();
                                       product.ImageURL = filePathFromResponse;
                                }
                                else
                                {
                                    await DisplayAlert("Hata", "Dosya yükleme başarısız.", "Tamam");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Hata", $"Dosya yüklenirken hata oluştu: {ex.Message}", "Tamam");
                    }
                }
                else
                {
                    Console.WriteLine("Dosya yolu geçersiz veya boş.");
                }


#else
                if (!String.IsNullOrEmpty(originalFilePath))
                {


                    var handler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                    };
                
                    using (var client = new HttpClient(handler))
                    {
                        using (var content = new MultipartFormDataContent())
                        {
                            var fileContent = new ByteArrayContent(File.ReadAllBytes(originalFilePath));
                            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");

                            // Doğru form alanı adı: "formFile"
                            content.Add(fileContent, "formFile", Path.GetFileName(originalFilePath));
                            var response = await client.PostAsync(baseApiUrl, content);
                            try
                            {

                                if (response.IsSuccessStatusCode)
                                {
                                    var jsonResponse = await response.Content.ReadAsStringAsync();
                                    var filePath = JObject.Parse(jsonResponse)["filePath"]?.ToString();
                                    product.ImageURL = filePath;
                                }
                                else
                                {
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }

                }
             
#endif





                var returnProductId = await DatabaseService.CreateProduct(product,year);

                if (returnProductId==-1)
                {
                    ExceptionAlert.ShowAlert("Uyarı","Ürün oluştururken hata");
                    return;
                }
                sale.ProductId = returnProductId;
                await Task.Delay(1500);
                var returnSaleId = await DatabaseService.CreateSale(sale, year);

                if (returnSaleId == -1)
                {
                    ExceptionAlert.ShowAlert("Uyarı", "Ürün oluştururken hata");
                    return;
                }
                sale.Id = returnSaleId;
                product1.IsVisible = false;
                await Task.Delay(1000);
                Sales1.IsVisible = true;
       
                Task.Run(async () =>
                {
                    await SecureStorage.SetAsync("MainRefresh", "true");
                });
            });

        }
        catch (Exception ex)
        {

        }

    }

#endregion

    #region Maliyet Hesaplama

    float sonuc = 0;
    private void Button_Clicked_1(object sender, EventArgs e)
    {
        try
        {

            var models = materialList.ItemsSource as ObservableCollection<SaleDetail>;

            this.Dispatcher.Dispatch(() =>
            {
                float toplam = 0;
                foreach (var item in models.Where(a => a.Unit != 0 && a.UnitePrice != 0))
                {
                    toplam += item.Price;
                }
                sonuc = toplam;
                totalLabel.Text = "Maliyet: " + " " + toplam.ToString("F2") + " ₺";
                if (!String.IsNullOrEmpty(totalLabel.Text))
                {
                    geneltoplam = sonuc;
                }
                if (!String.IsNullOrEmpty(totalLabel.Text) && !String.IsNullOrEmpty(karEntry.Text))
                {
                    geneltoplam = sonuc;
                    // karOrani = (float.Parse(e.NewTextValue) / 100)*100;
                    karOrani = float.Parse(karEntry.Text);
                    var textKarOrani = (float.Parse(karEntry.Text) / 100);
                    satisFiyati2 = geneltoplam * (1 + textKarOrani);
                    eldeDilenKar = satisFiyati2 - sonuc;

                    satisFiyati.Text = $"Önerilen Satış fiyatı: {satisFiyati2:F2} TL\n" +
                                       $"Kar Oranı Hesabı: %{textKarOrani * 100:F2}\n" +
                                       $"Elde Edilen Kar: {satisFiyati2 - sonuc:F2} TL";
                }

            });
        }
        catch (Exception)
        {

        }
    }


    private void OnEntryFocused(object sender, FocusEventArgs e)
    {
        if (sender is BorderlessEntry entry)
        {
            entry.BackgroundColor = Color.FromArgb("#F5F5F5"); // Süt beyazı rengi
        }
    }

    private void OnEntryUnfocused(object sender, FocusEventArgs e)
    {
        if (sender is BorderlessEntry entry)
        {
            entry.BackgroundColor = Colors.Transparent; // Varsayılan arka plan
        }
    }

    private void Entry_TextChanged_2(object sender, TextChangedEventArgs e)
    {
        try
        {
            if (!String.IsNullOrEmpty(e.NewTextValue))
            {

                if (!String.IsNullOrEmpty(totalLabel.Text))
                {

                    geneltoplam = sonuc;
                    // karOrani = (float.Parse(e.NewTextValue) / 100)*100;
                    karOrani = float.Parse(e.NewTextValue);
                    var textKarOrani = (float.Parse(e.NewTextValue) / 100);
                    satisFiyati2 = geneltoplam * (1 + textKarOrani);
                     eldeDilenKar = satisFiyati2 - sonuc;

                    satisFiyati.Text = $"Önerilen Satış fiyatı: {satisFiyati2:F2} TL\n" +
                                       $"Kar Oranı Hesabı: %{textKarOrani * 100:F2}\n" +
                                       $"Elde Edilen Kar: {satisFiyati2 - sonuc:F2} TL";
                }


            }
        }
        catch (Exception)
        {

        }
    }

    #endregion



    #region Update Product

    float geneltoplam;
    float karOrani;
    float satisFiyati2;
    float eldeDilenKar;


    private async  void Button_Clicked_2(object sender, EventArgs e)
    {
        try
        {
#if WINDOWS

#else

            DisplayAlert("Uyarı", "malzemeler ekleniyor, lütfen bekleyin", "Tamam");
#endif
            islemiTamamla.IsEnabled = false;
            var model = await DatabaseService.GetSaleById(sale.Id);

            if (model != null)
            {
                MainThread.BeginInvokeOnMainThread(() => IsBusy = true);

                model.KarOrani = karOrani;
                model.GenelToplam = geneltoplam;
                model.SatisFiyati = satisFiyati2;
                model.EldeEdilenKar = eldeDilenKar;

               await DatabaseService.UpdateSale(model);

                foreach (var item in _tempPopupMaterial)
                {

                    var checkResult = await DatabaseService.CheckSaleMaterial(item.SaleId, item.MaterialId);
                    if (!checkResult)
                    {
                        var saleModel = new SaleDetail()
                        {
                            MarketRate = item.MarketRate,
                            MarketRateType = item.MarketRateType,
                            MaterialId = item.MaterialId
                        ,
                            Price = item.Price,
                            SaleId = item.SaleId,
                            Unit = item.Unit,
                            UnitePrice = item.UnitePrice
                        };
                        await DatabaseService.CreateSaleDetails(saleModel);
                    }
                    
                }

                MainThread.BeginInvokeOnMainThread(() => IsBusy = false);
                islemiTamamla.IsEnabled = true;
                await SecureStorage.SetAsync("MainRefresh", "true");
                await DisplayAlert("İşlem başarılı", "Maliyet oluşturma işlemi tamamlandı", "Tamam");
                await Navigation.PopAsync();
            }
        }
        catch (Exception)
        {
            islemiTamamla.IsEnabled = true;
        }
    }
#endregion
 
    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}