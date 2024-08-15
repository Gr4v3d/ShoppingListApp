using CommunityToolkit.Mvvm.Input;
using MauiApp2.View;
using CommunityToolkit.Maui.Views;
using System.ComponentModel;
using MauiApp2.Model;
using CommunityToolkit.Mvvm.Messaging;
using MauiApp2.Messages;

namespace MauiApp2.ViewModel;

public partial class ShopListViewModel : INotifyPropertyChanged
{
    public ShopListViewModel() 
    {
        WeakReferenceMessenger.Default.Register<ShoppingListValueChanged>(this, (r, m) => {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ReloadShoppingList();
                PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(nameof(ListOfIngerdients)));
            });
            });
        WeakReferenceMessenger.Default.Register<NewElementsInShoppingList>(this, (r, m) => {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ReloadShoppingList();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ListOfIngerdients)));
            });
        });
        ReloadShoppingList();
    }

    private void ReloadShoppingList()
    {
        var dbAccess = new DataBase();
        var listOfDishes = dbAccess.GetShoppingList();
        ListOfIngerdients = new List<Composition>();
        foreach (var position in listOfDishes)
        {
            var dish = dbAccess.GetDish(position.DishId);
            var elements = dbAccess.GetShoppingListElements(position.ShoppingListID);
            foreach (var element in elements)
            {
                var ingredient = dbAccess.GetIngredient(position.DishId, element.IngredientName);
                ListOfIngerdients.Add(new Composition(position, element, dish, ingredient));
            }
        }
    }
    public ShopListPopUp popup {  get; set; }
    public Composition SelectedElement { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public List<Composition> ListOfIngerdients { get; set; }

    [RelayCommand]
    public async Task DisplayPopUp()
    {
        if(SelectedElement == null) return;
        popup = new ShopListPopUp(SelectedElement);
        await Shell.Current.ShowPopupAsync(popup);
    }
    [RelayCommand]

    public void RemoveChosenDishFromList()
    {
        var dbAccess = new DataBase();
        dbAccess.RemoveDishFromShoppingList(SelectedElement.NumberOnTheList.ShoppingListID);
        ReloadShoppingList();
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ListOfIngerdients)));
    }
}
