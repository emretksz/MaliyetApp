using CommunityToolkit.Maui.Views;

namespace MaliyetApp.Libs.AppSettings;

public partial class LoadingPopup : Popup
{
	public LoadingPopup()
	{
        InitializeComponent();
	}

	public void ClosePopupMethod()
	{
		this.Close();
	}
}