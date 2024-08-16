using CommunityToolkit.Mvvm.Input;
using MauiApp2.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using MauiApp2.Messages;
using MauiApp2.View;
using CommunityToolkit.Maui.Views;

namespace MauiApp2.ViewModel;

public partial class DishViewModel : INotifyPropertyChanged
{
    internal DataBase db = new DataBase();

    public event PropertyChangedEventHandler PropertyChanged;

    public Dish SelectedDish { get; set; }

    public string PortionSize { get; set; }

    public ObservableCollection<Dish> Dishes { get; set; }

    public DeletionConfirmationPopUp PopUp {get; set;}
    public DishViewModel()
    {
        WeakReferenceMessenger.Default.Register<NewDishAdded>(this, (r, m) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ReloadList();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Dishes)));
            });
        });
        ReloadList();
    }

    private void ReloadList()
    {
        Dishes = new ObservableCollection<Dish>();
        var allDishes = db.GetAllDishes();
        foreach (var dish in allDishes)
        {
            Dishes.Add(dish);
        }
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Dishes)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedDish)));
    }

    public void Reset()
    {
        SelectedDish = null;
        PortionSize = "";
        PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs("SelectedDish"));
        PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs("PortionSize"));
    }

    [RelayCommand]
    public void AddToShopList()
    {
        try
        {
            if (SelectedDish == null)
                throw new Exception("Nie wybrałeś dania do dodania");
            
            if (!Double.TryParse(PortionSize, out var d))
                throw new Exception("Rozmiar porcji musi być liczbą");
            
            if (Convert.ToDouble(PortionSize) <= 0)
                throw new Exception("Nie da się fizycznie zrobić ujemnej ilości jedzenia");
            
            if (PortionSize == null)
                PortionSize = "1";
        }
        catch (Exception ex)
        {
            var popup = new AlertPopUp(ex.Message);
            Shell.Current.ShowPopup(popup);
        }
        
        var temp = Convert.ToDouble(PortionSize);
        db.AddShoppingList(new ShoppingList(SelectedDish.DishId,temp));
                
        WeakReferenceMessenger.Default.Send(new NewElementsInShoppingList("Reload"));
        Reset();
    }
    [RelayCommand]

    public void RemoveDishButtonPressed()
    {
        if (SelectedDish == null) return;
        PopUp = new DeletionConfirmationPopUp(this);
        Shell.Current.ShowPopup(PopUp);
    }

    [RelayCommand]
    public void RemoveDishPermanently()
    {
        db.RemoveDishDB(SelectedDish.DishId);
        Reset();
        ReloadList();
        PopUp.Close();
    }
}
