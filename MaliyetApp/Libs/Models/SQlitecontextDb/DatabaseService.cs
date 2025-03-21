using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using MaliyetApp.Libs.AppServices;
using MaliyetApp.Libs.AppSettings;
using SQLite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MaliyetApp.Libs.Models.SQlitecontextDb
{
    public class DatabaseService
    {
     
        private static readonly string DatabaseFileName = "maliyetprogram.db";
        private static readonly string BackupFileName = "maliyetprogrambackupdata.db";
        // Veritabanı yolu
        public static string AppDataDirectory => 
            //DeviceInfo.Platform == (DevicePlatform.Android) ? 
            //Path.Combine(FileSystem.AppDataDirectory, DatabaseFileName) : (DeviceInfo.Platform == DevicePlatform.iOS ? 
            //Path.Combine(FileSystem.AppDataDirectory, DatabaseFileName) :
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "maliyetprogram.db"); 
        public static string BackupFilePath => 
            //DeviceInfo.Platform == (DevicePlatform.Android) ?
            //Path.Combine(FileSystem.AppDataDirectory, BackupFileName) : (DeviceInfo.Platform == DevicePlatform.iOS ?
            //Path.Combine(FileSystem.AppDataDirectory, BackupFileName) : 
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "maliyetprogrambackupdata.db"); 

        private static int takeCount = DeviceInfo.Platform == (DevicePlatform.Android) ? 8 : (DeviceInfo.Platform == DevicePlatform.iOS ? 8 : 16);
#if WINDOWS
        private readonly static SQLiteConnection _database = new SQLiteConnection(AppDataDirectory);
        private static DatabaseService _instance;
