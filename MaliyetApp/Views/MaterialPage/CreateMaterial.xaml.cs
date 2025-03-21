using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using MaliyetApp.Libs.Models;
using MaliyetApp.Libs.Models.SQlitecontextDb;

namespace MaliyetApp.Views.MaterialPage;

public partial class CreateMaterial : Popup
{
	public Action<bool> Action;
	public CreateMaterial()
	{
        InitializeComponent();
	}

    private async void Button_Clicked(object sender, EventArgs e)
    {

		if (String.IsNullOrEmpty(malzemeAdi.Text))
		{
            await Toast.Make("Malzeme adý boþ olamaz.", ToastDuration.Short).Show();
            return;
		}

		if (!m.IsChecked&&!gr.IsChecked&&!adet.IsChecked)
		{
            await Toast.Make("Malzeme ölçütü boþ olamaz.", ToastDuration.Short).Show();
            return;
        }
		var malzeme = new Material()
		{
			Name = malzemeAdi.Text,
			Type = m.IsChecked ? "Metre" : (gr.IsChecked ? "Gr" : "Adet")
		};
		DatabaseService.CreateMetarial(malzeme);
        Action?.Invoke(true);
        this.Close();
        //DisplayAlert("Baþarýlý", "Malzeme kaydedildi", "Tmama");
    }
}