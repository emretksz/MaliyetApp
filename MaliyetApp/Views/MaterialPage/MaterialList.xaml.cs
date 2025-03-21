using CommunityToolkit.Maui.Views;
using MaliyetApp.Libs.Models;
using MaliyetApp.Libs.Models.SQlitecontextDb;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Platform;
using System.Collections.ObjectModel;

namespace MaliyetApp.Views.MaterialPage;

public partial class MaterialList : ContentPage
{
    public Action<bool> ReturnValue;
	public MaterialList()
	{
		InitializeComponent();
        
    }
    ObservableCollection<Material> models = new ObservableCollection<Material>();
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var model = await DatabaseService.GetAllMaterials();
        models = new ObservableCollection<Material>();
        foreach (var item in model)
        {
            models.Add(item);
        }
        Dispatcher.Dispatch(() =>
        {

            //  materialList.ItemsSource = null;
            materialList.ItemsSource = models;
        });
        var panGesture = new PanGestureRecognizer();
        panGesture.PanUpdated += OnPanUpdated;

        //scrollbarview.GestureRecognizers.Add(panGesture);

    }
    private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        //if (e.StatusType == GestureStatus.Running)
        //{
        //    // Kayd�rma hareketi yap�l�yor
        //    var deltaY = e.TotalY;

        //    // Y�ksek delta ile a�a��ya veya yukar�ya kayd�rma yapma
        //    if (deltaY > 0)
        //    {
        //        // A�a�� kayd�rma
        //        scrollbarview.ScrollToAsync(0, scrollbarview.Height, true);
        //    }
        //    else if (deltaY < 0)
        //    {
        //        // Yukar� kayd�rma
        //        scrollbarview.ScrollToAsync(0, 0, true);
        //    }
        //}
    }
    private async void Button_Clicked(object sender , EventArgs e)
	{
        var popup = new CreateMaterial();
        popup.Action += async (sender) =>
        {
            var model =  await DatabaseService.GetAllMaterials();
            models = new ObservableCollection<Material>();
            foreach (var item in model)
            {
                models.Add(item);
            }
            materialList.ItemsSource = null;
            materialList.ItemsSource = models;
        };
        await this.ShowPopupAsync(popup);
        //this.ShowPopup(new CreateMaterial());
    
	}

  
}