
using CommunityToolkit.Maui.Views;
using MaliyetApp.Libs.AppSettings;
using MaliyetApp.Libs.Handlers;
using MaliyetApp.Libs.Models;
using MaliyetApp.Libs.Models.SQlitecontextDb;
using MaliyetApp.ViewModels;
using Microsoft.Maui.Dispatching;
using Newtonsoft.Json.Linq;
using RBush;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Windows.Input;


namespace MaliyetApp.Views.ProductPage;

public partial class UpdateProduct : ContentPage
{
    private int _saleId = 0;
    public UpdateProduct(int saleId)
    {
        InitializeComponent();
        _saleId = saleId;
    }
    Sale sale = new Sale();
    private List<SaleDetail> _tempPopupMaterial = new List<SaleDetail>();// popup filtresi için
    string baseApiUrl = AppConst.ImageAddress;
    string imagePath = AppConst.ImagePath;

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {

            var dolar = await SecureStorage.GetAsync("Dolar");
            var euro = await SecureStorage.GetAsync("Euro");

            float dolarKuru = string.IsNullOrEmpty(dolar) ? 0f : float.Parse(dolar);
            float euroKuru = string.IsNullOrEmpty(euro) ? 0f : float.Parse(euro);

            if (dolarKuru > 0)
            {
                Dolar_.Text = dolarKuru.ToString();
            }
            if (euroKuru > 0)
            {
                Euro_.Text = euroKuru.ToString();
            }
            this.Dispatcher.DispatchAsync(async() =>
            {
                sale =await DatabaseService.GetSaleWithSaleDetails(_saleId);

                if (sale.SaleDetail != null && sale.SaleDetail.Any())
                {
                    _tempPopupMaterial.AddRange(sale.SaleDetail.ToList());
                    materialList.ItemsSource = new ObservableCollection<SaleDetail>(_tempPopupMaterial); ;
                }
                else
                {
                    // Eğer boşsa, uygun bir işlem yapabilirsiniz
                    materialList.ItemsSource = new ObservableCollection<SaleDetail>();
                }
                try
                {
                        #if WINDOWS
                                    
                        #else
                                            sale.Product.ImageURL = $"{imagePath}/{sale.Product.ImageURL}";
                        #endif
                }
                catch 
                {

                }

                geneltoplam = sale.GenelToplam;
                sonuc = geneltoplam;
                karOrani = sale.KarOrani;
                satisFiyati2 = sale.SatisFiyati;
                eldeDilenKar = sale.EldeEdilenKar;
                var textKarOrani = (karOrani / 100);
                satisFiyati.Text = $"Önerilen Satış fiyatı: {satisFiyati2:F2} TL\n" +
                                   $"Kar Oranı Hesabı: %{textKarOrani * 100:F2}\n" +
                                   $"Elde Edilen Kar: {satisFiyati2 - sonuc:F2} TL";


                BindingContext = sale;
                marka.Unfocus();
                modelNu.Unfocus();
                aciklama.Unfocus();
                yassize.Unfocus();
                ickumasadi.Unfocus();
                ickumasicerigi.Unfocus();
                ickumastedarigi.Unfocus();
                diskumasadi.Unfocus();
                diskumasicerigi.Unfocus();
                diskumastedarigi.Unfocus();
                karEntry.Unfocus();

                toggleSwitch.Toggled += (s, e) =>
                {
                    bool isToggled = e.Value;
                    if (isToggled)
                    {
                        product1.IsVisible = true;
                        sales1.IsVisible = false;
                    }
                    else
                    {
                        sales1.IsVisible = true;
                        product1.IsVisible = false;
                    }
                };
            });

        }
        catch (Exception)
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
    #region Malzeme Popup


    private async void OnSubmitPopup(object sender, EventArgs e)
    {
        try
        {
            var popup = new MaterialPopup(sale.Id, _tempPopupMaterial);
            popup.ListAction += (returnlist) =>
            {

                this.Dispatcher.Dispatch(() =>
                {
                    ObservableCollection<SaleDetail> bindingModel = materialList.ItemsSource as ObservableCollection<SaleDetail>;

                    if (bindingModel == null)
                    {
                        bindingModel = new ObservableCollection<SaleDetail>();
                        materialList.ItemsSource = bindingModel;
                    }

                    if (_tempPopupMaterial.Count > 0)
                    {
                        foreach (var item in returnlist)
                        {
                            bindingModel.Add(item);
                        }

                        _tempPopupMaterial.AddRange(returnlist);
                        _tempPopupMaterial.ForEach(x => x.SaleId = sale.Id);
                        materialList.ItemsSource = null;
                        materialList.ItemsSource = bindingModel;
                    }
                    else
                    {
                        foreach (var item in returnlist)
                        {
                            bindingModel.Add(item);
                        }

                        _tempPopupMaterial.AddRange(bindingModel);
                        _tempPopupMaterial.ForEach(x => x.SaleId = sale.Id);
                        materialList.ItemsSource = null;

                        materialList.ItemsSource = bindingModel;
                    }
                });


            };
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
            ////////değişekkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkk
            var productModel = await DatabaseService.GetProductById(sale.ProductId);
            if (productModel==null)
            {
                DisplayAlert("uyarı", "Ürün bulunamadı", "Tamam");
                return;
            }

            güncelle.IsEnabled = false;
            Dispatcher.StartTimer(TimeSpan.FromSeconds(5), () =>
            {
                güncelle.IsEnabled = true;
                return false;
            });

            productModel.Id = sale.ProductId;
            productModel.Brand = marka.Text;
            productModel.ModelNumber = modelNu.Text;
            productModel.ModelDescription = modelAciklama.Text;
            productModel.Description = aciklama.Text;
            productModel.YasSize = yassize.Text;
            productModel.InnerFabricName = ickumasadi.Text;
            productModel.InnerFabricContent = ickumasicerigi.Text;
            productModel.InnerFabricSupply = ickumastedarigi.Text;
            productModel.OuterFabricName = diskumasadi.Text;
            productModel.OuterFabricContent = diskumasicerigi.Text;
            productModel.OuterFabricSupply = diskumastedarigi.Text;
            productModel.LastModificationTime = DateTime.Now;
            productModel.SqliteId = sale.SqliteId;

            //string appFolder = Path.Combine(FileSystem.Current.AppDataDirectory, "Images");

            //if (!Directory.Exists(appFolder))
            //    Directory.CreateDirectory(appFolder);

            //string newFileName = $"Image_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            //string newFilePath = Path.Combine(appFolder, newFileName);

            //// Resmi hedef klasöre kopyala
            //if (!String.IsNullOrEmpty(originalFilePath))
            //{
            //    File.Copy(originalFilePath, newFilePath, true);
            //    productModel.ImageURL = newFilePath;

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
                    productModel.ImageURL = newFilePath;
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
                                       productModel.ImageURL = filePathFromResponse;
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
                                productModel.ImageURL = filePath;
                            }
                            else
                            {
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionAlert.ShowAlert("resim yüklenemedi", ex.Message);
                        }
                    }
                }
            }


