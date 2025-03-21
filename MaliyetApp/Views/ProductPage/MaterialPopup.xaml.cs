using CommunityToolkit.Maui.Views;
using MaliyetApp.Libs.Models;
using MaliyetApp.Libs.Models.SQlitecontextDb;
using System.Collections.ObjectModel;
using System.Linq;
namespace MaliyetApp.Views.ProductPage;
public partial class MaterialPopup : Popup
{
    public Action<List<SaleDetail>> ListAction;
    private int _saleId;
    public MaterialPopup(int saleId,List<SaleDetail> _temp)
    {
        InitializeComponent();
        // Ekran boyutlarýný al
        var screenWidth = DeviceDisplay.MainDisplayInfo.Width;
        var screenHeight = DeviceDisplay.MainDisplayInfo.Height;
        _saleId = saleId;


        // Popup boyutlarýný oransal olarak ayarla
        //var width = screenWidth * 0.8;  // Ekranýn %80'i
        //var height = screenHeight * 0.6; // Ekranýn %60'ý

        //// Popup içeriðinin boyutunu ayarla
        //popupContent.WidthRequest = width;
        //popupContent.HeightRequest = height;
#if WINDOWS
        Dispatcher.DispatchAsync( async() =>
        {
            // materialList.AddRange(model);
            var materialId = _temp.Select(a => a.MaterialId).ToList();
            var model =( await DatabaseService.GetAllMaterials()).Where(a => !materialId.Contains(a.Id)).ToList();
            list1.ItemsSource = model;
        });

#else
   MainThread.BeginInvokeOnMainThread( async() =>
        {
            // materialList.AddRange(model);
            var materialId = _temp.Select(a => a.MaterialId).ToList();
            var model =( await DatabaseService.GetAllMaterials()).Where(a => !materialId.Contains(a.Id)).ToList();
            list1.ItemsSource = model;
        });
#endif

    }

    private void OnSubmitPopup(object sender, EventArgs e)
    {
        this.Close();
    }

    private void Button_Clicked(object sender, EventArgs e)
    {

        List<SaleDetail> sales = new List<SaleDetail>();
        foreach (var item in materialList)
        {
            sales.Add(new()
            {
                MaterialId = item.Id,
                Material = item,
                SaleId = _saleId
            });
        }

        ListAction?.Invoke(sales);
        this.Close();
    }
    List<Material> materialList = new List<Material>();
    private void Material_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        var unSelectedModel = (CheckBox)sender;

    }

    private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        var unSelectedModel = (CheckBox)sender;
        var model = unSelectedModel.BindingContext as Material;
        if (unSelectedModel.IsChecked)
        {

            materialList.Add(model);
        }
        else
        {
            var result = materialList.Where(a => a.Id == model.Id).FirstOrDefault();
            materialList.Remove(result);
            var ssss = "";
        }
    }

    private void Button_Clicked_1(object sender, EventArgs e)
    {
        this.Close();
    }
}