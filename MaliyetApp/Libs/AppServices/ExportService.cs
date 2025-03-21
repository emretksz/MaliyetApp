using ClosedXML.Excel;
using CommunityToolkit.Maui.Storage;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MaliyetApp.Libs.Models;
using MaliyetApp.Libs.Models.SQlitecontextDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Vml;
using ClosedXML.Excel.Drawings;
using MaliyetApp.Libs.AppSettings;
using Microsoft.Maui.Storage;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Drawing.Charts;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Net.Http;
using System.Text.Json;

namespace MaliyetApp.Libs.AppServices
{
    public class ExportService
    {

        public async static Task<bool> ExportToExcel(int orderId)
        {
            var orderList = await DatabaseService.GetOrderById(orderId);

            //string filePath = Path.Combine(_env.ContentRootPath, "wwwroot", "exports", "ÜrünListesi.xlsx");

            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Teklif");

                    int currentRow = 2; // Verilerin basımına 2. satırdan başlanacak

                    // Başlık için 2. satırda E2'den J2'ye kadar birleştirilecek hücre
                    worksheet.Range("E2", "K2").Merge();
                    worksheet.Cell(2, 5).Value = orderList.Title; // E2 hücresine yazı
                    worksheet.Cell(2, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                         .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                    worksheet.Cell(2, 5).Style.Font.SetFontSize(30).Font.SetBold(); // Font büyüklüğü 30 ve kalın yazı
                    worksheet.Range("E2", "K2").Style.Border.SetOutsideBorder(XLBorderStyleValues.Double); // Border ekle

                    // Başlıklar 3. satırda başlayacak
                    currentRow = 3;

                    foreach (var sale in orderList.OrderDetails.Select(a => a.Sale).ToList())
                    {
                        // 3 satır birleştirilmiş resim hücreleri
                        worksheet.Range(currentRow, 5, currentRow + 2, 5).Merge(); // E kolonundan başlıyor
                        worksheet.Cell(currentRow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                                       .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                                       .Font.SetBold() // Kalın yazı
                                                                       .Border.SetOutsideBorder(XLBorderStyleValues.Thin); // Border ekledik

                        // Satır yüksekliklerini artır (1. ve 2. satır için)
                        worksheet.Row(currentRow).Height = 50; // 1. satır yüksekliği artırıldı
                        worksheet.Row(currentRow + 1).Height = 50; // 2. satır yüksekliği artırıldı
                        worksheet.Row(currentRow + 2).Height = 20; // 3. satır yüksekliği normalde kalacak
                        // Resmi ekle
                        string imagePath = "";



#if WINDOWS
                           imagePath = sale.Product.ImageURL; //Path.Combine(product.ImagePath);
#else
                        imagePath = $"{AppSettings.AppConst.ImageAddress}/{sale.Product.ImageURL}";
#endif


                        if (System.IO.File.Exists(imagePath))
                        {
                            using var imageStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
                            var picture = worksheet.AddPicture(imageStream).MoveTo(worksheet.Cell(currentRow, 5));

                            // Resmin genişliğini artırmak için cellWidth'i büyütüyoruz
                            double cellWidth = worksheet.Column(5).Width * 18; // Görsel kolonunun genişlik birimi -> piksel çevrimi
                            picture.Width = (int)cellWidth + ((int)(cellWidth * 0.5)) + 20;
                            picture.Height = (int)(cellWidth / picture.OriginalWidth * picture.OriginalHeight) - 5; // Yükseklik orantılı olarak ayarlandı
                        }

                        // Başlıklar (Renk kırmızı olacak)
                        worksheet.Cell(currentRow, 6).Value = "Model Numarası";
                        worksheet.Cell(currentRow, 6).Style.Font.SetBold().Font.SetFontSize(14);
                        worksheet.Cell(currentRow, 7).Value = "Dış Kumaş Adı";
                        worksheet.Cell(currentRow, 7).Style.Font.SetBold().Font.SetFontSize(14);
                        worksheet.Cell(currentRow, 8).Value = "İç Kumaş Adı";
                        worksheet.Cell(currentRow, 8).Style.Font.SetBold().Font.SetFontSize(14);
                        worksheet.Cell(currentRow, 9).Value = "Yaş";
                        worksheet.Cell(currentRow, 9).Style.Font.SetBold().Font.SetFontSize(14);
                        worksheet.Cell(currentRow, 10).Value = "Fiyat";
                        worksheet.Cell(currentRow, 10).Style.Font.SetBold().Font.SetFontSize(14);
                        worksheet.Cell(currentRow, 11).Value = "Model Açıklama";  // Yeni başlık eklendi
                        worksheet.Cell(currentRow, 11).Style.Font.SetBold().Font.SetFontSize(14);

                        // Başlıkları kırmızı renkle stilize et ve kalın yap
                        worksheet.Range(currentRow, 6, currentRow, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                                                       .Font.SetBold() // Kalın yazı
                                                                                       .Font.SetFontColor(XLColor.Red)
                                                                                       .Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                        // İçerik satırı (Açıklamanın yerini aldı)
                        worksheet.Cell(currentRow + 1, 6).Value = sale.Product.ModelNumber;
                        worksheet.Cell(currentRow + 1, 7).Value = sale.Product.OuterFabricName + "\n" + sale.Product.OuterFabricContent;
                        worksheet.Cell(currentRow + 1, 8).Value = sale.Product.InnerFabricName + "\n" + sale.Product.InnerFabricContent;

                        worksheet.Cell(currentRow + 1, 7).Style.Alignment.WrapText = true; // Satır sonunu destekle
                        worksheet.Cell(currentRow + 1, 8).Style.Alignment.WrapText = true;

                        worksheet.Cell(currentRow + 1, 9).Value = sale.Product.YasSize;
                        worksheet.Cell(currentRow + 1, 10).Value = sale.SatisFiyati.ToString("F2");
                        worksheet.Cell(currentRow + 1, 11).Value = sale.Product.ModelDescription; // Yeni başlık için içerik

                        // İçeriklerin hizalanmasını sağla: Ortada yatay ve dikey ve kalın yap
                        worksheet.Range(currentRow + 1, 6, currentRow + 1, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        worksheet.Range(currentRow + 1, 6, currentRow + 1, 11).Style.Font.SetBold() // Kalın yazı
                                                                                  .Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                        // Açıklama satırı (İçeriğin yerini aldı, arka plan sarı olacak)
                        worksheet.Cell(currentRow + 2, 6).Value = "Açıklama: " + sale.Product.Description;
                        worksheet.Range(currentRow + 2, 6, currentRow + 2, 11).Merge(); // Açıklama alanını birleştir
                        worksheet.Cell(currentRow + 2, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                                                                          .Font.SetItalic() // İtalik yazı
                                                                          .Font.SetBold() // Kalın yazı
                                                                          .Fill.SetBackgroundColor(XLColor.Yellow) // Arka planı sarı yap
                                                                          .Border.SetOutsideBorder(XLBorderStyleValues.Thin); // Border ekledik

                        // Bir sonraki ürün için satır numarasını güncelle
                        currentRow += 3;
                    }

                    // Başlık kolonlarının genişliğini 80 yap ve genişlik daha fazla olursa otomatik olarak ayarla
                    for (int col = 6; col <= 11; col++)  // Yeni başlık için 11. kolon da dahil
                    {
                        worksheet.Column(col).Width = 30; // Başlık kolonlarının başlangıç genişliği 30 olarak ayarlandı
                                                          // worksheet.Column(col).AdjustToContents(); // İçeriğe göre kolon genişliğini ayarla
                    }

                    // Görsel kolon genişliği sabit, diğer kolonlar içerik genişliğine göre otomatik ayarlanır
                    worksheet.Column(5).Width = 35; // Resim kolonunun genişliği

                    //saveService.SaveAndView("Modified Data.xlsx", "application/octet-stream", workbook.ToStream());
                    //string appFolder = Path.Combine(FileSystem., "Images");

                    int totalRows = worksheet.RowsUsed().Count();

                    // En alt satırı bul
                    int footerRow = totalRows + 3; // Kullanılan son satırın altına yeni bir satır ekle

                    // F-G-H-I-J sütunlarını birleştir
                    worksheet.Range(footerRow, 6, footerRow, 11).Merge();  // F-G-H-I-J-K sütunlarını birleştir
                    worksheet.Cell(footerRow, 6).Value = orderList.Description;
                    worksheet.Cell(footerRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                                                .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                                                .Font.SetBold().Font.SetFontSize(14) // Yazıyı kalın yap
                                                                .Fill.SetBackgroundColor(XLColor.Gray) // Arka plan rengi açık gri
                                                                .Border.SetOutsideBorder(XLBorderStyleValues.Thin); // Dış kenarlara border ekle

                    // Satır yüksekliğini 20 birim olarak ayarla
                    worksheet.Row(footerRow).Height = 20;

                    var random = new Random();
                    var xd = random.Next(1, 999999);


                    string filePath = "";

#if WINDOWS
                    filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + orderList.Title + "_" + xd + ".xlsx";


#elif ANDROID

                    filePath = System.IO.Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath, orderList.Title + "_" + xd + ".xlsx");

#else

                     filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), orderList.Title + "_" + xd + ".xlsx");

#endif
                    workbook.SaveAs(filePath);


                    //await Share.RequestAsync(new ShareFileRequest
                    //{
                    //    Title = "Excel Dosyası Paylaş",
                    //    File = new ShareFile(filePath),

                    //});
                }




            }
            catch (Exception ex)
            {

                ExceptionAlert.ShowAlert("excel hataaa", ex.Message);

            }

            return true;
        }


        public static string DownloadImageFromUrl(string url)
        {
            string tempFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString() + ".jpg");

            using (HttpClient client = new HttpClient())
            {
                byte[] imageData = client.GetByteArrayAsync(url).Result;
                File.WriteAllBytes(tempFilePath, imageData);
            }

            return tempFilePath;
        }