#endif

            productModel.IsUpdate = false;

            await DatabaseService.UpdateProduct(productModel);
       
            toggleSwitch.IsToggled = true;
            sales1.IsVisible = false;
            product1.IsVisible = true;

           await SecureStorage.SetAsync("MainRefresh", "true");
        

        }
        catch (Exception ex)
        {
            IsBusy = false;
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
                totalLabel.Text = toplam.ToString("F2");
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



    private void Entry_TextChanged_2(object sender, TextChangedEventArgs e)
    {
        try
        {
            if (!String.IsNullOrEmpty(e.NewTextValue))
            {

                if (!String.IsNullOrEmpty(totalLabel.Text))
                {
                    //yüklenirken buraları değeri verrr ---oranı 33 gibi kaydet 0.33 gibi kaydloyr


                    geneltoplam = sonuc;
                   
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


    #region Update Product

    float geneltoplam; 
    float karOrani;
    float satisFiyati2;
    float eldeDilenKar;

 
    private async void Button_Clicked_2(object sender, EventArgs e)
    {
        try
        {

            var model =await DatabaseService.GetSaleById(sale.Id);
            if (model != null)
            {
#if WINDOWS

#else

                DisplayAlert("Uyarı", "malzemeler ekleniyor, lütfen bekleyin", "Tamam");
#endif
                islemiTamamla.IsEnabled = false;
                Dispatcher.DispatchAsync(() => IsBusy = true);
                model.KarOrani = karOrani;
                model.GenelToplam = geneltoplam;
                model.SatisFiyati = satisFiyati2;
                model.EldeEdilenKar = eldeDilenKar;
               await DatabaseService.UpdateSale(model);

                var saleAndDetail = await DatabaseService.GetSaleWithSaleDetails(sale.Id);

                var saleDetailList = materialList.ItemsSource as ObservableCollection<SaleDetail>;

                if (saleDetailList == null)
                {
                    saleDetailList = new ObservableCollection<SaleDetail>();
                    materialList.ItemsSource = saleDetailList;
                }

                foreach (var item in saleDetailList)
                {
                    var detay = saleAndDetail.SaleDetail.FirstOrDefault(a => a.Id == item.Id);

                    if (detay != null)
                    {
                        detay.MarketRate = item.MarketRate;
                        detay.MarketRateType = item.MarketRateType;
                        detay.Price = item.Price;
                        detay.Unit = item.Unit;
                        detay.UnitePrice = item.UnitePrice;
                        await DatabaseService.UpdateSaleDetail(detay);
                    }
                    else
                    {


                       var checkResult= await  DatabaseService.CheckSaleMaterial(item.SaleId, item.MaterialId);

                        if (!checkResult)
                        {
                            var saleModel = new SaleDetail()
                            {
                                MarketRate = item.MarketRate,
                                MarketRateType = item.MarketRateType,
                                MaterialId = item.MaterialId,
                                Price = item.Price,
                                SaleId = item.SaleId,
                                Unit = item.Unit,
                                UnitePrice = item.UnitePrice
                            };
                           await DatabaseService.CreateSaleDetails(saleModel);
                        }
                    }
                }
                Dispatcher.DispatchAsync(() => IsBusy = false);
                islemiTamamla.IsEnabled = true;
                await SecureStorage.SetAsync("MainRefresh", "true");
          
                await DisplayAlert("İşlem başarılı", "Maliyet oluşturma işlemi tamamlandı", "Tamam");

              await  Navigation.PopAsync();
            }
        }
        catch (Exception)
        {

        }
    }
    #endregion

    private void karEntry_Focused(object sender, FocusEventArgs e)
    {

    }

    private void marka_Unfocused(object sender, FocusEventArgs e)
    {

    }

    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();

    }
}