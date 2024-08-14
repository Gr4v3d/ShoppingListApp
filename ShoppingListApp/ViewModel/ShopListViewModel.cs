using CommunityToolkit.Mvvm.Input;
using MauiApp2.View;
using CommunityToolkit.Maui.Views;
using System.ComponentModel;
using System.Windows.Input;
using MauiApp2.Model;

namespace MauiApp2.ViewModel;

public partial class ShopListViewModel : INotifyPropertyChanged
{
    public ShopListViewModel() 
    {
        var dbAccess = new DataBase();
        var listOfDishes = dbAccess.GetShoppingList();
        foreach ( var dish in listOfDishes)
        {
            var elements = dbAccess.GetShoppingListElements(dish.ShoppingListID);
            foreach ( var element in elements)
            {
                ListOfIngerdients.Add(new Composition(dish, element));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    internal List<Composition> ListOfIngerdients = new List<Composition>();
    public ICommand Guzik { get; }

    [RelayCommand]
    public void DisplayPopUp()
    {
        var popup = new ShopListPopUp();
        Shell.Current.ShowPopup(popup);
    }

    internal class Composition
    {
        public ShoppingList Dish { get; set; }
        public ShoppingListElement Element { get; set; }
        public Composition(ShoppingList dish, ShoppingListElement element)
        {
            Dish = dish;
            Element = element;
        }
    }
}
