using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using MauiApp2.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MauiApp2.ViewModel;

internal class ShopListViewModel : INotifyPropertyChanged
{
    private readonly IPopupService popupService;

    public event PropertyChangedEventHandler? PropertyChanged;

    public ICommand Guzik { get; }

    public ShopListViewModel(IPopupService popupService)
    {
        this.popupService = popupService;
    }
    public ShopListViewModel()
    {
        Guzik = new AsyncRelayCommand(DisplayPopup);
    }
    private async Task DisplayPopup()
    {
        this.popupService.ShowPopup<PopUpViewModel>();
    }
}