        public async static Task<bool> ExportToSaleExcel(int saleId)
        {
            var model = await DatabaseService.GetSaleWithSaleDetails(saleId);
            string imagePath = "";
#if WINDOWS
                imagePath = model.Product.ImageURL; //Path.Combine(product.ImagePath);
#else
            imagePath = $"{AppSettings.AppConst.ImageAddress}/{model.Product.ImageURL}";
#endif
            try
            {
                var workbook = new XLWorkbook();
                var worksheet = workbook.AddWorksheet("Sheet1");

                // Resmin yerleştirileceği hücreyi belirle
                var imageCell = worksheet.Cell("C2");

                // Resmi yükle ve boyutunu hücreye uydur
                if (imagePath != null)
                {
                    //var image = System.Drawing.Image.FromFile(imagePath);

#if WINDOWS
                        var picture = worksheet.AddPicture(imagePath).MoveTo(imageCell).WithSize(255, 140);            
#else
                    string tempImagePath = DownloadImageFromUrl(imagePath);
                    var picture = worksheet.AddPicture(tempImagePath).MoveTo(imageCell).WithSize(255, 140);
#endif
                }

                // Hücreleri birleştir
                worksheet.Range("C2:D8").Merge();

                // Alt bilgi satırlarını ekle
                worksheet.Cell("C9").Value = "Model Numarası:";
                worksheet.Cell("C10").Value = "Yaş/Size:";
                worksheet.Cell("C11").Value = "Model Açıklaması: ";
                worksheet.Cell("C12").Value = "İç Kumaş Adı: ";
                worksheet.Cell("C13").Value = "İç Kumaş İçeriği: ";
                worksheet.Cell("C14").Value = "İç Kumaş Tedariği: ";
                worksheet.Cell("C15").Value = "Dış Kumaş Adı: ";
                worksheet.Cell("C16").Value = "Dış Kumaş İçeriği: ";
                worksheet.Cell("C17").Value = "Dış Kumaş Tedariği: ";
                for (int i = 9; i <= 17; i++)
                {
                    worksheet.Cell("C" + i).Style.Font.SetFontSize(14).Font.SetBold();
                    worksheet.Cell("D" + i).Style.Font.SetFontSize(14);
                }

                worksheet.Cell("D9").Value = model.Product.ModelNumber;
                worksheet.Cell("D10").Value = model.Product.YasSize;
                worksheet.Cell("D11").Value = model.Product.ModelDescription;
                worksheet.Cell("D12").Value = model.Product.InnerFabricName;
                worksheet.Cell("D13").Value = model.Product.InnerFabricContent;
                worksheet.Cell("D14").Value = model.Product.InnerFabricSupply;
                worksheet.Cell("D15").Value = model.Product.OuterFabricName;
                worksheet.Cell("D16").Value = model.Product.OuterFabricContent;
                worksheet.Cell("D17").Value = model.Product.OuterFabricSupply;



                // Başlıkları ekleyin (C18 hücresinin altına)
                var lastRow = worksheet.LastRowUsed().RowNumber();
                worksheet.Cell(lastRow + 1, 3).Value = "MALZEME";
                worksheet.Cell(lastRow + 1, 3).Style.Font.SetBold().Font.SetFontSize(14);
                worksheet.Cell(lastRow + 1, 3).Style.Fill.BackgroundColor = XLColor.LightGray; // Açık gri arka plan

                worksheet.Cell(lastRow + 1, 4).Value = "BİRİM";
                worksheet.Cell(lastRow + 1, 4).Style.Font.SetBold().Font.SetFontSize(14);
                worksheet.Cell(lastRow + 1, 4).Style.Fill.BackgroundColor = XLColor.LightGray; // Açık gri arka plan

                worksheet.Cell(lastRow + 1, 5).Value = "BİRİM FİYAT";
                worksheet.Cell(lastRow + 1, 5).Style.Font.SetBold().Font.SetFontSize(14);
                worksheet.Cell(lastRow + 1, 5).Style.Fill.BackgroundColor = XLColor.LightGray; // Açık gri arka plan

                worksheet.Cell(lastRow + 1, 6).Value = "TOPLAM";
                worksheet.Cell(lastRow + 1, 6).Style.Font.SetBold().Font.SetFontSize(14);
                worksheet.Cell(lastRow + 1, 6).Style.Fill.BackgroundColor = XLColor.LightGray; // Açık gri arka plan


                // İçerikleri modelden eklemek için döngü
                int rowIndex = lastRow + 2; // Başlıkların altına eklemeye başlıyoruz
                foreach (var item in model.SaleDetail)
                {
                    worksheet.Cell(rowIndex, 3).Value = item.Material.Name; // Malzeme
                    worksheet.Cell(rowIndex, 4).Value = item.Unit.ToString("F2"); // Birim
                    worksheet.Cell(rowIndex, 5).Value = item.UnitePrice.ToString("F2") + " " + item.MarketRateType; // Birim fiyat
                    worksheet.Cell(rowIndex, 6).Value = item.Price.ToString("F2"); ; // Toplam
                    rowIndex++; // Bir sonraki satıra geç
                }

                rowIndex++;
                // Genel Toplam
                worksheet.Cell(rowIndex, 5).Value = "Genel Toplam:";
                worksheet.Cell(rowIndex, 6).Value = model.GenelToplam.ToString("F2"); // Genel toplam hesaplama
                worksheet.Cell(rowIndex, 5).Style.Font.SetBold().Font.SetFontSize(14);
                // worksheet.Cell(rowIndex, 7).Style.Font.SetBold().Font.SetFontSize(14);
                // worksheet.Cell(rowIndex, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                //kar oranı
                rowIndex++;
                worksheet.Cell(rowIndex, 5).Value = "Kar Oranı:";
                worksheet.Cell(rowIndex, 6).Value = model.KarOrani.ToString("F2");
                worksheet.Cell(rowIndex, 5).Style.Font.SetBold().Font.SetFontSize(14);
                //worksheet.Cell(rowIndex, 7).Style.Font.SetBold().Font.SetFontSize(14);

                //kar oranı   rowIndex++;+
                rowIndex++;
                worksheet.Cell(rowIndex, 5).Value = "Elde edilen  kar :";
                worksheet.Cell(rowIndex, 6).Value = model.EldeEdilenKar.ToString("F2"); ;
                worksheet.Cell(rowIndex, 5).Style.Font.SetBold().Font.SetFontSize(14);
                //worksheet.Cell(rowIndex, 7).Style.Font.SetBold().Font.SetFontSize(14);

                rowIndex++;
                //Satış Fiyatı
                worksheet.Cell(rowIndex, 5).Value = "Satış Fiyatı:";
                worksheet.Cell(rowIndex, 6).Value = model.SatisFiyati.ToString("F2"); ;
                worksheet.Cell(rowIndex, 5).Style.Font.SetBold().Font.SetFontSize(14);
                // worksheet.Cell(rowIndex, 7).Style.Font.SetBold().Font.SetFontSize(14);

                worksheet.Column("C").AdjustToContents();
                worksheet.Column("D").AdjustToContents();
                worksheet.Column("E").AdjustToContents();
                worksheet.Column("F").AdjustToContents();

                // Excel dosyasını kaydet
                //string filePath = 

                string filePath = "";

#if WINDOWS
                filePath =Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + "Maliyet" + "_" + DateTime.Now.Ticks + ".xlsx";
#elif ANDROID
                filePath = System.IO.Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath, "Maliyet" + "_" + DateTime.Now.Ticks + ".xlsx");
#else
                filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Maliyet" + "_" + DateTime.Now.Ticks + ".xlsx");
#endif

                workbook.SaveAs(filePath);


            }
            catch (Exception ex)
            {
                ExceptionAlert.ShowAlert("excel hataaa", ex.Message);
                //DatabaseService.AddLogData(ex.Message, "SaleExcelEx");
            }

