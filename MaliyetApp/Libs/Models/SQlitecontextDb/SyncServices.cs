using MaliyetApp.Libs.AppSettings;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Newtonsoft.Json.Linq;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MaliyetApp.Libs.Models.SQlitecontextDb
{
    public class SyncServices
    {
        public static async Task StartSyncSevices()
        {
            try
            {
                await MaterialLocalDataToServer();
                await ProductLocalDataToServer();
                await SaleLocalDataToServer();
                await SaleDetailLocalDataToServer();
                await OrderLocalDataToServer();
                await OrderDetailLocalDataToServer();
            }
            catch (Exception ex)
            {

            }
        }
        public static async Task MaterialLocalDataToServer()
        {
            List<Material> returnMaterial = new List<Material>();

            var materials = await DatabaseService.GetAllMaterials(); // SQLite verilerini al
            var jsonData = JsonSerializer.Serialize(materials);
            var handler2 = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            var httpClient = new HttpClient(handler2);
           
             
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            //   var apiUrl = "https://localhost:7239/Entegrasyon/sync-materials";
                var apiUrl = $"{AppConst.HostAddress}Entegrasyon/sync-materials";

                var response = await httpClient.PostAsync(apiUrl, content);


                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<List<Material>>();


                    if (result.Count>0)
                    {
                        foreach (var item in result)
                        {
                            var material = await DatabaseService.GetMaterialById(item.Id);

                            if (material==null)
                            {
                                item.LastModificationTime = DateTime.Now;
                                 var itemId= await DatabaseService.CreateMetarial(item);
                                item.Id = itemId;
                                returnMaterial.Add(item);
                            }
                            else
                            {
                                material.LastModificationTime = DateTime.Now;
                                material.Name = item.Name;
                                material.Type = item.Type;
                                material.SqliteId = item.SqliteId;

                                await DatabaseService.UpdateMaterial(material);
                            }
                        }



                    }
                    Console.WriteLine("Veriler başarıyla sunucuya gönderildi.");
                }
                else
                {
                    Console.WriteLine($"Hata: {response.StatusCode}");
                }


            if (returnMaterial.Count > 0)
            {

                try
                {
                         jsonData = JsonSerializer.Serialize(returnMaterial);
                         content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                         apiUrl = $"{AppConst.HostAddress}Entegrasyon/sync-materialsupdate";
                         response = await httpClient.PostAsync(apiUrl, content);
                }
                catch (Exception ex)
                {

                }
            }
        }
       
        
        
        public static async Task UploadImage(Product product)
        {
            try
            {
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };
                using (HttpClient client = new HttpClient(handler))
                {
                    using (var multipart = new MultipartFormDataContent())
                    {
                        var fileContent = new ByteArrayContent(File.ReadAllBytes(product.ImageURL));
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
                        multipart.Add(fileContent, "formFile", Path.GetFileName(product.ImageURL));
                        var response = await client.PostAsync("", multipart);
                        if (response.IsSuccessStatusCode)
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            
           
        }

        public static async Task ProductLocalDataToServer()
        {
            
           // ExceptionAlert.ShowAlert("Sekronizasyon işlemi başladı","İşlme bitti uyarısı gelene kadar lütfen bekleyin, aksi halde veriler eksik bir şekilde yüklenir");
            
            try
            {
                var year = await SecureStorage.GetAsync("Year");

                var getAllProduct = await DatabaseService.GetAllProduct(year);
                var updateProductList = getAllProduct.Where(a => a.IsUpdate != true ).ToList();
                List<Product> returnProduct = new List<Product>();
                if (updateProductList.Count>0)
                {
                    foreach (var item in updateProductList)
                    {
                        var handler2 = new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                        };
                        using (HttpClient client2 = new HttpClient(handler2))
                        {
                            client2.Timeout = TimeSpan.FromMinutes(20);
                            using (var multipart = new MultipartFormDataContent())
                            {
                                try
                                {

                                    if (!String.IsNullOrEmpty(item.ImageURL))
                                    {
                                        var replacePath = item.ImageURL.Replace("\\", "/");
                                        var optimizedImage = OptimizeImage(Path.GetFullPath(replacePath), 500, 800, 800); // Maks. 500 KB, 800x800 çözünürlük

                                        var content1 = new ByteArrayContent(optimizedImage);
                                        content1.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");

                                        multipart.Add(content1, "formFile", Path.GetFileName(replacePath));
                                        var response1 = await client2.PostAsync($"{AppConst.ImageAddress}", multipart);

                                        if (response1.IsSuccessStatusCode)
                                        {
                                            var jsonResponse = await response1.Content.ReadAsStringAsync();
                                            var filePath = JObject.Parse(jsonResponse)["filePath"]?.ToString();
                                            item.ImageURL = filePath;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    DatabaseService.AddLogData(ex.Message, "Sunucuya Resim yüklerken");
                                }

                            }

                        }


                    } 


                }

                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };

                DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                DateTime newDate;
                var jsonData = JsonSerializer.Serialize(updateProductList.Select(p => new
                {
                    p.Id,
                    p.Brand,
                    p.ModelNumber,
                    p.ModelDescription,
                    p.Description,
                    p.Size,
                    p.YasSize,
                    p.InnerFabricName,
                    p.InnerFabricContent,
                    p.InnerFabricSupply,
                    p.OuterFabricName,
                    p.OuterFabricContent,
                    p.OuterFabricSupply,
                    Creationtime = DateTime.TryParse(p.Creationtime, out newDate)
                                    ? newDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
                                    : null,
                    p.ImageURL,
                    p.IsDeleted,
                    p.LastModificationTime
                }), new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                });


                var httpClient = new HttpClient(handler);
                httpClient.Timeout = TimeSpan.FromMinutes(20);
                if (jsonData == null || jsonData.Count() == 0)
                {
                    jsonData = JsonSerializer.Serialize(new List<Product>());
                }
                var apiUrl = $"{AppConst.HostAddress}Entegrasyon/sync-productcrud";

                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {

                        var result = await response.Content.ReadFromJsonAsync<List<Product>>();
                        string costFolder = "";
                        if (result.Count>0)
                        {
                            string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                             //costFolder = Path.Combine(documentsFolder, "MaliyetApp_Entegrasyon_" + DateTime.Now.ToString("dd.mm.yyyy")); MaliyetApp
                             costFolder = Path.Combine(documentsFolder, "MaliyetApp"); 

                            if (!Directory.Exists(costFolder))
                                Directory.CreateDirectory(costFolder);
                        }
                        foreach (var item in result)
                        {
                            var product= await DatabaseService.GetProductById(item.Id);
                            if (product==null)
                            {
                                try
                                {
                                    if (!String.IsNullOrEmpty(item.ImageURL))
                                    {

                                        string newFileName = $"Image_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                                        string newFilePath = Path.Combine(costFolder, newFileName);
                                        string tempFilePath = Path.Combine(costFolder, $"temp_{newFileName}");

                                        // Geçici olarak indir
                                        using (WebClient client = new WebClient())
                                        {
                                            var uri = new Uri($"{AppConst.ImageAddress}/{item.ImageURL}");
                                            await client.DownloadFileTaskAsync(uri, tempFilePath); // Asenkron indirme
                                            await Task.Delay(1100);
                                        }
                                        var optimizedImage = OptimizeImage(tempFilePath, 500, 800, 800);
                                        File.WriteAllBytes(newFilePath, optimizedImage);
                                        await Task.Delay(500);
                                        if (File.Exists(tempFilePath))
                                            File.Delete(tempFilePath);


                                        item.ImageURL = newFilePath;
                                    }

                                }
                                catch (Exception ex)
                                {
                                    DatabaseService.AddLogData(ex.Message, "Sunucudan Resim indirirken");
                                }
                                item.LastModificationTime = DateTime.Now;
                              
                                var itemId= await DatabaseService.CreateProduct(item,year,true);
                                item.Id = itemId;
                                returnProduct.Add(item);

                            }
                            else
                            {
                                try
                                {
                                    if (!String.IsNullOrEmpty(item.ImageURL))
                                    {


                                        string newFileName = $"Image_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                                        string newFilePath = Path.Combine(costFolder, newFileName);
                                        string tempFilePath = Path.Combine(costFolder, $"temp_{newFileName}");

                                        using (WebClient client = new WebClient())
                                        {
                                            var uri = new Uri($"{AppConst.ImageAddress}/{item.ImageURL}");
                                            await client.DownloadFileTaskAsync(uri, tempFilePath); // Asenkron indirme
                                            await Task.Delay(1100);
                                        }

                                        var optimizedImage = OptimizeImage(tempFilePath, 500, 800, 800); // Maks. 500 KB, 800x800 çözünürlük
                                        File.WriteAllBytes(newFilePath, optimizedImage); // Optimize edilmiş resmi kaydet
                                        await Task.Delay(1100);
                                        if (File.Exists(tempFilePath))
                                            File.Delete(tempFilePath);
                                        await Task.Delay(1100);

                                        if (File.Exists(product.ImageURL))
                                            File.Delete(product.ImageURL);
                                        await Task.Delay(1300);

                                        product.ImageURL = newFilePath;
                                    
                                    }
                                }
                                catch (Exception ex)
                                {

                                    DatabaseService.AddLogData(ex.Message, "Sunucudan Resim indirirken");

                                }

                                product.IsUpdate = true;
                                product.IsDeleted = item.IsDeleted;
                                product.LastModificationTime = DateTime.Now;
                                product.Brand = item.Brand;
                                product.Description = item.Description;
                                product.InnerFabricContent = item.InnerFabricContent;
                                product.InnerFabricName = item.InnerFabricName;
                                product.InnerFabricSupply = item.InnerFabricSupply;
                                product.ModelDescription = item.ModelDescription;
                                product.ModelNumber = item.ModelNumber;
                                product.OuterFabricContent = item.OuterFabricContent;
                                product.OuterFabricName = item.OuterFabricName;
                                product.OuterFabricSupply = item.OuterFabricSupply;
                                product.Size = item.Size;
                                product.YasSize = item.YasSize;
                                product.SqliteId = item.SqliteId;
                                

                                await DatabaseService.UpdateProduct(product);



                            }
                           
                        }
                        Console.WriteLine("Veriler başarıyla gönderildi.");
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Hata Mesajı: {errorMessage}");
                        DatabaseService.AddLogData($"Hata Mesajı: {errorMessage}", "Sunucudan Resim indirirken");
                    }


                if (returnProduct.Count > 0)
                {

                    try
                    {

                        DateTime newDate2;
                        jsonData = JsonSerializer.Serialize(returnProduct.Select(p => new
                        {
                            p.Id,
                            p.Brand,
                            p.ModelNumber,
                            p.ModelDescription,
                            p.Description,
                            p.Size,
                            p.YasSize,
                            p.InnerFabricName,
                            p.InnerFabricContent,
                            p.InnerFabricSupply,
                            p.OuterFabricName,
                            p.OuterFabricContent,
                            p.OuterFabricSupply,
                            Creationtime = DateTime.TryParse(p.Creationtime, out newDate2)
                                           ? newDate2.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
                                           : null,
                            p.ImageURL,
                            p.IsDeleted,
                            p.LastModificationTime,
                            p.SqliteId,
                          
                        }), new JsonSerializerOptions
                        {
                            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                        });

                             content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                             apiUrl = $"{AppConst.HostAddress}Entegrasyon/sync-productupdate";
                             response = await httpClient.PostAsync(apiUrl, content);
                    }
                    catch (Exception ex)
                    {
                        DatabaseService.AddLogData($"Hata Detayı: {ex.Message}", "sale update");
                        ExceptionAlert.ShowAlert("hata",ex.Message);
                    }
                }



            }
            catch (Exception ex)
            {
                DatabaseService.AddLogData(ex.Message, "Sunucudan Resim indirirken");
                ExceptionAlert.ShowAlert("hata", ex.Message);
            }
        }

        

        public static byte[] OptimizeImage(string filePath, int maxFileSizeKb, int maxWidth, int maxHeight)
        {
            try
            {
                using (var inputStream = File.OpenRead(filePath))
                using (var originalBitmap = SKBitmap.Decode(inputStream))
                {
                    int newWidth = originalBitmap.Width;
                    int newHeight = originalBitmap.Height;

                    // Boyutlandırma işlemi
                    if (originalBitmap.Width > maxWidth || originalBitmap.Height > maxHeight)
                    {
                        double widthRatio = (double)maxWidth / originalBitmap.Width;
                        double heightRatio = (double)maxHeight / originalBitmap.Height;
                        double ratio = Math.Min(widthRatio, heightRatio);

                        newWidth = (int)(originalBitmap.Width * ratio);
                        newHeight = (int)(originalBitmap.Height * ratio);
                    }

                    using (var resizedBitmap = originalBitmap.Resize(new SKImageInfo(newWidth, newHeight), SKFilterQuality.High))
                    {
                        int quality = 90; // Başlangıç sıkıştırma oranı
                        byte[] result;

                        do
                        {
                            using (var image = SKImage.FromBitmap(resizedBitmap))
                            using (var memoryStream = new MemoryStream())
                            {
                                image.Encode(SKEncodedImageFormat.Jpeg, quality).SaveTo(memoryStream);
                                result = memoryStream.ToArray();
                            }

                            // Eğer dosya boyutu hala büyükse kaliteyi düşür
                            if (result.Length > maxFileSizeKb * 1024)
                            {
                                quality -= 5; // Kaliteyi 5'er düşürerek tekrar dene
                            }
                            else
                            {
                                break; // Boyut uygun, işlemi sonlandır
                            }
                        } while (quality > 10); // Kalite 10'dan aşağı düşmesin

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                DatabaseService.AddLogData(ex.Message, "Resim Boyutlanırken Hata");
                byte[] result = new byte[0];
                return result;
            }
    }

        public static async Task SaleLocalDataToServer()
        {
            try
            {
                var year = await SecureStorage.GetAsync("Year");
                var getAllSale = await DatabaseService.GetAllSaleIncludeDelete(year,asyncProgress:true);
                DateTime newDate;
                List<Sale> returnSale = new List<Sale>();
                var jsonData = JsonSerializer.Serialize(getAllSale.Select(a => new 
                {

                  CreationTime=DateTime.TryParse(a.Creationtime,out newDate ) ? newDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"): null,
                  a.SatisFiyati,
                  a.ProductId,
                  a.LastModificationTime,
                  a.KarOrani,
                  a.IsDeleted,
                  a.Id,
                  a.GenelToplam,
                  a.EldeEdilenKar,

                }), new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                });

                if (jsonData== null || jsonData.Count()==0)
                {
                    jsonData = JsonSerializer.Serialize(new List<Sale>());
                }


                var handler2 = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };
                    var httpClient = new HttpClient(handler2);
                    httpClient.Timeout = TimeSpan.FromMinutes(20);
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    var apiUrl = $"{AppConst.HostAddress}Entegrasyon/sync-salecrud";
                    var response = await httpClient.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<List<Sale>>();
                        if (result.Count>0)
                        {
                            foreach (var item in result)
                            {
                                try
                                {
                                    var sale = await DatabaseService.GetSaleById(item.Id);
                                    if (sale == null|| item.Id==0)
                                    {
                                        item.LastModificationTime = DateTime.Now;

                                        var productModel = await DatabaseService.GetProductBySqliteId(item.ProductId);

                                        if (productModel!=null)
                                        {
                                            item.ProductId = productModel.Id;
                                        }
                                        var itemId=   await DatabaseService.CreateSale(item, year,true);
                                        item.Id = itemId;
                                        returnSale.Add(item);

                                    }
                                    else
                                    {
                                        sale.EldeEdilenKar = item.EldeEdilenKar;
                                        sale.SatisFiyati = item.SatisFiyati;
                                        sale.KarOrani = item.KarOrani;
                                        sale.GenelToplam = item.GenelToplam;
                                        sale.IsDeleted = item.IsDeleted;
                                        sale.LastModificationTime = DateTime.Now;
                                         sale.SqliteId =item.SqliteId;
                                        var productModel = await DatabaseService.GetProductBySqliteId(item.ProductId);

                                        if (productModel != null)
                                        {
                                            sale.ProductId = productModel.Id;
                                        }
                                        await DatabaseService.UpdateSale(sale,true);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    DatabaseService.AddLogData(item, ex.Message + "sale update");
                                ExceptionAlert.ShowAlert("hata", ex.Message);
                            }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Hata: {response.StatusCode}");
                        var errorMessage = await response.Content.ReadAsStringAsync();

                        Console.WriteLine();
                        DatabaseService.AddLogData($"Hata Detayı: {errorMessage}", "sale update");
                    }

                if (returnSale.Count > 0)
                {
                    try
                    {
                         jsonData = JsonSerializer.Serialize(returnSale.Select(a => new
                        {

                            CreationTime = DateTime.TryParse(a.Creationtime, out newDate) ? newDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ") : null,
                            a.SatisFiyati,
                            a.ProductId,
                            a.LastModificationTime,
                            a.KarOrani,
                            a.IsDeleted,
                            a.Id,
                            a.GenelToplam,
                            a.EldeEdilenKar,
                            a.SqliteId,


                        }), new JsonSerializerOptions
                        {
                            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                        });
                       
                             content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                             apiUrl = $"{AppConst.HostAddress}Entegrasyon/sync-saleupdate";
                             response = await httpClient.PostAsync(apiUrl, content);
                    }
                    catch (Exception ex)
                    {
                        DatabaseService.AddLogData($"Hata Detayı: {ex.Message}", "sale update");
                        ExceptionAlert.ShowAlert("hata", ex.Message);
                    }
                }

            }
            catch (Exception ex)
            {
                      DatabaseService.AddLogData($"Hata Detayı: {ex.Message}", "sale update");
                ExceptionAlert.ShowAlert("hata", ex.Message);
            }
        }
        public static async Task SaleDetailLocalDataToServer()
        {
            try
            {
                var year = await SecureStorage.GetAsync("Year");
                var getAllSaleDetail = await DatabaseService.GetAllSaleDetails();
                List<SaleDetail> returnSaleDetail = new List<SaleDetail>();
                var jsonData = JsonSerializer.Serialize(getAllSaleDetail, new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                });
                var handler2 = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };
                var httpClient = new HttpClient(handler2);
                httpClient.Timeout = TimeSpan.FromMinutes(20);

                if (jsonData == null || jsonData.Count() == 0)
                {
                    jsonData = JsonSerializer.Serialize(new List<SaleDetail>());
                }

                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    var apiUrl = $"{AppConst.HostAddress}Entegrasyon/sync-saledetailcrud";


                    var response = await httpClient.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<List<SaleDetail>>();
                        if (result.Count > 0)
                        {
                            foreach (var item in result)
                            {
                                try
                                {
                                    var saleDetail = await DatabaseService.GetSaleDetailById(item.Id);
                                    if (saleDetail == null)
                                    {
                                        var sale = await DatabaseService.GetSaleBySqliteId(item.SaleId);
                                        if (sale != null)
                                        {
                                            item.SaleId = sale.Id;
                                        }

                                        var material = await DatabaseService.GetMaterialBySqliteId(item.MaterialId);
                                        if (material != null)
                                        {
                                            item.MaterialId = material.Id;
                                        }

                                    item.LastModificationTime = DateTime.Now;
                                        var itemId =await DatabaseService.CreateSaleDetails(item);
                                        item.Id = itemId;
                                        returnSaleDetail.Add(item);
                                    }
                                    else
                                    {
                                        saleDetail.IsDeleted = item.IsDeleted;
                                        saleDetail.MarketRate = item.MarketRate;
                                        saleDetail.MarketRateType = item.MarketRateType;
                                        saleDetail.MaterialId = item.MaterialId;
                                        saleDetail.Price = item.Price;
                                        saleDetail.LastModificationTime = DateTime.Now;
                                        saleDetail.Unit = item.Unit;
                                        saleDetail.UnitePrice = item.UnitePrice;
                                        saleDetail.SqliteId = item.SqliteId;
                                        var sale =await  DatabaseService.GetSaleBySqliteId(item.SaleId);
                                        if (sale != null)
                                        {
                                            saleDetail.SaleId = sale.Id;
                                        }

                                        var material = await DatabaseService.GetMaterialBySqliteId(item.MaterialId);
                                        if (material != null)
                                        {
                                            saleDetail.MaterialId = material.Id;
                                        }

                                    await DatabaseService.UpdateSaleDetail(saleDetail);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    DatabaseService.AddLogData(item, ex.Message+ "   SaleDetail update");

                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Hata: {response.StatusCode}");
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Hata Detayı: {errorMessage}");
                        DatabaseService.AddLogData($"Hata Detayı: {errorMessage}" , "   SaleDetail update");
                    }

                if (returnSaleDetail.Count > 0)
                {
                    try
                    {
                             jsonData = JsonSerializer.Serialize(returnSaleDetail);
                             content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                             apiUrl = $"{AppConst.HostAddress}Entegrasyon/sync-saledetailupdate";
                             response = await httpClient.PostAsync(apiUrl, content);
                    }
                    catch (Exception ex)
                    {

                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Bir hata oluştu: {ex.Message}");
            }
        }
        public static async Task OrderLocalDataToServer()
        {
            try
            {
                var handler2 = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };
                var year = await SecureStorage.GetAsync("Year");
                var getAllOrder = await DatabaseService.GetAllOrderIncludeDelete(year,asyncProgress:true);
                List<Order> returnOrder = new List<Order>();
                //Creationtime = DateTime.TryParse(p.Creationtime, out newDate)
                //              ? newDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
                //              : null,
                DateTime newDate;
                var jsonData = JsonSerializer.Serialize(getAllOrder.Select(a=>new
                {

                    a.Description,
                    a.Id,
                    a.LastModificationTime,
                    a.Title,a.IsDeleted,
                    Creationtime = DateTime.TryParse(a.Creationtime, out newDate)
                                 ? newDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
                            : null,


                }), new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                });
               var httpClient = new HttpClient(handler2);
                if (jsonData == null || jsonData.Count() == 0)
                {
                    jsonData = JsonSerializer.Serialize(new List<Order>());
                }

                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    var apiUrl = $"{AppConst.HostAddress}Entegrasyon/sync-ordercrud";

                    var response = await httpClient.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {

                        var result = await response.Content.ReadFromJsonAsync<List<Order>>();
                        if (result.Count > 0)
                        {
                            foreach (var item in result)
                            {
                                try
                                {
                                    var order = await DatabaseService.GetOrderByIdForEntegrasyon1(item.Id);
                                    if (order == null)
                                    {
                                        item.LastModificationTime = DateTime.Now;
                                        var itemId= await DatabaseService.CreateOrder(item,year,true);
                                        item.Id = itemId;
                                        returnOrder.Add(item);
                                    }
                                    else
                                    {
                                        order.IsDeleted = item.IsDeleted;
                                        order.Title = item.Title;
                                        order.LastModificationTime = DateTime.Now;
                                        order.SqliteId = item.SqliteId;
                                        
                                        await DatabaseService.UpdateOrder(order,true);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    DatabaseService.AddLogData(item, ex.Message + "   order update");

                                }
                            }
                        }
                    
                    }
                    else
                    {
                        Console.WriteLine($"Hata: {response.StatusCode}");
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Hata Detayı: {errorMessage}");
                        DatabaseService.AddLogData($"Hata Detayı: {errorMessage}","   order update");
                    }


                if (returnOrder.Count > 0)
                {
                    try
                    {
                        DateTime newDate2;
                         jsonData = JsonSerializer.Serialize(returnOrder.Select(a => new
                        {

                            a.Description,
                            a.Id,
                            a.LastModificationTime,
                            a.Title,a.SqliteId,
                             a.IsDeleted,
                             Creationtime = DateTime.TryParse(a.Creationtime, out newDate2)
                                         ? newDate2.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
                                    : null,


                        }), new JsonSerializerOptions
                        {
                            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                        });
                  
                             content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                             apiUrl = $"{AppConst.HostAddress}Entegrasyon/sync-orderupdate";
                             response = await httpClient.PostAsync(apiUrl, content);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Bir hata oluştu: {ex.Message}");
            }
        }


        //Order,sale,product,orderdetil gibi alanların ıd sorunu var.
        public static async Task OrderDetailLocalDataToServer()
        {
            try
            {
                List<OrderDetails> orderDetails = new List<OrderDetails>();
                var handler2 = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };
                var getAllOrderDetail = await DatabaseService.GetAllOrderDetailIncludeDelete();
                var jsonData = JsonSerializer.Serialize(getAllOrderDetail, new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                });

                if (jsonData == null || jsonData.Count() == 0)
                {
                    jsonData = JsonSerializer.Serialize(new List<OrderDetails>());
                }
                var httpClient = new HttpClient(handler2);
                httpClient.Timeout = TimeSpan.FromMinutes(20);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    var apiUrl = $"{AppConst.HostAddress}Entegrasyon/sync-orderdetailcrud";

                    var response = await httpClient.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<List<OrderDetails>>();
                        if (result.Count > 0)
                        {
                            foreach (var item in result)
                            {
                                try
                                {
                                    var order = await DatabaseService.GetOrderDetailById1(item.Id);
                                    if (order == null)
                                    {
                                        var sale = await DatabaseService.GetSaleBySqliteId(item.SaleId);
                                        if (sale != null)
                                        {
                                            item.SaleId = sale.Id;
                                        }
                                        var orders = await DatabaseService.GetOrderBySqliteId(item.OrderId);
                                        if (orders != null)
                                        {
                                            item.OrderId = orders.Id;
                                        }

                                        item.LastModificationTime = DateTime.Now;
                                        var itemId= await DatabaseService.CreateOrderDetail(item);
                                        item.Id = itemId;
                                        orderDetails.Add(item);
                                    }
                                    else
                                    {
                                        int saleId = 0;
                                        var sales = await DatabaseService.GetSaleById(order.SaleId);

                                        if (sales!=null)
                                           saleId = sales.Id;
                                        else
                                           saleId = item.SaleId;

                                        var orders = await DatabaseService.GetOrderBySqliteId(item.OrderId);
                                        if (orders != null)
                                        {
                                            order.OrderId = orders.Id;
                                        }
                                        var sale = await DatabaseService.GetSaleBySqliteId(item.SaleId);
                                        if (sale != null)
                                        {
                                             saleId = sale.Id;
                                        }

                                        order.IsDeleted = item.IsDeleted;
                                        order.SaleId = saleId;
                                        order.LastModificationTime = DateTime.Now;
                                        order.IsDeleted = item.IsDeleted;
                                        order.SqliteId = item.SqliteId;
                                        await DatabaseService.UpdateOrderDetail(order,true);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    DatabaseService.AddLogData(item, ex.Message + "   orderdetail update");

                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Hata: {response.StatusCode}");
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        Console.WriteLine();
                        DatabaseService.AddLogData($"Hata Detayı: {errorMessage}","   orderdetail update");
                    }

                if (orderDetails.Count > 0)
                {
                    try
                    {
                            jsonData = JsonSerializer.Serialize(orderDetails);
                             content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                             apiUrl = $"{AppConst.HostAddress}Entegrasyon/sync-orderdetailupdate";
                             response = await httpClient.PostAsync(apiUrl, content);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Bir hata oluştu: {ex.Message}");
            }
        }
       
    }
}
