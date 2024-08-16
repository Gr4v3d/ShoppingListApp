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
    ShopListPopUp PopUp { get; set; }
    
    public Composition SelectedElement { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public List<Composition> ListOfIngerdients { get; set; }

    public List<Composition> ReadyDishes { get; set; }

    public string PopUpEntry { get; set; }

    public ShopListViewModel()
    {
        //Nasłuchiwacz który uruchamia się podczas dodawania nowych dań do listy z DishView
        WeakReferenceMessenger.Default.Register<NewElementsInShoppingList>(this, (r, m) => {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ReloadShoppingList();
            });
        });
        ReloadShoppingList();
    }

    //Pobiera liste dań z bazy danych
    private void ReloadShoppingList()
    {
        var dbAccess = new DataBase();

        //Pobieranie wszystkich dań które znajdują się na liście zakupów
        var listOfDishes = dbAccess.GetShoppingList();

        ListOfIngerdients = new List<Composition>();

        foreach (var position in listOfDishes)
        {
            //Pobieranie danych o konkretnym daniu
            var dish = dbAccess.GetDish(position.DishId);

            //Pobieranie wszystkich składników dla tego dania
            var elements = dbAccess.GetShoppingListElements(position.ShoppingListID);
            foreach (var element in elements)
            {
                //Tworzenie klasy-kompozycji aby ułatwić wyświetlanie danych w CollectionView-ie
                var ingredient = dbAccess.GetIngredient(position.DishId, element.IngredientName);
                ListOfIngerdients.Add(new Composition(position, element, dish, ingredient));
            }
        }
        //Sprawdza czy któreś z dań jest już gotowe
        ReadyDishes = CheckForReady(ListOfIngerdients);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReadyDishes)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ListOfIngerdients)));
    }

    public List<Composition> CheckForReady(List<Composition> Original)
    {
        //Kopia na bazie której sprawdzane będzie czy dania są gotowe
        var toCheck = new List<Composition>(Original);
        //Lista która będzie zwracana zawierająca jedynie gotowe dania
        var alreadyChecked = new List<Composition>(toCheck);
        
        foreach(var item in toCheck)
        {
            //Jeśli dla któregokolwiek rekordu ilość posiadanych składników będzie mniejsza od wymaganej, całe danie będzie usuwane ze zwracanej listy
            if (item.IngredientCounter.IngredientAmountOwned < item.RequiredAmount)
                alreadyChecked.RemoveAll(x => x.NumberOnTheList.ShoppingListID == item.NumberOnTheList.ShoppingListID);            
        }
        //Zwracaj tylko pojedyncze rekordy (a nie tyle ile każde danie ma składników
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
            //Sprawdzenie aby nie było żadnych błędnych danych na wejściu
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