            return true;
        }



        public async static Task<bool> ExportToSaleExcelIOS(int saleId)
        {
            try
            {
                string url = "MobileService/ExportToSaleExcel?saleId=" + saleId;
                var response = await Export1(url);
                if (String.IsNullOrEmpty(response))
                {
                    return false;
                }
                await DownloadAndSaveFile(response);
                return true;
            }
            catch (Exception)
            {
                return false;

            }
        }
        public async static Task<bool> ExportToExcelIOS(int orderId)
        {
            try
            {
                //"https://maliyetapp.emretoksoz.com/MobileService/UploadImage";
                string url = "MobileService/ExportToExcel?orderId=" + orderId;

                var response = await Export1(url);

                if (String.IsNullOrEmpty(response))
                {
                    return false;
                }
                await DownloadAndSaveFile(response);
                return true;
            }
            catch (Exception)
            {
                return false;

            }
        }
        private static string baseApiUrl = AppConst.HostAddress;
        private async static Task<string> Export1( string url)
        {

            try
            {

                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };
                using (var httpClient = new HttpClient(handler))
                {
                    var urltest = $"{baseApiUrl}{url}";
                    var response = await httpClient.GetAsync(urltest);
                    response.EnsureSuccessStatusCode();
                    var responseData = await response.Content.ReadAsStringAsync();
                    if (String.IsNullOrEmpty(responseData))
                    {
                        return "";
                    }
                    return responseData;
                }
            }
            catch (Exception ex)
            {
                // Hata yönetimi
                ExceptionAlert.ShowAlert("Hata", $"Hata oluştu: {ex.Message}");
                return "";
            }
        }
        public async static Task DownloadAndSaveFile(string fileUrl)
        {
            try
            {
                var client = new HttpClient();


                var url = AppConst.ExcelPath + fileUrl;
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var fileBytes = await response.Content.ReadAsByteArrayAsync();

                    var random = new Random();
                    var xd = random.Next(1, 999999);
                    // iOS cihazında geçerli bir yol almak için FileSystem kullanabilirsiniz
                    // var filePath = System.IO.Path.Combine(FileSystem.AppDataDirectory, xd + "_" + "maliyetexcel.xlsx");
                    var filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), xd + "_" + "maliyetexcel.xlsx");

                    // Dosyayı kaydet
                    await File.WriteAllBytesAsync(filePath, fileBytes);
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                ExceptionAlert.ShowAlert("Hata", ex.Message);
            }
        }

    }

}
