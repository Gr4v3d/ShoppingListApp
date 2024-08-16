using MauiApp2.Model;
namespace MauiApp2.ViewModel;

public class Composition
{
    public ShoppingList NumberOnTheList { get; set; }
    
    public ShoppingListElement IngredientCounter { get; set; }
    
    public Dish TheDish { get; set; }
    
    public IngredientList Ingredient { get; set; }

    public double RequiredAmount { get; set; }
    
    public double Difference {  get; set; }
    
    public Composition(ShoppingList number, ShoppingListElement element, Dish dish, IngredientList ingredient)
    {
        NumberOnTheList = number;
        TheDish = dish;
        IngredientCounter = element;
        Ingredient = ingredient;
        RequiredAmount = NumberOnTheList.PortionSize * Ingredient.IngredientCount;
        Difference = RequiredAmount - IngredientCounter.IngredientAmountOwned;
    }
}