#endif
        private static readonly object _lock = new object();
        public DatabaseService()
        {
#if WINDOWS
                    // Tabloları oluştur

                    // _database.DropTable<Product>();
                    _database.CreateTable<Product>();

                    //_database.DropTable<Material>();
                    _database.CreateTable<Material>();

                    // _database.DropTable<Order>();
                    _database.CreateTable<Order>();

                    // _database.DropTable<OrderDetails>();
                    _database.CreateTable<OrderDetails>();

                    // _database.DropTable<Sale>();
                    _database.CreateTable<Sale>();

                    //_database.DropTable<SaleDetail>();
                    _database.CreateTable<SaleDetail>();
                    _database.CreateTable<BaseLog>();
#endif
#if WINDOWS
                        InitializeMaterialData();
#endif
            //  TempData ss = new TempData();
        }
    #if WINDOWS
            public static DatabaseService Instance
            {

                get
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new DatabaseService();
                        }
                        return _instance;
                    }
                }
            }
    #endif
        // Veritabanını yedekleme
        public static void BackupDatabase(string backupPath)
        {
            try
            {
                // Yedekleme dosyasını oluştur
                File.Copy(BackupFilePath, backupPath, true);
       
            }
            catch (Exception ex)
            {
                // Hata durumunda işlem yap
            }
        }

        public static void AddLogData<T>(T data,string method)
        {
            if (data!=null)
            {
                    #if WINDOWS
                var serialize = JsonSerializer.Serialize(data);
                _database.Insert(new BaseLog { Name = serialize,Method=method });
#endif
            }
       
        }
        private void InitializeMaterialData()
        {
            #if WINDOWS
                        var setupCompleted = _database.Table<Material>().FirstOrDefault();

                        if (setupCompleted == null)
                        {
                            TempData.InitializeMaterial();
                        }
            #endif
        }
        // Material CRUD İşlemleri
        public static async  Task<int> CreateMetarial(Material material) {

            material.LastModificationTime = DateTime.Now;
            try
            {
#if WINDOWS
           
                                _database.Insert(material); 
                                AddLogData(material, "Material");
                                return material.Id; 
#else
                return await MobileService.PostAsync<Material, int>("MobileService/CreateMaterial", material); 

#endif
            }
            catch (Exception ex)
            {

                ExceptionAlert.ShowAlert("Uyarı",ex.Message);
                return -1;
            }
        }

        public static async Task<List<Material>> GetAllMaterials()
        {
            try
            {
#if WINDOWS
                                    return  _database.Table<Material>().Where(a => !a.IsDeleted).ToList();
#else
                var test= await MobileService.GetAsync<List<Material>>("MobileService/GetAllMaterials");
                return test;
#endif
            }
            catch (Exception ex)
            {

                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return null;
            }
        }
        public static async Task<Material> GetMaterialById(long id) 
        {



#if WINDOWS
                       return   _database.Find<Material>(id);
#else
                            return await MobileService.GetAsync<Material>("MobileService/GetMaterialById?id="+id);
#endif


        } // ID'ye göre getir
        public static async Task<List<Material>> GetMaterialsByType(string type) {


            try
         {
#if WINDOWS
                                               return      _database.Table<Material>().Where(m => m.Type == type && !m.IsDeleted).ToList(); 
#else
                            return await MobileService.GetAsync<List<Material>>("MobileService/GetMaterialsByType?type=" + type);
#endif

            }
            catch (Exception ex)
            {

                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return null;
            }
        
        
        }
        public static async Task UpdateMaterial(Material material){

            try
            {
                material.LastModificationTime = DateTime.Now;
#if WINDOWS
                      material.IsDeleted = false; 
                      AddLogData(material, "Material"); 
                      _database.Update(material);

#else
                          var xd=   await MobileService.PostAsync<Material,string>("MobileService/UpdateMaterial",material);
#endif
            }
            catch (Exception ex)
            {
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
            }
        
        
        }
        public static async Task DeleteMaterial(long id) 
        {
            try
            {


#if WINDOWS
                                var model = _database.Find<Material>(id);
                                model.IsDeleted = true;
                                    model.LastModificationTime = DateTime.Now;
                                _database.Update(model);
                                AddLogData(model,"Material");

#else
                await MobileService.GetAsync<string>("MobileService/DeleteMaterial?id="+id);
#endif
            }
            catch (Exception ex)
            {
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);

            }

        }

        // SaleDetail CRUD İşlemleri
        public static async Task<int> CreateSaleDetails(SaleDetail saleDetail) 
        {
            saleDetail.LastModificationTime = DateTime.Now;

            try
            {

#if WINDOWS
                                        _database.Insert(saleDetail);
                                        AddLogData(saleDetail, "SaleDetail");
                                        return saleDetail.Id;

#else
                                return await MobileService.PostAsync<SaleDetail, int>("MobileService/CreateSaleDetails", saleDetail);
#endif
            }
            catch (Exception ex)
            {
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return -1;
            }

        }
        public static async Task<List<SaleDetail>> GetAllSaleDetails()
        {


            try
            {
#if WINDOWS
              return  _database.Table<SaleDetail>().Where(a => !a.IsDeleted).ToList();

#else
             return await MobileService.GetAsync<List<SaleDetail>>("MobileService/GetAllSaleDetails");
#endif
            }
            catch (Exception ex)
            {
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return null;
            }

        }
        public static async  Task<SaleDetail> GetSaleDetailById(long id) { 
            

            try
            {
#if WINDOWS
                       return  _database.Find<SaleDetail>(id);
#else
                                return await MobileService.GetAsync<SaleDetail>("MobileService/GetSaleDetailById?id="+id);
#endif
            }
            catch (Exception ex)
            {
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return null;
            }

        }// ID'ye göre getir

    
        public async static Task<List<SaleDetail>> GetSaleDetailsBySaleId(int sale)
        {



            try
            {
#if WINDOWS
                   return   _database.Table<SaleDetail>().Where(sd => sd.SaleId == sale&&!sd.IsDeleted).ToList();
#else
                 return await MobileService.GetAsync<List<SaleDetail>>("MobileService/GetSaleDetailsBySaleId?id=" + sale);
#endif
            }
            catch (Exception ex)
            {
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return null;
            }

        }
        public static async Task UpdateSaleDetail(SaleDetail saleDetail)
        {
            saleDetail.LastModificationTime = DateTime.Now;
            try
            {
#if WINDOWS
                             saleDetail.IsDeleted = false; 
                              
                            _database.Update(saleDetail);
                            AddLogData(saleDetail, "SaleDetail");
#else
                await MobileService.PostAsync<SaleDetail,string>("MobileService/UpdateSaleDetail", saleDetail);
#endif
            }
            catch (Exception ex)
            {
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
            }
        }
        public static  async Task DeleteSaleDetail(int id) {
            
            //_database.Delete<SaleDetail>(id);
           


            try
            {
#if WINDOWS
                         var model = _database.Find<SaleDetail>(id);
                        model.IsDeleted = true;
                            model.LastModificationTime = DateTime.Now;
                        _database.Update(model);
                        AddLogData(model, "SaleDetail");
#else
                await MobileService.GetAsync<SaleDetail>("MobileService/DeleteSaleDetail?id="+ id);
#endif
            }
            catch (Exception ex)
            {
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
            }

        }


        // Product CRUD İşlemleri
        public static async Task<int> CreateProduct(Product product, string year, bool isEntagrasyon = false)
        {
           
            int parsedYear;
            product.LastModificationTime = DateTime.Now;

            try
            {
#if WINDOWS
                product.IsUpdate=false;
             if(!isEntagrasyon){
                    if (year != null && int.TryParse(year, out parsedYear) && parsedYear != 0)
                    {
                        var now = DateTime.Now;
                        product.Creationtime = new DateTime(parsedYear, now.Month, now.Day).ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        product.Creationtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                        parsedYear = DateTime.Now.Year;
                    }
            }
                                    _database.Insert(product);
                                    AddLogData(product, "product");
                                    return product.Id;

#else

                if (year != null && int.TryParse(year, out parsedYear) && parsedYear != 0)
                {
                    var now = DateTime.Now;
                    product.Creationtime = new DateTime(parsedYear, now.Month, now.Day).ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                }
                else
                {
                    product.Creationtime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                    parsedYear = DateTime.Now.Year;
                }
                return   await MobileService.PostAsync<Product,int>("MobileService/CreateProduct?year=" + parsedYear, product);

#endif
            }
            catch (Exception ex)
            {
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return -1;
            }

        }
        public static async Task<bool> GetOrderDetailCheckSaleWindows( int orderId,  int saleId)
        {
#if WINDOWS

            return  _database.Table<OrderDetails>().Where(a => !a.IsDeleted && a.OrderId == orderId && a.SaleId == saleId).Any();
#endif

            return false;
        }
        public static async Task<List<Product>> GetAllProduct(string year)
        {
            

            try
            {
#if WINDOWS
      
                                var model = _database.Table<Product>();
                                int parsedYear;
                                if (year != null && int.TryParse(year, out parsedYear) && parsedYear != 0)
                                {
                                        var queryModel = model.AsEnumerable().Where(a =>
                                        {
                                            if (DateTime.TryParse(a.Creationtime, out DateTime creationDate))
                                            {
                                                return creationDate.Year == (year != null ? parsedYear : DateTime.Now.Year) && !a.IsDeleted;
                                            }
                                            return false;
                                        });
                                         return queryModel.ToList();
                                }
                                else{
                                    var queryModel = model.AsEnumerable().Where(a =>
                                    {
                                        if (DateTime.TryParse(a.Creationtime, out DateTime creationDate))
                                        {
                                            return creationDate.Year ==  DateTime.Now.Year && !a.IsDeleted;
                                        }
                                        return false;
                                    });

                                    return queryModel.ToList();
                                }



#else
                return await MobileService.GetAsync<List<Product>>("MobileService/GetAllProduct");
#endif
            }
            catch (Exception ex)
            {
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return null;
            }


          

        }


        public static async Task<List<Product>> GetFilterProduct(string searchText,string year="",int?page=0,int?exportCount=0)
        {
            if (string.IsNullOrEmpty(searchText))
                return new List<Product>();

            var skipCount = (page.HasValue ? page.Value : 0) * takeCount;

            if (exportCount.HasValue && exportCount.Value>0)
            {
                skipCount = 0;
                takeCount = 50;
            }
#if WINDOWS
      



                        var model = _database.Table<Product>().Where(a =>
                       ((a.ModelNumber != null && a.ModelNumber.ToLower().Contains(searchText.ToLower())) ||
                        (a.Brand != null && a.Brand.ToLower().Contains(searchText.ToLower())) ||
                        (a.YasSize != null && a.YasSize.ToLower().Contains(searchText.ToLower())) ||
                        (a.InnerFabricSupply != null && a.InnerFabricSupply.ToLower().Contains(searchText.ToLower())) ||
                        (a.InnerFabricName != null && a.InnerFabricName.ToLower().Contains(searchText.ToLower())) ||
                        (a.OuterFabricSupply != null && a.OuterFabricSupply.ToLower().Contains(searchText.ToLower())) ||
                        (a.OuterFabricName != null && a.OuterFabricName.ToLower().Contains(searchText.ToLower()))) &&
                       !a.IsDeleted);



                        try
                        {
                            int parsedYear;
                            if (year != null && int.TryParse(year, out parsedYear) && parsedYear != 0)
                            {
                                //return model.AsEnumerable().Where(a =>
                                //{
                                //    DateTime creationDate;
                                //    return DateTime.TryParse(a.Creationtime, out creationDate) && creationDate.Year == parsedYear;

                                //}).ToList();
                                var queryModel = model.AsEnumerable().Where(a =>
                                {
                                    if (DateTime.TryParse(a.Creationtime, out DateTime creationDate))
                                    {
                                        return creationDate.Year == (year != null ? parsedYear : DateTime.Now.Year) && !a.IsDeleted;
                                    }
                                    return false;
                                });

                                if (page.HasValue && queryModel.Count() > 0)
                                    return model.Skip(skipCount).Take(takeCount).ToList();
                                else
                                    return model.ToList();

                            }
                            else
                            {
                                var queryModel= model.AsEnumerable().Where(a =>
                                {
                                    DateTime creationDate;
                                    return DateTime.TryParse(a.Creationtime, out creationDate) && creationDate.Year == DateTime.Now.Year && !a.IsDeleted;
                                });

                                if (page.HasValue && queryModel.Count() > 0)
                                    return model.Skip(skipCount).Take(takeCount).ToList();
                                else
                                    return model.ToList();

                            }

                            return null;
                        }
                        catch (Exception ex)
                        {
                            AddLogData(ex.Message, "product");
                            return null;
                        }

#else

            try
            {
                return await MobileService.GetAsync<List<Product>>("MobileService/GetFilterProduct?searchText=" + searchText + "&year=" + year + "&page=" + page);
            }
            catch (Exception ex)
            {
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return null;
            }
#endif
        }


        public static async Task<Product> GetProductById(int id)
        {


            try
            {
#if WINDOWS
      
                                                return    _database.Find<Product>(id);

#else
                                return await MobileService.GetAsync<Product>("MobileService/GetProductById?id="+id);
#endif
            }
            catch (Exception ex)
            {
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return null;
            }
        }
        public static async Task<Product> GetProductBySqliteId(int sqliteId)
        {


            try
            {
#if WINDOWS


                return _database.Find<Product>(a=>a.SqliteId==sqliteId);

#endif
                return null;
            }
            catch (Exception ex)
            {
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return null;
            }
        }
        public static async Task<Material> GetMaterialBySqliteId(long sqliteId)
        {

            try
            {
#if WINDOWS
                return _database.Find<Material>(a => a.SqliteId == sqliteId);
#endif
                return null;
            }
            catch (Exception ex)
            {
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return null;
            }

        }
        public static async Task<Sale> GetSaleBySqliteId(long sqliteId)
        {

            try
            {
#if WINDOWS
                return _database.Find<Sale>(a => a.SqliteId == sqliteId);
#endif
                return null;
            }
            catch (Exception ex)
            {
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return null;
            }

        }
        public static async Task<Order> GetOrderBySqliteId(long sqliteId)
        {

            try
            {
#if WINDOWS
                return _database.Find<Order>(a => a.SqliteId == sqliteId);
#endif
                return null;
            }
            catch (Exception ex)
            {
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return null;
            }

        }
        public static async  Task UpdateProduct(Product product)
        {

            product.LastModificationTime = DateTime.Now;

            try
            {
#if WINDOWS
    
                            product.IsDeleted = false;
                            _database.Update(product);
                            AddLogData(product, "product");

#else
                await MobileService.PostAsync<Product,string>("MobileService/UpdateProduct",product);
#endif
            }
            catch (Exception ex)
            {
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
               
            }
        }
        public static async Task DeleteProduct(int id)
        {
           
            // => _database.Delete<Product>(id);


            try
            {
#if WINDOWS
      
                          var model = _database.Find<Product>(id);
                            model.IsUpdate=true;
            model.IsDeleted = true;
              model.LastModificationTime = DateTime.Now;
            _database.Update(model);
            AddLogData(model, "product");

#else
                await MobileService.GetAsync<string>("MobileService/DeleteProduct?id="+id);
#endif
            }
            catch (Exception ex)
            {
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);

            }
        }


        // Sale CRUD İşlemleri
        public static async Task<int> CreateSale(Sale sale,string year,bool isEntegrasyon=false) {



            sale.LastModificationTime = DateTime.Now;

            try
            {
#if WINDOWS
      
                  if(!isEntegrasyon){
            int parsedYear;
            if (year != null && int.TryParse(year, out parsedYear) && parsedYear != 0)
            {
                var now = DateTime.Now;
                sale.Creationtime = new DateTime(parsedYear, now.Month, now.Day).ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            }
            else
            {
                sale.Creationtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            }
            }
            _database.Insert(sale); 
            
            AddLogData(sale, "sale");
            return sale.Id;
#else
                year = String.IsNullOrEmpty(year) ? DateTime.Now.Year.ToString() : year;

                sale.Creationtime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

                return  await MobileService.PostAsync<Sale,int>("MobileService/CreateSale?year=" + year,sale);
#endif
            }
            catch (Exception ex)
            {
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return -1;
            }


        }
        public static async  Task<List<Sale>> GetAllSale(string year,int?page=0,int? exportPage = 0,bool asyncProgress=false)
        {
#if WINDOWS
            try
            {
                var skipCount = (page.HasValue ? page.Value : 0)* takeCount;

                if (exportPage.HasValue && exportPage.Value>0)
                {
                    skipCount = 0;
                    skipCount= page.Value * 10;
                    takeCount = 10;
                }

                int parsedYear;
                if (year != null && int.TryParse(year, out parsedYear) && parsedYear != 0)
                {
                    var model = _database.Table<Sale>().AsEnumerable().Where(a =>
                    {
                        DateTime creationDate;
                        return DateTime.TryParse(a.Creationtime, out creationDate) && creationDate.Year == parsedYear && !a.IsDeleted;
                    });

                    if (page.HasValue && model.Count() > 0 && !asyncProgress)
                    {
                      return model.OrderByDescending(a=>a.Id).Skip(skipCount).Take(takeCount).ToList();
                    }
                    else{
                        return model.OrderByDescending(a => a.Id).ToList();
                    }
                }
                else
                {
                    var model = _database.Table<Sale>().AsEnumerable().Where(a =>
                    {
                        DateTime creationDate;
                        return DateTime.TryParse(a.Creationtime, out creationDate) && creationDate.Year == DateTime.Now.Year && !a.IsDeleted;
                    });
               
                    if (page.HasValue && model.Count()>0 && !asyncProgress)
                    {
                        return model.OrderByDescending(a => a.Id).Skip(skipCount).Take(takeCount).ToList();
                    }
                    else
                    {
                        return model.OrderByDescending(a => a.Id).ToList();
                    }
                }

            }
            catch (Exception xe)
            {

                AddLogData(xe.Message, "sale");
                return null;
            }
#else
            try
            {
                return await MobileService.GetAsync<List<Sale>>("MobileService/GetAllSale?year=" + year + "&page=" + page);
            }
            catch (Exception ex)
            {

                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return null;
            }

#endif
        }

        public static async Task<List<Sale>> GetAllSaleIncludeDelete(string year, int? page = 0, int? exportPage = 0, bool asyncProgress = false)
        {
#if WINDOWS
            try
            {
                var skipCount = (page.HasValue ? page.Value : 0)* takeCount;

                if (exportPage.HasValue && exportPage.Value>0)
                {
                    skipCount = 0;
                    skipCount= page.Value * 10;
                    takeCount = 10;
                }

                int parsedYear;
                if (year != null && int.TryParse(year, out parsedYear) && parsedYear != 0)
                {
                    var model = _database.Table<Sale>().AsEnumerable().Where(a =>
                    {
                        DateTime creationDate;
                        return DateTime.TryParse(a.Creationtime, out creationDate) && creationDate.Year == parsedYear;
                    });

                    if (page.HasValue && model.Count() > 0 && !asyncProgress)
                    {
                      return model.OrderByDescending(a=>a.Id).Skip(skipCount).Take(takeCount).ToList();
                    }
                    else{
                        return model.OrderByDescending(a => a.Id).ToList();
                    }
                }
                else
                {
                    var model = _database.Table<Sale>().AsEnumerable().Where(a =>
                    {
                        DateTime creationDate;
                        return DateTime.TryParse(a.Creationtime, out creationDate) && creationDate.Year == DateTime.Now.Year;
                    });
               
                    if (page.HasValue && model.Count()>0 && !asyncProgress)
                    {
                        return model.OrderByDescending(a => a.Id).Skip(skipCount).Take(takeCount).ToList();
                    }
                    else
                    {
                        return model.OrderByDescending(a => a.Id).ToList();
                    }
                }

            }
            catch (Exception xe)
            {

                AddLogData(xe.Message, "sale");
                return null;
            }
#else
            try
            {
                return await MobileService.GetAsync<List<Sale>>("MobileService/GetAllSale?year=" + year + "&page=" + page);
            }
            catch (Exception ex)
            {

                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return null;
            }

#endif
        }

        public static async Task<List<Sale>> GetAllSaleForMainPage(string year,List<int> ids, List<int> _saleIds = null)
        {
            try
            {

#if WINDOWS
                 try
                            {

                                int parsedYear;
                                if (year != null && int.TryParse(year, out parsedYear) && parsedYear != 0)
                                {
                                    var model = _database.Table<Sale>().AsEnumerable().Where(a =>
                                    {
                                        DateTime creationDate;
                                        return DateTime.TryParse(a.Creationtime, out creationDate) && creationDate.Year == parsedYear && !a.IsDeleted && ids.Contains(a.ProductId);
                                    });


                                    if (_saleIds != null && _saleIds.Count > 0)
                                    {
                                        return model.Where(a => !_saleIds.Contains(a.Id)).ToList();
                                    }
                                    else
                                    {
                                        return model.ToList();
                                    }
                                }
                                else
                                {
                                    var model = _database.Table<Sale>().AsEnumerable().Where(a =>
                                    {
                                        DateTime creationDate;
                                        return DateTime.TryParse(a.Creationtime, out creationDate) && creationDate.Year == DateTime.Now.Year && !a.IsDeleted && ids.Contains(a.ProductId);
                                    });

                                    if (_saleIds != null && _saleIds.Count > 0)
                                    {
                                        return model.Where(a => !_saleIds.Contains(a.Id)).ToList();
                                    }
                                    else
                                    {
                                        return model.ToList();
                                    }
                                }

                            }
                            catch (Exception xe)
                            {

                                AddLogData(xe.Message, "sale");
                                return null;
                            }

#else

                string _saleIdsString = "";
                if (_saleIds != null && _saleIds.Count > 0)
                {
                    _saleIdsString = JsonSerializer.Serialize(_saleIds);
                }
                string _idsString = "";
                if (ids != null && ids.Count > 0)
                {
                    _idsString = JsonSerializer.Serialize(ids);
                }
                return await MobileService.GetAsync<List<Sale>>("MobileService/GetAllSaleForMainPage?year=" + year + "&ids="+ _idsString + "&_saleIds="+ _saleIdsString);

#endif
            }
            catch (Exception ex) 
            {

                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return null;
            }
        }
        public static async Task<List<Sale>> GetFilterSaleAndProduct(string searchText,string year="",int? page=0,int?exportCount=0)
        {

            try
            {
#if WINDOWS
                                  var productList = await GetFilterProduct(searchText, year, page, exportCount);
                                                if (productList != null && productList.Count > 0)
                                                {
                                                    var ids = productList.Select(a => a.Id).ToList();
                                                    var sale = ( await GetAllSaleForMainPage(year, ids)).ToList();
                                                    foreach (var item in sale)
                                                    {
                                                        item.SaleDetail = await GetSaleDetailsBySaleId(item.Id);
                                                        foreach (var item2 in item.SaleDetail)
                                                        {
                                                            item2.Material = await GetMaterialById(item2.MaterialId);
                                                        }
                                                        item.Product = await GetProductById(item.ProductId);
                                                    }
                                                    return sale;
                                                }
                                                return null;
#else
                            return await MobileService.GetAsync<List<Sale>>("MobileService/GetFilterSaleAndProduct?searchText=" + searchText + "&year=" + year + "&page=" + page);
#endif

            }
            catch (Exception ex)
            {
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);

                return null;
            }
        }
        public static async Task<List<Sale>> GetFilterSaleAndProductNewMothod(string searchText, string year = "", int? page = 0, int? exportCount = 0)
        {

            try
            {
#if WINDOWS
                            var productList = await GetFilterProduct(searchText, year, page, exportCount);
                            if (productList != null && productList.Count > 0)
                            {
                                var ids = productList.Select(a => a.Id).ToList();
                                var sale = (await GetAllSaleForMainPage(year, ids)).ToList();
                                foreach (var item in sale)
                                {
                                    item.Product = await GetProductById(item.ProductId);
                                }
                                return sale;
                            }
                            return null;
#else
                            return await MobileService.GetAsync<List<Sale>>("MobileService/GetFilterSaleAndProductNewMothod?searchText=" + searchText + "&year=" + year + "&page=" + page);
#endif
            }
            catch (Exception ex)
            {
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return null;
            }
        }
        public static async Task<List<Sale>> GetAllSaleMain(string year,int ?page=0,int?exportPage=0) 
        {
            try
            {
#if WINDOWS
                                    var sale = await GetAllSale(year,page,exportPage);
                                    foreach (var item in sale)
                                    {

                                        item.SaleDetail = await GetSaleDetailsBySaleId(item.Id);
                                        foreach (var item2 in item.SaleDetail)
                                        {
                                            item2.Material = await GetMaterialById(item2.MaterialId);
                                        }
                                        item.Product = await GetProductById(item.ProductId);
                                    }
       
                                    return sale;
#else
                            return await MobileService.GetAsync<List<Sale>>("MobileService/GetAllSaleMain?year=" + year + "&page=" + page);
#endif
            }
            catch (Exception ex)
            {
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return null;
            }

        }

        public static async Task <List<Sale>> GetAllSaleMainNewMethod(string year, int? page = 0, int? exportPage = 0)
        {
            try
            {
#if WINDOWS
                                var sale = await GetAllSale(year, page, exportPage);
                                foreach (var item in sale)
                                {
                                    item.Product = await GetProductById(item.ProductId);
                                }

                                return sale;
#else
             
                                return await MobileService.GetAsync<List<Sale>>("MobileService/GetAllSaleMainNewMethod?year=" + year + "&page=" + page);
#endif

            }
            catch (Exception ex)
            {
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return null;
            }
        }


        public static async  Task<List<Sale>> GetAllSaleOrder(string year, int? page = 0, int? exportPage = 0,List<int> _saleIds=null)
        {
            try
            {
#if WINDOWS

                                var sale = await GetAllSaleOrderPopup(year, page, _saleIds);

                                return sale;
#else
                year = String.IsNullOrEmpty(year) ? DateTime.Now.Year.ToString() : year;

                string _saleIdsString = "";
                if (_saleIds!=null && _saleIds.Count>0)
                {
                    _saleIdsString = JsonSerializer.Serialize(_saleIds);
                }
                return await MobileService.GetAsync<List<Sale>>("MobileService/GetAllSaleOrder?year=" + year + "&page=" + page+ "&_saleIds="+ _saleIdsString);
#endif

            }
            catch (Exception ex)
            {
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return null;
            }

        }
        public static async Task<List<Sale>> GetAllSaleOrderPopup(string year, int? page = 0, List<int> _saleIds = null)
        {

            try
            {
#if WINDOWS
                                    var skipCount = (page.HasValue ? page.Value : 0) * 10;

                                    int parsedYear;
                                    if (year != null && int.TryParse(year, out parsedYear) && parsedYear != 0) { }
                                    else
                                    {
                                        parsedYear = DateTime.Now.Year;
                                    }
                                    DateTime creationDate;
                                    _saleIds = _saleIds ?? new List<int>();
                                    var query =    (from a in _database.Table<Sale>()
                                                    join b in _database.Table<Product>() on a.ProductId equals b.Id
                                                    where !_saleIds.Contains(a.Id)
                                                        && !a.IsDeleted 
                                                        && !b.IsDeleted 
                                                        && DateTime.TryParse(a.Creationtime, out creationDate) 
                                                        && creationDate.Year == parsedYear
                                                        select new Sale()
                                                        {
                                                            Id = a.Id,
                                                            EldeEdilenKar = a.EldeEdilenKar,
                                                            GenelToplam = a.GenelToplam,
                                                            KarOrani = a.KarOrani,
                                                            ProductId = a.ProductId,
                                                            IsDeleted = a.IsDeleted,
                                                            SatisFiyati = a.SatisFiyati,
                                                            Product = b

                                                        }).Skip(skipCount).Take(10).ToList();

                                    return query;

#else
                string _saleIdsString = "";
                if (_saleIds != null && _saleIds.Count > 0)
                {
                    _saleIdsString = JsonSerializer.Serialize(_saleIds);
                }
                return await MobileService.GetAsync<List<Sale>>("MobileService/GetAllSaleOrderPopup?year=" + year + "&page=" + page + "&_saleIds=" + _saleIdsString);
#endif

            }
            catch (Exception ex)
            {

#if WINDOWS
                                AddLogData(ex.Message, "sale");
#else
                                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
#endif

                return null;
            }
        }

        public static async  Task<List<Sale>> GetFilterSaleAndProductPopup(string searchText, string year = "", int? page = 0, int? exportCount = 0, List<int> _saleIds = null)
        {

            try
            {


#if WINDOWS
                int parsedYear;
                if (year != null && int.TryParse(year, out parsedYear) && parsedYear != 0)
                {

                }
                else
                {
                    parsedYear = DateTime.Now.Year;
                }
                DateTime creationDate;

                var skipCount = (page.HasValue ? page.Value : 0) * 10;
                _saleIds = _saleIds ?? new List<int>();

                var req = (from a in _database.Table<Sale>()
                           join b in _database.Table<Product>() on a.ProductId equals b.Id
                           where !_saleIds.Contains(a.Id) 
                           && !a.IsDeleted
                           && !b.IsDeleted 
                           && (!String.IsNullOrEmpty(searchText)?
                            ((b.ModelNumber != null && b.ModelNumber.ToLower().Contains(searchText.ToLower())) ||
                            (b.Brand != null && b.Brand.ToLower().Contains(searchText.ToLower()))):true) 
                            && DateTime.TryParse(a.Creationtime, out creationDate) && creationDate.Year == parsedYear
                            select new Sale()
                           {
                              Id=a.Id,
                              EldeEdilenKar=a.EldeEdilenKar,
                              GenelToplam=a.GenelToplam,
                             KarOrani=a.KarOrani,
                                ProductId=a.ProductId,
                               IsDeleted=a.IsDeleted,
                               SatisFiyati=a.SatisFiyati,
                               Product=b 

                           }).Skip(skipCount).Take(10).ToList();
             


                //var productList = GetFilterProductOrder(searchText, year, page, exportCount);
                //if (productList != null && productList.Count > 0)
                //{
                //    var ids = productList.Select(a => a.Id).ToList();
                //    var sale = GetAllSaleForMainPage(year, ids, _saleIds).ToList();
                //    foreach (var item in sale)
                //    {
                //        //item.SaleDetail = GetSaleDetailsBySaleId(item.Id);
                //        //foreach (var item2 in item.SaleDetail)
                //        //{
                //        //    item2.Material = GetMaterialById(item2.MaterialId);
                //        //}
                //        item.Product = GetProductById(item.ProductId);
                //    }
                //    return sale;
                //}
                return req;

#else
                string _saleIdsString = "";
                if (_saleIds != null && _saleIds.Count > 0)
                {
                    _saleIdsString = JsonSerializer.Serialize(_saleIds);
                }
                return await MobileService.GetAsync<List<Sale>>("MobileService/GetFilterSaleAndProductPopup?searchText="+ searchText+"&year="+year + "&page=" + page + "&_saleIds=" + _saleIdsString);

#endif
            }
            catch (Exception ex)
            {

#if WINDOWS
                                                AddLogData(ex.Message, "sale");
#endif
                                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return null;
            }
        }
        public static async Task<List<Product>> GetFilterProductOrder(string searchText, string year = "", int? page = 0, int? exportCount = 0)
        {

            if (string.IsNullOrEmpty(searchText))
                return new List<Product>();
            try
            {
#if WINDOWS
            var skipCount = (page.HasValue ? page.Value : 0) * 10;

            var model = _database.Table<Product>().Where(a =>
           ((a.ModelNumber != null && a.ModelNumber.ToLower().Contains(searchText.ToLower())) ||
            (a.Brand != null && a.Brand.ToLower().Contains(searchText.ToLower())) ||
            (a.YasSize != null && a.YasSize.ToLower().Contains(searchText.ToLower())) ||
            (a.InnerFabricSupply != null && a.InnerFabricSupply.ToLower().Contains(searchText.ToLower())) ||
            (a.InnerFabricName != null && a.InnerFabricName.ToLower().Contains(searchText.ToLower())) ||
            (a.OuterFabricSupply != null && a.OuterFabricSupply.ToLower().Contains(searchText.ToLower())) ||
            (a.OuterFabricName != null && a.OuterFabricName.ToLower().Contains(searchText.ToLower()))) &&
           !a.IsDeleted);



            
                int parsedYear;
                if (year != null && int.TryParse(year, out parsedYear) && parsedYear != 0)
                {
                    //return model.AsEnumerable().Where(a =>
                    //{
                    //    DateTime creationDate;
                    //    return DateTime.TryParse(a.Creationtime, out creationDate) && creationDate.Year == parsedYear;

                    //}).ToList();
                    var queryModel = model.AsEnumerable().Where(a =>
                    {
                        if (DateTime.TryParse(a.Creationtime, out DateTime creationDate))
                        {
                            return creationDate.Year == (year != null ? parsedYear : DateTime.Now.Year) && !a.IsDeleted;
                        }
                        return false;
                    });

                    if (page.HasValue && queryModel.Count() > 0)
                        return model.Skip(skipCount).Take(10).ToList();
                    else
                        return model.ToList();

                }
                else
                {
                    var queryModel = model.AsEnumerable().Where(a =>
                    {
                        DateTime creationDate;
                        return DateTime.TryParse(a.Creationtime, out creationDate) && creationDate.Year == DateTime.Now.Year && !a.IsDeleted;
                    });

                    if (page.HasValue && queryModel.Count() > 0)
                        return model.Skip(skipCount).Take(10).ToList();
                    else
                        return model.ToList();

                }

                return null;

#else
                return await MobileService.GetAsync<List<Product>>("MobileService/GetFilterProductOrder?searchText=" + searchText + "&year=" + year + "&page=" + page);

#endif
        }
            catch (Exception ex)
            {
#if WINDOWS
                                    AddLogData(ex.Message, "sale");
#endif
                                    ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return null;
            }


        }

        public static async Task<Sale> GetSaleById(long id)
        {
            try
            {
#if WINDOWS
                    return  _database.Find<Sale>(id); 
#else
                  return await MobileService.GetAsync<Sale>("MobileService/GetSaleById?id=" +id);

#endif
            }
            catch (Exception ex)
            {

#if WINDOWS
                                 AddLogData(ex.Message, "sale");
#endif
                                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return null;
            }

        }
        public static async Task UpdateSale(Sale sale, bool entegrasyon = false)
        {
            try
            {
                sale.LastModificationTime = DateTime.Now;


#if WINDOWS
                if (!entegrasyon)
                {
                    sale.IsDeleted = false;
                }
                            _database.Update(sale);
                            AddLogData(sale, "sale");
#else
                await MobileService.PostAsync<Sale,string>("MobileService/UpdateSale",sale);

#endif
            }
            catch (Exception ex)
            {

#if WINDOWS
                                                 AddLogData(ex.Message, "sale");
#endif
                                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
            }
        }
        public static async Task DeleteSale(long id)
        {
            //_database.Delete<Sale>(id);
            try
            {

#if WINDOWS
                                  var model = _database.Find<Sale>(id);
                                            model.IsDeleted = true;
                                              model.LastModificationTime = DateTime.Now;
                                            _database.Update(model);
                                            AddLogData(model, "sale");
                                            var list = _database.Table<OrderDetails>().Where(a => a.SaleId == id).ToList();
                                            foreach (var item in list)
                                            {
                                                // _database.Delete<OrderDetails>(item.Id);
                                                var model2 = _database.Find<OrderDetails>(item.Id);
                                                model2.IsDeleted = true;
                                                 model2.LastModificationTime = DateTime.Now;
                                                _database.Update(model2);
                                                Task.Delay(500);

                                                AddLogData(model2, "OrderDetails");

                                                //var orderModel = _database.Find<Order>(item.OrderId);
                                                //orderModel.IsDeleted = true;
                                                // orderModel.LastModificationTime = DateTime.Now;
                                                //_database.Update(orderModel);
                                                //Task.Delay(500);

                                                //AddLogData(orderModel, "Order");

                                            }
#else
                await MobileService.GetAsync<string>("MobileService/DeleteSale?id="+id);

#endif
            }
            catch (Exception ex)
                {
#if WINDOWS
                                                     AddLogData(ex.Message, "Order");
#endif
                    ExceptionAlert.ShowAlert("Uyarı", ex.Message);
            }
        }

        public static async Task<Sale> GetSaleWithSaleDetails(int saleId)
        {
            try
            {
#if WINDOWS
                             var sale = await GetSaleById(saleId);
                                            if (sale.SaleDetail != null)
                                            {
                                                sale.SaleDetail.Clear();
                                            }

                                            sale.SaleDetail =await  GetSaleDetailsBySaleId(saleId);
                                            foreach (var item in sale.SaleDetail)
                                            {
                                                item.Material =await GetMaterialById(item.MaterialId);
                                            }
                                            sale.Product =await GetProductById(sale.ProductId);
                                            return sale;

#else
                          return  await MobileService.GetAsync<Sale>("MobileService/GetSaleWithSaleDetails?id=" + saleId);

#endif
            }
            catch (Exception ex)
            {

#if WINDOWS
                                                     AddLogData(ex.Message, "GetSaleWithSaleDetails");
#else
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
#endif
                return null;
            }
        }
        public static async Task<bool> CheckSaleMaterial(int saleId,int materialId)
        {
            try
            {
#if WINDOWS
              var sale = await GetSaleById(saleId);
                            if (sale.SaleDetail != null)
                            {
                                sale.SaleDetail.Clear();
                            }
                            sale.SaleDetail = await GetSaleDetailsBySaleId(saleId);
                            return sale.SaleDetail.Where(a => a.MaterialId == materialId).Any();

#else
                           return await MobileService.GetAsync<bool>("MobileService/CheckSaleMaterial?saleId=" + saleId+ "&materialId="+ materialId);
#endif


            }
            catch (Exception ex)
            {
#if WINDOWS
                     AddLogData(ex.Message, "CheckSaleMaterial");
#endif
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return false;
            }
        }



        public static async Task<int> CreateOrder(Order order, string year, bool isEntegrasyon = false) {
            order.LastModificationTime = DateTime.Now;

            try
            {
#if WINDOWS
                if(!isEntegrasyon){
                        int parsedYear;
                        if (year != null && int.TryParse(year, out parsedYear) && parsedYear != 0)
                        {
                            var now = DateTime.Now;
                            order.Creationtime = new DateTime(parsedYear, now.Month, now.Day).ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            order.Creationtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                        }

                       // order.Creationtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                       }
                        _database.Insert(order);
                        AddLogData(order, "Order");
                        return order.Id; 
#else
                order.Creationtime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                return await MobileService.PostAsync<Order,int>("MobileService/CreateOrder?year=" + year,order);
#endif
            }
            catch (Exception ex)
            {

#if WINDOWS
                     AddLogData(ex.Message, "CreateOrder");
#endif
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return -1;
            }


        }
        public static async  Task<List<Order>> GetAllOrder(string year,int pageSize=0,bool asyncProgress=false) 
        {
            try
            {

#if WINDOWS
                var skipCount = pageSize * takeCount;

                int parsedYear;
                if (year != null && int.TryParse(year, out parsedYear) && parsedYear != 0)
                {
                    var model= _database.Table<Order>().AsEnumerable().Where(a =>
                    {
                        DateTime creationDate;
                        return DateTime.TryParse(a.Creationtime, out creationDate) && creationDate.Year == parsedYear && !a.IsDeleted;
                    });


                    if (model != null && model.Count() > 0 && !asyncProgress)
                    {
                        return model.Skip(skipCount).Take(takeCount).ToList();
                    }
                    else
                    {
                        return model.ToList();
                    }
                }
                else
                {
                    var model = _database.Table<Order>().AsEnumerable().Where(a =>
                    {
                        DateTime creationDate;
                        return DateTime.TryParse(a.Creationtime, out creationDate) && creationDate.Year == DateTime.Now.Year && !a.IsDeleted;
                    });

                    if (model!=null && model.Count()>0 && !asyncProgress)
                    {
                        return model.Skip(skipCount).Take(takeCount).ToList();
                    }
                    else
                    {
                        return model.ToList();
                    }
                }
#else
                return await MobileService.GetAsync<List<Order>>("MobileService/GetAllOrder?year=" + year+ "&pageSize="+ pageSize);

#endif

            }
            catch (Exception ex)
            {

#if WINDOWS
                     AddLogData(ex.Message, "GetAllOrder");
#endif
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return null;
            }
        }

        public static async Task<List<Order>> GetAllOrderIncludeDelete(string year, int pageSize = 0, bool asyncProgress = false)
        {
            try
            {

#if WINDOWS
                var skipCount = pageSize * takeCount;

                int parsedYear;
                if (year != null && int.TryParse(year, out parsedYear) && parsedYear != 0)
                {
                    var model= _database.Table<Order>().AsEnumerable().Where(a =>
                    {
                        DateTime creationDate;
                        return DateTime.TryParse(a.Creationtime, out creationDate) && creationDate.Year == parsedYear ;
                    });


                    if (model != null && model.Count() > 0 && !asyncProgress)
                    {
                        return model.Skip(skipCount).Take(takeCount).ToList();
                    }
                    else
                    {
                        return model.ToList();
                    }
                }
                else
                {
                    var model = _database.Table<Order>().AsEnumerable().Where(a =>
                    {
                        DateTime creationDate;
                        return DateTime.TryParse(a.Creationtime, out creationDate) && creationDate.Year == DateTime.Now.Year;
                    });

                    if (model!=null && model.Count()>0 && !asyncProgress)
                    {
                        return model.Skip(skipCount).Take(takeCount).ToList();
                    }
                    else
                    {
                        return model.ToList();
                    }
                }
#else
                return await MobileService.GetAsync<List<Order>>("MobileService/GetAllOrder?year=" + year + "&pageSize=" + pageSize);

#endif

            }
            catch (Exception ex)
            {

#if WINDOWS
                     AddLogData(ex.Message, "GetAllOrder");
#endif
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return null;
            }
        }
        public static async  Task<List<Order>> GetAllOrderMain(string year)
        {
            List<Order> orderList = new List<Order>();

            foreach (var item in await GetAllOrder(year))
            {
                Order order = item;
                order.OrderDetails.AddRange(await GetOrderDetailByOrderId(item.Id));
            }

            return orderList;

        }
        public static async  Task<int> CreateOrderDetail(OrderDetails order) 
        {
            try
            {
                order.LastModificationTime = DateTime.Now;
#if WINDOWS
                                    _database.Insert(order);


                                    AddLogData(order, "CreateOrderDetail");

                                    return order.Id; 
#else

                return await MobileService.PostAsync<OrderDetails, int>("MobileService/CreateOrderDetail", order);

#endif
                        }
                        catch (Exception ex)
                        {

#if WINDOWS
                                             AddLogData(ex.Message, "CreateOrderDetail");
#endif
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                            return -1;
            }

        }
        public static async Task<List<OrderDetails>> GetAllOrderDetail()
        {
       

            try
            {
#if WINDOWS
                        return _database.Table<OrderDetails>().Where(a=>!a.IsDeleted).ToList();
#else
                return await MobileService.GetAsync<List<OrderDetails>>("MobileService/GetAllOrderDetail");

#endif
            }
            catch (Exception ex)
            {

#if WINDOWS
                                 AddLogData(ex.Message, "GetAllOrderDetail");
#endif
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return null;
            }
        }
        public static async Task<List<OrderDetails>> GetAllOrderDetailIncludeDelete()
        {


            try
            {
#if WINDOWS
                        return _database.Table<OrderDetails>().ToList();
#else
                return await MobileService.GetAsync<List<OrderDetails>>("MobileService/GetAllOrderDetail");

#endif
            }
            catch (Exception ex)
            {

#if WINDOWS
                                 AddLogData(ex.Message, "GetAllOrderDetail");
#endif
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return null;
            }
        }
        public static async Task<OrderDetails> GetOrderDetailById(int id)
        {


            try
            {
#if WINDOWS
            return _database.Table<OrderDetails>().FirstOrDefault(a=>a.Id==id&& !a.IsDeleted);
#else
                return await MobileService.GetAsync<OrderDetails>("MobileService/GetOrderDetailById?id="+id);

#endif
            }
            catch (Exception ex)
            {

#if WINDOWS
                                 AddLogData(ex.Message, "GetOrderDetailById");
#endif
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return null;
            }

        }
        public static async Task<OrderDetails> GetOrderDetailById1(int id)
        {


            try
            {
#if WINDOWS
            return _database.Table<OrderDetails>().FirstOrDefault(a=>a.Id==id);
#else
                return await MobileService.GetAsync<OrderDetails>("MobileService/GetOrderDetailById?id=" + id);

#endif
            }
            catch (Exception ex)
            {

#if WINDOWS
                                 AddLogData(ex.Message, "GetOrderDetailById");
#endif
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return null;
            }

        }
        public static async  Task<List<OrderDetails>> GetOrderDetailByOrderId(int id)
        {
          



            try
            {
#if WINDOWS
              var model = _database.Table<OrderDetails>().Where(a => a.OrderId == id&&!a.IsDeleted).ToList();
            foreach (var item in model)
            {
                item.Sale = await GetSaleWithSaleDetails(item.SaleId);
            }
            return model;
#else
                return await MobileService.GetAsync<List<OrderDetails>>("MobileService/GetOrderDetailByOrderId?id=" + id);

#endif
            }
            catch (Exception ex)
            {

#if WINDOWS
                                 AddLogData(ex.Message, "GetOrderDetailByOrderId");
#endif
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return null;
            }
        }
        
        public static async  Task<Order> GetOrderById(int id)
        {
          

            try
            {
#if WINDOWS
            var model = _database.Table<Order>().Where(a => a.Id == id&& !a.IsDeleted).FirstOrDefault();

            model.OrderDetails = await GetOrderDetailByOrderId(id);

            return model;
#else
                return await MobileService.GetAsync<Order>("MobileService/GetOrderById?id=" + id);

#endif
            }
            catch (Exception ex)
            {

#if WINDOWS
                                 AddLogData(ex.Message, "GetOrderById");
#endif
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return null;
            }
        }

        public static async Task<Order> GetOrderByIdForEntegrasyon(int id)
        {


            try
            {
#if WINDOWS
            var model = _database.Table<Order>().Where(a => a.Id == id&& !a.IsDeleted).FirstOrDefault();
            return model;
#else
                return await MobileService.GetAsync<Order>("MobileService/GetOrderById?id=" + id);

#endif
            }
            catch (Exception ex)
            {

#if WINDOWS
                                 AddLogData(ex.Message, "GetOrderById");
#endif
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return null;
            }
        }
        public static async Task<Order> GetOrderByIdForEntegrasyon1(int id)
        {


            try
            {
#if WINDOWS
            var model = _database.Table<Order>().Where(a => a.Id == id).FirstOrDefault();
            return model;
#else
                return await MobileService.GetAsync<Order>("MobileService/GetOrderById?id=" + id);

#endif
            }
            catch (Exception ex)
            {

#if WINDOWS
                                 AddLogData(ex.Message, "GetOrderById");
#endif
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return null;
            }
        }
        public static async Task UpdateOrder(Order order,bool entegrasyon=false)
        {

            order.LastModificationTime = DateTime.Now;
           
            try
            {
#if WINDOWS
            if (!entegrasyon)
                order.IsDeleted = false;

            var model = _database.Update(order);
            AddLogData(order, "Order");
#else
                await MobileService.PostAsync<Order,string>("MobileService/UpdateOrder",order);
#endif
            }
            catch (Exception ex)
            {

#if WINDOWS
                AddLogData(ex.Message, "UpdateOrder");
#endif
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
            }

        }
        public static async Task UpdateOrderDetail(OrderDetails orderDetail,bool entegrasyon=false)
        {


            orderDetail.LastModificationTime = DateTime.Now;
         
            try
            {
#if WINDOWS
             if (!entegrasyon)
                orderDetail.IsDeleted = false;

            var model = _database.Update(orderDetail);

            AddLogData(model, "orderDetail");
#else
                await MobileService.PostAsync<OrderDetails, string>("MobileService/UpdateOrderDetail", orderDetail);

#endif
            }
            catch (Exception ex)
            {

#if WINDOWS
                                 AddLogData(ex.Message, "UpdateOrderDetail");
#endif
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
            }





        }
        public static async Task DeleteOrder(int id) {

            //try
            //{
               
            //}
            //catch (Exception ex )
            //{
            //    AddLogData(ex.Message, "Order");
            //}

            try
            {
#if WINDOWS
        //_database.Delete<Order>(id);
                var model = _database.Find<Order>(id);
                model.IsDeleted = true;
                       model.LastModificationTime = DateTime.Now;
                _database.Update(model);

                AddLogData(model, "Order");
#else
                await MobileService.GetAsync<string>("MobileService/DeleteOrder?id=" + id);

#endif
            }
            catch (Exception ex)
            {

#if WINDOWS
                                 AddLogData(ex.Message, "DeleteOrder");
#endif
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
            }





        }
        public static async Task DeleteOrderDetail(int id) {
            //_database.Delete<OrderDetails>(id)
           



            try
            {
#if WINDOWS
            var model = _database.Find<OrderDetails>(id);
            model.IsDeleted = true;
              model.LastModificationTime = DateTime.Now;
            _database.Update(model);

            AddLogData(model, "OrderDetail");
#else
                await MobileService.GetAsync<string>("MobileService/DeleteOrderDetail?id=" + id);

#endif
            }
            catch (Exception ex)
            {

#if WINDOWS
                                 AddLogData(ex.Message, "DeleteOrderDetail");
#endif
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
            }

        }
        
        public static async Task<bool> GetOrderDetailCheckSale(int orderId,int saleId)
        {
            try
            {
               return await MobileService.GetAsync<bool>("MobileService/GetOrderDetailCheckSale?orderId=" + orderId+"&saleId="+ saleId);
            }
            catch (Exception ex)
            {
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);
                return true;
            }

        }
        /// <summary>
        /// /buradaki sorguya daha sonra bakılmalı tarihi sonrardan filtreliyorum bir sonraki güncellemede düzeltilmeli !!!!!!!!!!!!
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static async Task<List<Order>> GetFilterOrderAndProduct(string searchText, string year = "",int pageSize=0)
        {
            try
            {
#if WINDOWS

            var skipCount = pageSize * takeCount;


            int parsedYear;
            DateTime creationDate = DateTime.MinValue;

            // Yıl parametresi varsa ve geçerli bir yıl ise
            if (!string.IsNullOrEmpty(year) && int.TryParse(year, out parsedYear) && parsedYear != 0)
            {
                // Yıl parametresi geçerliyse, creationDate'ı bu yıla göre ayarla
                creationDate = new DateTime(parsedYear, 1, 1); // Yılın ilk günü
            }
            else
            {
                // Eğer yıl parametresi yoksa, geçerli tarihi kullan
                creationDate = DateTime.Now;
            }
        
            //var model = _database.Table<Order>()
            //                      .Where(a => a.Title.Contains(searchText)
            //                                  && !a.IsDeleted )
            //                      .ToList();
          var models=   _database.Table<Order>().AsEnumerable().Where(a =>
            {
                return DateTime.TryParse(a.Creationtime, out creationDate) && creationDate.Year == DateTime.Now.Year && !a.IsDeleted &&
                (a.Title != null && a.Title.ToLower().Contains(searchText.ToLower())) || (a.Description != null && a.Description.ToLower().Contains(searchText.ToLower()));
                //((a.Title != null ? a.Title.ToLower().Contains(searchText.ToLower()) : false));
            });


            if (models.Count() > 0)
            {
                models= models.Skip(skipCount).Take(takeCount).ToList();
            }
            else
            {
                models= models.ToList();
            }
            // OrderDetails'ları yüklemek için döngü
            foreach (var item in models)
            {
                item.OrderDetails =await  GetOrderDetailByOrderId(item.Id);
            }

            return models.ToList();

#else
              return  await MobileService.GetAsync<List<Order>>("MobileService/GetFilterOrderAndProduct?searchText=" + searchText+ "&year="+ year+"&pageSize="+pageSize);

#endif
            }
            catch (Exception ex)
            {


#if WINDOWS
                                 AddLogData(ex.Message, "GetFilterOrderAndProduct");
#endif
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);

                return null;
            }


        
        }
        public static async Task<List<Order>> GetFilterOrderAndProductExportList(string searchText, string year = "", int pageSize = 0)
        {


            try
            {

#if WINDOWS
  var skipCount = pageSize * takeCount;


                int parsedYear;
                DateTime creationDate = DateTime.MinValue;

                if (!string.IsNullOrEmpty(year) && int.TryParse(year, out parsedYear) && parsedYear != 0)
                {
                    creationDate = new DateTime(parsedYear, 1, 1); // Yılın ilk günü
                }
                else
                {
                    creationDate = DateTime.Now;
                }

          
                var models = _database.Table<Order>().AsEnumerable().Where(a =>
                {
                    return DateTime.TryParse(a.Creationtime, out creationDate) && creationDate.Year == DateTime.Now.Year && !a.IsDeleted &&
                    (a.Title != null && a.Title.ToLower().Contains(searchText.ToLower())) || (a.Description != null && a.Description.ToLower().Contains(searchText.ToLower()));
                });


                if (models.Count() > 0)
                {
                    models = models.Skip(skipCount).Take(takeCount).ToList();
                }
                else
                {
                    models = models.ToList();
                }

                return models.ToList();

#else
                return await MobileService.GetAsync<List<Order>>("MobileService/GetFilterOrderAndProductExportList?searchText=" + searchText + "&year=" + year + "&pageSize=" + pageSize);

#endif
            }
            catch (Exception ex)
            {
              
#if WINDOWS
                                 AddLogData(ex.Message, "GetFilterOrderAndProduct");
#endif
                ExceptionAlert.ShowAlert("Uyarı", ex.Message);

                return null;


            }

        }

    }

}

