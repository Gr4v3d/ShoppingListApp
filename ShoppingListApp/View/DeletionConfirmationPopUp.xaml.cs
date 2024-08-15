using CommunityToolkit.Mvvm.Messaging;
using MauiApp2.Messages;

namespace MauiApp2.View;

public partial class DeletionConfirmationPopUp
{
	public DeletionConfirmationPopUp()
	{
		InitializeComponent();
	}

    private void Button_Clicked(object sender, EventArgs e)
    {
		WeakReferenceMessenger.Default.Send(new DeletionMessage("Dump it"));
		Close();
    }
}