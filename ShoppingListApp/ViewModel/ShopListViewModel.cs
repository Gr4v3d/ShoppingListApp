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
        PopUp = new ShopListPopUp(this);
        WeakReferenceMessenger.Default.Register<ShoppingListValueChanged>(this, (r, m) => {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ReloadShoppingList();
            });
            });
        WeakReferenceMessenger.Default.Register<NewElementsInShoppingList>(this, (r, m) => {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ReloadShoppingList();
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
        ReadyDishes = CheckForReady(ListOfIngerdients);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReadyDishes)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ListOfIngerdients)));
    }

    ShopListPopUp PopUp { get; set; }
    public Composition SelectedElement { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public List<Composition> ListOfIngerdients { get; set; }

    public List<Composition> ReadyDishes { get; set; }

    public string PopUpEntry { get; set; }

    public List<Composition> CheckForReady(List<Composition> Original)
    {
        var toCheck = new List<Composition>(Original);
        var alreadyChecked = new List<Composition>(toCheck);
        foreach(var item in toCheck)
        {
            if (item.IngredientCounter.IngredientAmountOwned < item.RequiredAmount)
                alreadyChecked.RemoveAll(x => x.NumberOnTheList.ShoppingListID == item.NumberOnTheList.ShoppingListID);            
        }
        return alreadyChecked.DistinctBy(x => x.NumberOnTheList.ShoppingListID).ToList();
    }

    [RelayCommand]
    public async Task DisplayPopUp()
    {
        PopUp = new ShopListPopUp(this);
        if(SelectedElement == null) return;
        await Shell.Current.ShowPopupAsync(PopUp);
    }
    [RelayCommand]

    public void RemoveChosenDishFromList()
    {
        if(SelectedElement == null) return;
        var dbAccess = new DataBase();
        dbAccess.RemoveDishFromShoppingList(SelectedElement.NumberOnTheList.ShoppingListID);
        ReloadShoppingList();
    }

    [RelayCommand]

    public void ChangeAmountOwned()
    {
        try
        {
            SelectedElement.IngredientCounter.IngredientAmountOwned = Convert.ToDouble(PopUpEntry);
        }
        catch
        {
            var poper = new AlertPopUp("Proszę podać liczbę");
            return;
        }
        var dbAccess = new DataBase();
        dbAccess.ChangeAmountOwned(SelectedElement.NumberOnTheList.ShoppingListID, SelectedElement.Ingredient.IngredientName, SelectedElement.IngredientCounter.IngredientAmountOwned);
        PopUp.Close();
        ReloadShoppingList();
    }
}
