using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using MauiApp2.Messages;
using MauiApp2.Model;
using MauiApp2.ViewModel;

namespace MauiApp2.View;

public partial class ShopListPopUp : Popup
{
	public ShopListPopUp(Composition inherited)
	{
		Inherited = inherited;
		InitializeComponent();
		Title.Text = Inherited.Ingredient.IngredientName;
		Needed.Text = Convert.ToString(Inherited.RequiredAmount);
		Measure.Text = Inherited.Ingredient.Measure;
	}
	
	public Composition Inherited {  get; set; }

	public double AmountOwned { get; set; }
    private void CancelClicked(object sender, EventArgs e)
    {
		this.Close();
    }

	private void ConfirmClicked(object sender, EventArgs e)
	{
		try
		{
			AmountOwned = Convert.ToDouble(Have.Text);
		}
		catch
		{
			Have.Text = "Proszê podaæ liczbê";
			return;
		}
		var dbAccess = new DataBase();
		dbAccess.ChangeAmountOwned(Inherited.NumberOnTheList.ShoppingListID, Inherited.Ingredient.IngredientName, AmountOwned);
		WeakReferenceMessenger.Default.Send(new ShoppingListValueChanged("Reload"));
		Close();

	}
}