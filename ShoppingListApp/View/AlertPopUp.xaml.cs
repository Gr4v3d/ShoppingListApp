using CommunityToolkit.Maui.Views;
namespace MauiApp2.View;

public partial class AlertPopUp : Popup
{
	public AlertPopUp(string message)
	{
        InitializeComponent();
        TheMessage.Text = message;
    }
}