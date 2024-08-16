using CommunityToolkit.Maui.Views;
using MauiApp2.ViewModel;

namespace MauiApp2.View;

public partial class ShopListPopUp : Popup
{
	public ShopListPopUp(ShopListViewModel shopListViewModel)
	{
		BindingContext = shopListViewModel;
		InitializeComponent();
	}
	
    private void CancelClicked(object sender, EventArgs e)
    {
		this.Close();
    }
}