using CommunityToolkit.Maui.Views;
using MaliyetApp.Libs.Models;
using MaliyetApp.Libs.Models.SQlitecontextDb;
using SQLite;

namespace MaliyetApp.Views.Actions;

public partial class DeletePopup : Popup
{
    private int _id;
    private string _type;
    public DeletePopup(int id,string type)
	{
		InitializeComponent();
        _id = id;
        _type = type;

    }

    private async void Sil(object sender, EventArgs e)
    {
        if (_type=="order")
        {
           
            DatabaseService.DeleteOrder(_id);
           
            await this.CloseAsync();
        }
  
    }
    private async void Iptal(object sender, EventArgs e)
    {
        await this.CloseAsync();

    }
}