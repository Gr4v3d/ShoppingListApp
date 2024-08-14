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
        ListOfIngerdients = new List<Composition>();
        foreach ( var position in listOfDishes)
        {
            var dish = dbAccess.GetDish(position.DishId);
            var elements = dbAccess.GetShoppingListElements(position.ShoppingListID);
            foreach ( var element in elements)
            {
                var ingredient = dbAccess.GetIngredient(position.DishId,element.IngredientName);
                ListOfIngerdients.Add(new Composition(position, element,dish,ingredient));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public List<Composition> ListOfIngerdients { get; set; }
    public ICommand Guzik { get; }

    [RelayCommand]
    public void DisplayPopUp()
    {
        var popup = new ShopListPopUp();
        Shell.Current.ShowPopup(popup);
    }

    public class Composition
    {
        public ShoppingList NumberOnTheList { get; set; }
        public ShoppingListElement IngredientCounter { get; set; }
        public Dish TheDish { get; set; }
        public IngredientList Ingredient { get; set; }
        
        public double RequiredAmount { get; set; }
        public Composition(ShoppingList number, ShoppingListElement element, Dish dish, IngredientList ingredient)
        {
            NumberOnTheList = number;
            TheDish = dish;
            IngredientCounter = element;
            Ingredient = ingredient;
            RequiredAmount = NumberOnTheList.PortionSize * Ingredient.IngredientCount;
        }
    }
}
