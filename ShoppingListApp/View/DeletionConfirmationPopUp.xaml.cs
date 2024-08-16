using MauiApp2.ViewModel;
namespace MauiApp2.View;

public partial class DeletionConfirmationPopUp
{
	public DeletionConfirmationPopUp(DishViewModel context)
	{
		BindingContext = context;
		InitializeComponent();
	}
}