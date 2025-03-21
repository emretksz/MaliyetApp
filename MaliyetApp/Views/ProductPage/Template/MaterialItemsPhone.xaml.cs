using MaliyetApp.Libs.Models;
using static MaliyetApp.Libs.AppSettings.AppConst;

namespace MaliyetApp.Views.ProductPage.Template;

public partial class MaterialItemsPhone : ContentView
{
	public MaterialItemsPhone()
	{
		InitializeComponent();
	}
    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        if (BindingContext!=null)
        {

        var model = (SaleDetail)BindingContext;
        try
        {

            switch (model.MarketRateType)
            {
                case "Dolar":
                    dolarButton.BackgroundColor = Colors.LightBlue;
                    euroButton.BackgroundColor = Colors.LightGray;
                    tlButton.BackgroundColor = Colors.LightGray;
                    marketRateType = MarketRateType.Dolar;
                    break;
                case "Euro":
                    euroButton.BackgroundColor = Colors.LightBlue;
                    dolarButton.BackgroundColor = Colors.LightGray;
                    tlButton.BackgroundColor = Colors.LightGray;
                    marketRateType = MarketRateType.Euro;
                    break;
                case "TL":
                    euroButton.BackgroundColor = Colors.LightGray;
                    dolarButton.BackgroundColor = Colors.LightGray;
                    tlButton.BackgroundColor = Colors.LightBlue;
                    marketRateType = MarketRateType.TL;
                    break;
                default:
                    euroButton.BackgroundColor = Colors.LightGray;
                    dolarButton.BackgroundColor = Colors.LightGray;
                    tlButton.BackgroundColor = Colors.LightBlue;
                    break;
            }
        }
        catch (Exception ex)
        {

        }
    
        }

    }
    private MarketRateType marketRateType;
    private void OnDolarSelected(object sender, EventArgs e)
    {

        dolarButton.BackgroundColor = Colors.LightBlue;
        euroButton.BackgroundColor = Colors.LightGray;
        tlButton.BackgroundColor = Colors.LightGray;
        marketRateType = MarketRateType.Dolar;
    }

    private void OnEuroSelected(object sender, EventArgs e)
    {
        euroButton.BackgroundColor = Colors.LightBlue;
        dolarButton.BackgroundColor = Colors.LightGray;
        tlButton.BackgroundColor = Colors.LightGray;
        marketRateType = MarketRateType.Euro;

    }
    private void OnTlSelected(object sender, EventArgs e)
    {
        euroButton.BackgroundColor = Colors.LightGray;
        dolarButton.BackgroundColor = Colors.LightGray;
        tlButton.BackgroundColor = Colors.LightBlue;
        marketRateType = MarketRateType.TL;
    }
    private void birimFiyatiEntry_Completed(object sender, EventArgs e)
    {
        birimFiyatiEntry.Unfocus();
    }
    private void birimEntry_Completed(object sender, EventArgs e)
    {
        birimEntry.Unfocus();
    }
    private async void birimFiyatiEntry_TextChanged(object sender, TextChangedEventArgs e)
    {

        ////  await SecureStorage.SetAsync("Dolar", "18.5");
        ///
        bool buttonChange = false;

        if (euroButton.BackgroundColor == Colors.LightBlue)
        {
            marketRateType = MarketRateType.Euro;
            buttonChange = true;
        }
        if (dolarButton.BackgroundColor == Colors.LightBlue)
        {
            marketRateType = MarketRateType.Dolar;
            buttonChange = true;
        }
        if (tlButton.BackgroundColor == Colors.LightBlue)
        {
            marketRateType = MarketRateType.TL;
            buttonChange = true;

        }

        if (buttonChange)
        {
            switch (marketRateType)
            {
                case MarketRateType.Dolar:
                    if (!String.IsNullOrEmpty(birimEntry.Text) && !String.IsNullOrEmpty(birimFiyatiEntry.Text))
                    {

                        //#if WINDOWS
                        //    dolarKuru = Preferences.Get("Dolar", 0f);
                        //#elif IOS
                        //    dolarKuru = Preferences.Get("Dolar", 0f);
                        //#endif
                        string dolarKuruString = await SecureStorage.GetAsync("Dolar");
                        float dolarKuru = string.IsNullOrEmpty(dolarKuruString) ? 0f : float.Parse(dolarKuruString);

                        float birim = float.Parse(birimEntry.Text);
                        float birimFiyati = float.Parse(birimFiyatiEntry.Text);
                        float toplam = birim * birimFiyati * dolarKuru;
                        toplamSonuc.Text = toplam.ToString("F2");
                        var model = (SaleDetail)BindingContext;
                        model.Price = toplam;
                        model.Unit = birim;
                        model.UnitePrice = birimFiyati;
                        model.MarketRateType = MarketRateType.Dolar.ToString();
                        model.MarketRate = dolarKuru;
                    }
                    break;
                case MarketRateType.Euro:
                    if (!String.IsNullOrEmpty(birimEntry.Text) && !String.IsNullOrEmpty(birimFiyatiEntry.Text))
                    {
                        string euroKuruString = await SecureStorage.GetAsync("Euro");
                        float euroKuru = string.IsNullOrEmpty(euroKuruString) ? 0f : float.Parse(euroKuruString);

                        float birim = float.Parse(birimEntry.Text);
                        float birimFiyati = float.Parse(birimFiyatiEntry.Text);

                        float toplam = birim * birimFiyati * euroKuru;

                        toplamSonuc.Text = toplam.ToString("F2");
                        var model = (SaleDetail)BindingContext;
                        model.Price = toplam;
                        model.Unit = birim;
                        model.UnitePrice = birimFiyati;
                        model.MarketRateType = MarketRateType.Euro.ToString();
                        model.MarketRate = euroKuru;
                    }
                    break;
                case MarketRateType.TL:
                    if (!String.IsNullOrEmpty(birimEntry.Text) && !String.IsNullOrEmpty(birimFiyatiEntry.Text))
                    {
                        float birim = float.Parse(birimEntry.Text);
                        float birimFiyati = float.Parse(birimFiyatiEntry.Text);

                        float toplam = birim * birimFiyati;

                        toplamSonuc.Text = toplam.ToString("F2");
                        var model = (SaleDetail)BindingContext;
                        model.Price = toplam;
                        model.Unit = birim;
                        model.UnitePrice = birimFiyati;
                        model.MarketRateType = MarketRateType.TL.ToString();

                        //var viewModel = (MaterialListViewModel)BindingContext;
                        //viewModel.Total = toplam;

                    }
                    break;
                default:
                    break;
            }
            buttonChange = false;
        }

    }

}